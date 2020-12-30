
module(..., package.seeall)

require "op_crc"
require "op_4g"
local all_data = ''
local buf_data1 = ''
local buf_data2 = ''
local index_recv_buf = 1


-- b: 低字节在前, string 类型
function bytes_to_int(b)
    ret = 0
    i = 0
    for j=1,b:len() do
        ret = ret + bit.lshift(b:byte(j),i)
        i = i+8
    end
    return ret
end

-- return b: 低字节在前
function bytes_from_int(v)
    value = v
    i = 0
    while value ~= 0 do
        i = i + 1
        value = bit.rshift(value,8)
    end
    ret = ''
    value = v
    while value ~= 0 do
        ret = ret..string.char(bit.band(0xff,value))
        value = bit.rshift(value,8)
    end

    return ret
end


-- 发送数据的函数
-- cmd: 长度为1的string类型
-- data: 适当长度的string类型
function data_pack(cmd,data)
    len_data = data:len()
    lens = bytes_from_int(len_data)
    len_lens = lens:len() -- int
    ret = cmd..string.char(len_lens)..lens
    ret = op_crc.ConCRC(ret)..data
    return ret
end

-- 对处理后得到的命令及数据进行解析，此处仅是将其打印
function task_parse(cmd,data)
    log.info('task_parse: ','cmd: '..cmd:toHex()..' data(hex): '..data:toHex())
    log.info('task_parse: ','cmd: '..cmd:toHex()..' data: '..data)
end

function recv_to_buf(data)
    if index_recv_buf == 1 then
        buf_data1 = buf_data1..data
        log.info('recv_to_buf','len of buf_data1: ',buf_data1:len())
    else
        buf_data2 = buf_data2..data
        log.info('recv_to_buf','len of buf_data2: ',buf_data2:len())
    end
end

function proc()
    log.info('op_data','into proc')
    local len_len_data = 0
    local len_data = 0
    log.info('op_data','len all_data is',all_data:len())
    while all_data:len() > 4 do
        log.info('op_data,while','all_data is '..all_data:toHex())
        len_len_data = all_data:byte(2)
        if (len_len_data > 0) and op_crc.DataCRC(all_data:sub(1,4+len_len_data)) then -- 如为 true 则是定位到了一条命令的开头
            len_data = bytes_to_int(all_data:sub(3,2+len_len_data)) -- len_len_data 至少为 1
            local a = len_data + 4 + len_len_data
            local b = all_data:len()
            if a > b then
                return -- 把 all_data 留给下一次处理
            elseif a < b then
                tmp_data = all_data:sub(1,a)
                all_data = all_data:sub(a+1,-1) -- 把富余部分留给下一次处理
            else
                tmp_data = all_data
                all_data = ''
            end
            if tmp_data:byte(1) ~= 99 then
                sys.taskInit(task_parse, tmp_data:sub(1,1),tmp_data:sub(len_len_data+5,-1))
                --parse(tmp_data[1],tmp_data:sub(len_len_data+5,-1))
            end
        else
            all_data = all_data:sub(2,-1)
        end
    end
    log.info('op_data','output proc')
end


sys.taskInit(function()
    sys.wait(6000)
    while true do
        if index_recv_buf == 1 then
            if buf_data2:len() > 0 then
                all_data = all_data..buf_data2
                buf_data2 = ''
                index_recv_buf = 2
                proc()
                sys.wait(50)
            else
                index_recv_buf = 2
                sys.wait(100)
            end
        else
            if buf_data1:len() > 0 then
                all_data = all_data..buf_data1
                buf_data1 = ''
                index_recv_buf = 1
                proc()
                sys.wait(50)
            else
                index_recv_buf = 1
                sys.wait(100)
            end
        end
    end
end)



--[[ ======= TEST FUNCTION BEGIN ======== ]]--

function test_init()
    op_4g.IP = '192.168.0.1' -- 此处只是示例，这里要填入公网IP
    op_4g.PORT = '11111'
    op_4g.init()
end


function test1()
    local a = op_data.data_pack('\x01','\x11\x22\x33\x44')
    op_4g.send(a)
end

function test2()
    local a = op_data.data_pack('\x02','\x12\x34\x56\x78')
    op_4g.send(a)
end

--[[ ======= TEST FUNCTION END ======== ]]--




