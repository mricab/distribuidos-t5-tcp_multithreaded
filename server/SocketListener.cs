using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Threading;

namespace server
{
    public interface ISynchronousSocketListener
    {
        event EventHandler<ClientAcceptedEventArgs> ClientAccepted; //Event and Delegate
    }

    public class ClientAcceptedEventArgs : EventArgs
    {
        public TcpClient Client { get; set; }
    }


    public class SynchronousSocketListener : ISynchronousSocketListener
    {
        private static ArrayList ClientSockets;
        private static bool ContinueListen;
        private static TcpListener Listener;
        public event EventHandler<ClientAcceptedEventArgs> ClientAccepted;

        public SynchronousSocketListener(ref ArrayList ClientList)
        {
            ClientSockets = ClientList;
        }

        public void StartListening(object portNum)
        {
            int port = (int)portNum;

            Listener = new TcpListener(IPAddress.Loopback, port);
            try
            {
                Listener.Start();
                int ClientNbr = 0;

                // Start listening for connections.
                Console.WriteLine("Listening for connections...");
                while (ContinueListen)
                {
                    TcpClient client = Listener.AcceptTcpClient();

                    if (client != null)
                    {
                        // Processing Incoming clients.
                        Console.WriteLine("Client#{0} accepted!", ++ClientNbr);
                        OnClientAccepted(client);
                    }
                    else
                    {
                        break;
                    }
                }
                Listener.Stop();

            }
            catch (SocketException)
            {
                Console.WriteLine("Listener interrupted!");
            }
            catch (Exception e)
            {
                Console.WriteLine("SocketListener: Can't accept conections.");
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine("\nNot listening anymore.");

        }

        public void Disable()
        {
            ContinueListen = false;
            Listener.Stop();
        }

        public void Enable()
        {
            ContinueListen = true;
        }

        protected virtual void OnClientAccepted(TcpClient clientSocket) //Event Raiser
        {
            if (ClientAccepted != null)
            {
                ClientAccepted(this, new ClientAcceptedEventArgs() { Client = clientSocket });
            }
        }

    } // class SynchronousSocketListener
}
