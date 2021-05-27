using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Threading;

namespace server
{
    public class SynchronousSocketListener
    {
        private static ArrayList ClientSockets;

        public SynchronousSocketListener(ref ArrayList ClientList)
        {
            ClientSockets = ClientList;
        }

        public void StartListening(object portNum)
        {
            int port = (int)portNum;

            TcpListener listener = new TcpListener(port);
            try
            {
                listener.Start();

                int TestingCycle = 3;
                int ClientNbr = 0;

                // Start listening for connections.
                Console.WriteLine("Listening for a connection...");
                while (TestingCycle > 0)
                {

                    TcpClient client = listener.AcceptTcpClient();

                    if (client != null)
                    {
                        Console.WriteLine("Client#{0} accepted!", ++ClientNbr);

                        // Processing Incoming clients.
                        lock (ClientSockets.SyncRoot)
                        {
                            int i = ClientSockets.Add(new ClientHandler(client));
                            ((ClientHandler)ClientSockets[i]).Start();
                        }
                        --TestingCycle;
                    }
                    else
                    {
                        break;
                    }
                }
                listener.Stop();

            }
            catch (Exception e)
            {
                Console.WriteLine("SocketListener: Can't accept conections.");
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nNot listening anymore.");

        }

    } // class SynchronousSocketListener
}
