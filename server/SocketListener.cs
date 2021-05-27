using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Threading;

namespace server
{
    public class ClientAcceptedEventArgs : EventArgs
    {
        public TcpClient Client { get; set; }
    }


    public class SynchronousSocketListener
    {
        private static ArrayList ClientSockets;

        public delegate void ClientAcceptedEventHandler(object source, ClientAcceptedEventArgs args);  //Delegate

        public event ClientAcceptedEventHandler ClientAccepted;  // Event

        protected virtual void OnClientAccepted(TcpClient clientSocket) //Event Raiser
        {
            if(ClientAccepted != null)
            {
                ClientAccepted(this, new ClientAcceptedEventArgs() { Client = clientSocket });
            }
        }


        public SynchronousSocketListener(ref ArrayList ClientList)
        {
            ClientSockets = ClientList;
        }

        public void StartListening(object portNum)
        {
            int port = (int)portNum;

            TcpListener listener = new TcpListener(IPAddress.Loopback, port);
            try
            {
                listener.Start();

                int TestingCycle = 3;
                int ClientNbr = 0;

                // Start listening for connections.
                Console.WriteLine("Listening for connections...");
                while (TestingCycle > 0)
                {

                    TcpClient client = listener.AcceptTcpClient();

                    if (client != null)
                    {
                        Console.WriteLine("Client#{0} accepted!", ++ClientNbr);

                        // Processing Incoming clients.
                        OnClientAccepted(client);
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
