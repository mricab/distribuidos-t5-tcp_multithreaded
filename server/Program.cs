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
        // Propieties
        private const int ListeningPort = 10116;
        private static ArrayList ClientSockets;
        private static SynchronousSocketListener SocketListener;
        private static SynchronousSocketReclaimer SocketReclaimer;

        // Threads
        // ThreadReclaim = new Thread(new ThreadStart(Reclaim));
        // ThreadReclaim.Start();


        public static int Main(String[] args)
        {
            // Initializations
            ClientSockets = new ArrayList();
            SocketReclaimer = new SynchronousSocketReclaimer(ref ClientSockets);
            SocketListener = new SynchronousSocketListener(ref ClientSockets);

            // Start Socket Reclaimer
            SocketReclaimer.Enable();
            Thread ThreadReclaim = new Thread(SocketReclaimer.StartReclaiming);
            ThreadReclaim.Start();

            // Start Socket Listener
            Thread ThreadListen = new Thread(SocketListener.StartListening);
            ThreadListen.Start(ListeningPort);
            ThreadListen.Join();   // Blocks main thread until ThreadListerner ends.

            // Stop Socket Reclaimer
            SocketReclaimer.Disable();
            ThreadReclaim.Join();  // Blocks main thread until ThreadReclaim ends.

            // Close all remaining connections.
            Console.WriteLine("Closing clients...");
            foreach (Object Client in ClientSockets)
            {
                ((ClientHandler)Client).Stop();
            }
            Console.WriteLine("Server stoped.");

            return 0;
        }
    }

}
