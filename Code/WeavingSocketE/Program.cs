namespace WeavingSocketE
{
    internal class Program
    {
        static void Main(string[] args)
        {
            String path = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine("WeavingSocket 物联网通信架构");
            if (args.Length > 0)
            {
                if (args[0] == "v")
                {
                    Console.WriteLine("v:2.0.22");
                }
            }
        }
    }
}