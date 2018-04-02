using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace smsForCsharp.CRC
{
    /// <summary>   
    /// 消息CRC校验算法   
    /// </summary>   
    public class CRC
    {
        /// <summary>

        public static byte [] CRCCalc(byte[] crcbuf)
        {
            //string[] datas = data.Split(' ');
            //List<byte> bytedata = new List<byte>();

            //foreach (string str in datas)
            //{
            //    bytedata.Add(byte.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier));
            //}
           // byte[] crcbuf = bytedata.ToArray();
            //计算并填写CRC校验码
            int crc = 0xffff;
            int len = crcbuf.Length;
            for (int n = 0; n < len; n++)
            {
                byte i;
                crc = crc ^ crcbuf[n];
                for (i = 0; i < 8; i++)
                {
                    int TT;
                    TT = crc & 1;
                    crc = crc >> 1;
                    crc = crc & 0x7fff;
                    if (TT == 1)
                    {
                        crc = crc ^ 0xa001;
                    }
                    crc = crc & 0xffff;
                }

            }
            byte[] redata = new byte[2];
            redata[1] = (byte)((crc >> 8) & 0xff);
            redata[0] = (byte)((crc & 0xff));
            return redata;
        }
    }



 
}