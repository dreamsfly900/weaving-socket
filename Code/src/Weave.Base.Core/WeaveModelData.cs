namespace Weave.Base
{
    public class WeaveModelData
    {
        public string Request
        {
            get; set;
        }

        public string Type
        {
            get; set;
        }

        public bool Dtu
        {
            get; set;
        }

        public WeaveRequestDataDelegate Rd
        {
            get; set;
        }

        public WeaveRequestDataDtuDelegate Rd2
        {
            get; set;
        }
    }
}
