
module(..., package.seeall)


-- 添加CRC校验字
-- b: the bytes will add crc
function ConCRC(b)
    t1 = crypto.crc16("XMODEM",b)
    t2 = b..string.char(bit.rshift(t1,8))..string.char(bit.band(t1,0xff))
    return t2
end

-- CRC数据验证
-- b: the bytes will check crc
function DataCRC(b)
    log.info('DataCRC','b is '..b:toHex())
    t1 = crypto.crc16("XMODEM",b:sub(1,-3))
    t2 = string.char(bit.rshift(t1,8))..string.char(bit.band(t1,0xff))
    if b:sub(-2,-1) == t2 then
        return true
    else
        return false
    end
end



