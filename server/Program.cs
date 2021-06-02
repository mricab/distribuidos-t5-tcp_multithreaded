using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Threading;

namespace server
{

    class MainClass
    {
        // Propierties
        private static int ListeningPort;
        private static Server server;

        public static void Main(String[] args)
        {
            if (args.Length>0 && GetPort(args[0], out ListeningPort))
            {
                server = new Server(ListeningPort);
                server.Start();
                Menu();
            }
            else
            {
                Console.WriteLine("Server port expected as first and only argument (Invalid port or no port supplied).");
            }
        }

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

        private static void Menu()
        {
            while(true)
            {
                String answer;
                Console.Write("\nStop server (Y/N)? ");
                answer = Console.ReadLine();
                if(answer == "Y")
                {
                    server.Stop();
                    break;
                }
            }

        }
    }

}
