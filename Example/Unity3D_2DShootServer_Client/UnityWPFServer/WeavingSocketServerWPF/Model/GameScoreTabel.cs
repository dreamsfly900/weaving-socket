using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class GameScoreTabel
    {

        [BsonId]
        public string Id { get; set; }

        public string userId { get; set; }
        public int score { get; set; }
        public int missenemy { get; set; }

    }
}
