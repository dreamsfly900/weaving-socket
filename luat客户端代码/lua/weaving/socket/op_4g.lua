




--[[
net.switchFly(true)     --进入飞行模式
net.switchFly(false)    --退出飞行模式
]]--





module(..., package.seeall)
require "sim"
require "net"
require "socket"
require "op_uart1"
require "op_fs"
require "op_data"

IP = ''
PORT = ''

local flag_4g_conn = false
local asyncClient
local clientConnected

-- sys.restart 后临时变量会重置

function switch_sim()
    sim.setId(sim.getId()==0 and 1 or 0, function(result)
        if result then
            --log.info("-------------------------sim id is ",sim.getId())
            op_uart1.save_at_cfg()
            sys.restart("simcross")
        end
    end)
end

function sim_set(sim_id)
    sim.setId(tonumber(sim_id)==0 and 0 or 1, function(result)
        if result then
            --log.info("-------------------------sim id is ",sim.getId())
            op_uart1.save_at_cfg()
            sys.restart("simcross")
        end
    end)
end


-- @string ip,IP地址
-- @string port,端口号
-- @number keepAlive,服务器和客户端最大通信间隔时间,也叫心跳包最大时间,单位秒
-- @string pingreq,心跳包的字符串
function cfg(ip,port,keepAlive, pingreq)
    -- todo
end

function disconn()
    flag_4g_conn = false
    send('') -- 触发一下 asyncClient:asyncSelect，使其判断到 flag_4g_conn = false 从而断开连接
end


-- @string ip,IP地址
-- @string port,端口号
-- @function cb, callback function，用于处理接收到的数据，该函数定义在 op_uart1.lua 中
-- cb 目前强制为 op_uart1.writeNN，故不需赋值
function conn(ip,port,cb)
    flag_4g_conn = true
    sys.taskInit(function()
        while flag_4g_conn do
            while not socket.isReady() do sys.wait(1000) end
            asyncClient = socket.tcp()
            while not asyncClient:connect(ip, port) do sys.wait(2000) end
            clientConnected = true
            while asyncClient:asyncSelect(0, "ping") do
                if not flag_4g_conn then break end
            end
            log.info('op_4g.conn','disconn')
            clientConnected = false
            asyncClient:close()
        end
    end)

    sys.subscribe("SOCKET_RECV", function(id)
        if asyncClient.id == id then
            local data = asyncClient:asyncRecv()
            if #data > 0 then
                op_data.recv_to_buf(data)
            end
        end
    end)
end

function send(s)
    if clientConnected then
        asyncClient:asyncSend(s)
        log.info('op_4g','send data:'..s:toHex())
    end
end



function init()
    op_4g.conn(IP,PORT)
end


