using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class UserTabel
    {
        [BsonId]
        public string Id { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string logintime { get; set; }
    }
}
