using System;
using System.Collections;
using System.Threading;


namespace server
{
    public class Server
    {
        private int ListeningPort;
        private static ArrayList ClientSockets;
        private static SynchronousSocketListener SocketListener;
        private static SynchronousSocketReclaimer SocketReclaimer;
        private static Thread ThreadListen;
        private static Thread ThreadReclaim;

        public Server(int ListeningPort)
        {
            this.ListeningPort = ListeningPort;
            
            // Initializations
            ClientSockets = new ArrayList();
            SocketReclaimer = new SynchronousSocketReclaimer(ref ClientSockets);
            SocketListener = new SynchronousSocketListener(ref ClientSockets);
        }

        public void Start()
        {
            // Opening message
            Console.WriteLine("Starting server on port {0}.", ListeningPort);

            // Events subscriptions
            SocketListener.ClientAccepted += OnClientAccepted;

            // Start Socket Reclaimer
            SocketReclaimer.Enable();
            ThreadReclaim = new Thread(SocketReclaimer.StartReclaiming);
            ThreadReclaim.Start();

            // Start Socket Listener
            SocketListener.Enable();
            ThreadListen = new Thread(SocketListener.StartListening);
            ThreadListen.Start(ListeningPort);

            // Closing message
            Thread.Sleep(5000);
            Console.WriteLine("Server running.");
        }

        public void Stop()
        {
            // Oppening message
            Console.WriteLine("Stopping server...");

            // Stopping Socket Listener
            SocketListener.Disable();
            ThreadListen.Join(); // Blocks this thread until ThreadListen ends.

            // Stopping Socket Reclaimer
            SocketReclaimer.Disable();
            ThreadReclaim.Join(); // Blocks this thread until ThreadReclaim ends.

            // Closing all remaining connections
            CloseClients();

            // Closing message
            Console.WriteLine("Server stopped.");
        }

        public void Restart()
        {
            Console.WriteLine("Restarting server...");
            Console.WriteLine("Server running.");
        }

        public void Pause()
        {
            Console.WriteLine("Pausing server...");
            Console.WriteLine("Server paused.");
        }

        private static void CloseClients()
        {
            Console.WriteLine("Closing clients...");
            foreach (Object Client in ClientSockets)
            {
                ((ClientHandler)Client).Stop();
            }
        }

        private static void OnClientAccepted(object source, ClientAcceptedEventArgs e)
        {
            lock (ClientSockets.SyncRoot)
            {
                int i = ClientSockets.Add(new ClientHandler(e.Client));
                ((ClientHandler)ClientSockets[i]).Start();
            }
        }
    }
}
