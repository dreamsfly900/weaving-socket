using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace WeaveBase
{
    /// <summary>
    /// 0x00
    /// </summary>
    public class WeaveUser
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        public string Passworld { get; set; }
        /// <summary>
        /// 回话ID
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get; set; }
    }
}
