using System;
using System.Text;

namespace Weave.Base
{
    public enum FrameType : byte
    {
        Continuation,
        Text,
        Binary,
        Close = 8,
        Ping = 9,
        Pong = 10,
    }

    /// <summary>
    /// 数据帧 ？？类，定义了header，三个1位（ _extend，_mask，_content） byte 
    /// </summary>
    public class DataFrame
    {
        DataFrameHeader _header;
        private byte[] _extend = new byte[0];
        private byte[] _mask = new byte[0];
        private byte[] _content = new byte[0];

        public DataFrame()
        { }

        public byte[] GetData(byte[] buffer, ref byte[] masks, ref int lens, ref int payload_len, ref DataFrameHeader dfh)
        {
            var isFinal = (buffer[0] & 128) != 0;
            var reservedBits = (buffer[0] & 112);
            var frameType = (FrameType)(buffer[0] & 15);
            var isMasked = (buffer[1] & 128) != 0;
            var length = (buffer[1] & 127);
            if (!isMasked
                 || !Enum.IsDefined(typeof(FrameType), frameType)
                 || reservedBits != 0 //Must be zero per spec 5.2
                 || (frameType == FrameType.Continuation))
                return null;
            lens = 0;
            //帧头
            _header = new DataFrameHeader(buffer);
            dfh = _header;
            payload_len = 0;
            if (_header.OpCode != 2)
            {
            }
            //扩展长度
            if (_header.Length == 126)
            {
                lens = 8;
                _extend = new byte[2];
                Buffer.BlockCopy(buffer, 2, _extend, 0, 2);
            }
            else if (_header.Length == 127)
            {
                lens = 14;
                _extend = new byte[8];
                Buffer.BlockCopy(buffer, 2, _extend, 0, 8);
            }
            //是否有掩码
            if (_header.HasMask)
            {
                _mask = new byte[4];
                Buffer.BlockCopy(buffer, _extend.Length + 2, _mask, 0, 4);
            }
            //消息体
            if (_extend.Length == 0)
            {
               
                _content = new byte[_header.Length];
                payload_len = _content.Length;
                lens = _extend.Length + _mask.Length + 2;
                try
                {
                    Buffer.BlockCopy(buffer, _extend.Length + _mask.Length + 2, _content, 0, _content.Length);
                }
                catch
                { }
            }
            else if (_extend.Length == 2)
            {
                int contentLength = (int)_extend[0] * 256 + (int)_extend[1];
                payload_len = contentLength;
                lens = _extend.Length + _mask.Length + 2;
                _content = new byte[contentLength];
                Buffer.BlockCopy(buffer, _extend.Length + _mask.Length + 2, _content, 0, contentLength > 1024 * 100 ? 1024 * 100 : contentLength);
            }
            else
            {
                long len = 0;
                int n = 1;
                for (int i = 7; i >= 0; i--)
                {
                    len += (int)_extend[i] * n;
                    n *= 256;
                }
                payload_len = (int)len;
                _content = new byte[len];
                lens = _extend.Length + _mask.Length + 2;
                Buffer.BlockCopy(buffer, _extend.Length + _mask.Length + 2, _content, 0, _content.Length);
            }
            if (_header.HasMask) _content = Mask(_content, _mask);
            return _content;
        }

        public void SetByte(byte[] contents)
        {
            _content = contents;
            int length = _content.Length;
            if (length < 126)
            {
                _extend = new byte[0];
                _header = new DataFrameHeader(true, false, false, false, 2, false, length);
            }
            else if (length < 65536)
            {
                _extend = new byte[2];
                _header = new DataFrameHeader(true, false, false, false, 2, false, 126);
                _extend[0] = (byte)(length / 256);
                _extend[1] = (byte)(length % 256);
            }
            else
            {
                _extend = new byte[8];
                _header = new DataFrameHeader(true, false, false, false, 2, false, 127);
                int left = length;
                int unit = 256;
                for (int i = 7; i > 1; i--)
                {
                    _extend[i] = (byte)(left % unit);
                    left = left / unit;
                    if (left == 0)
                        break;
                }
            }
        }

        public byte[] GetBytes()
        {
            byte[] buffer = new byte[2 + _extend.Length + _mask.Length + _content.Length];
            Buffer.BlockCopy(_header.GetBytes(), 0, buffer, 0, 2);
            Buffer.BlockCopy(_extend, 0, buffer, 2, _extend.Length);
            Buffer.BlockCopy(_mask, 0, buffer, 2 + _extend.Length, _mask.Length);
            Buffer.BlockCopy(_content, 0, buffer, 2 + _extend.Length + _mask.Length, _content.Length);
            return buffer;
        }

        public string Text
        {
            get
            {
                if (_header.OpCode != 1)
                    return string.Empty;
                return Encoding.UTF8.GetString(_content);
            }
        }

        private byte[] Mask(byte[] data, byte[] mask)
        {
            for (var i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] ^ mask[i % 4]);
            }
            return data;
        }
    }
}
