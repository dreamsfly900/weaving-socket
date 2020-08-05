using System;

namespace Weave.Base
{
    /// <summary>
    /// 数据帧头 定义????
    /// </summary>
    public class DataFrameHeader
    {
        public bool FIN { get; private set; }

        public bool RSV1 { get; private set; }

        public bool RSV2 { get; private set; }

        public bool RSV3 { get; private set; }

        public sbyte OpCode { get; private set; }

        public bool HasMask { get; private set; }

        public sbyte Length { get; private set; }

        public DataFrameHeader(byte[] buffer)
        {
            if (buffer.Length < 2)
                throw new Exception("无效的数据头.");
            //第一个字节
            FIN = (buffer[0] & 0x80) == 0x80;
            RSV1 = (buffer[0] & 0x40) == 0x40;
            RSV2 = (buffer[0] & 0x20) == 0x20;
            RSV3 = (buffer[0] & 0x10) == 0x10;
            OpCode = (sbyte)(buffer[0] & 0x0f);
            //第二个字节
            HasMask = (buffer[1] & 0x80) == 0x80;
            Length = (sbyte)(buffer[1] & 0x7f);
        }

        /// <summary>
        /// 发送封装数据
        /// </summary>
        /// <param name="fin"></param>
        /// <param name="rsv1"></param>
        /// <param name="rsv2"></param>
        /// <param name="rsv3"></param>
        /// <param name="opcode"></param>
        /// <param name="hasmask"></param>
        /// <param name="length"></param>
        public DataFrameHeader(bool fin, bool rsv1, bool rsv2, bool rsv3, sbyte opcode, bool hasmask, int length)
        {
            FIN = fin;
            RSV1 = rsv1;
            RSV2 = rsv2;
            RSV3 = rsv3;
            OpCode = opcode;
            //第二个字节
            HasMask = hasmask;
            Length = (sbyte)length;
        }

        /// <summary>
        /// 返回帧头字节
        /// </summary>
        /// <returns>返回帧头字节</returns>
        public byte[] GetBytes()
        {
            byte[] buffer = new byte[2] { 0, 0 };
            if (FIN) buffer[0] ^= 0x80;
            if (RSV1) buffer[0] ^= 0x40;
            if (RSV2) buffer[0] ^= 0x20;
            if (RSV3) buffer[0] ^= 0x10;
            buffer[0] ^= (byte)OpCode;
            if (HasMask) buffer[1] ^= 0x80;
            buffer[1] ^= (byte)Length;
            return buffer;
        }
    }
}
