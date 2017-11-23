using System.Collections.Generic;
namespace WeaveBase
{
    public class WeaveMenu
    {
        public string Id
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public List<WeaveMenu> Nodes
        {
            get; set;
        }
        public string Menu_ID { get; set; }
        public string Menu_Name { get; set; }
    }
}
