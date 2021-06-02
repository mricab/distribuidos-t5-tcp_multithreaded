using System;
using System.Threading;

namespace client
{
    class MainClass
    {

        private static int ServerPort;
        private static Client client;

        public static void Main(String[] args)
        {
            if (args.Length > 0 && GetPort(args[0], out ServerPort))
            {
                client = new Client(ServerPort);
                client.Start();
            }
            else
            {
                Console.WriteLine("Server port expected as first and only argument (Invalid port or no port supplied).");
            }

        } // Main()

        private static bool GetPort(String arg, out int Port)
        {
            ushort Aux;
            if (ushort.TryParse(arg, out Aux))
            {
                Port = Aux;
                if (Aux > 1023) // Well-known ports: 0 to 1023
                {
                    return true;
                }
                return false;
            }
            Port = 0;
            return false;
        }

    } //MainClass()
}
