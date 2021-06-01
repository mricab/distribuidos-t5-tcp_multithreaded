using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Threading;

namespace server
{
    public class SynchronousSocketReclaimer
    {
        private static ArrayList ClientSockets;
        private static bool ContinueReclaim = false;

        public SynchronousSocketReclaimer(ref ArrayList ClientList)
        {
            ClientSockets = ClientList;
        }

        public void StartReclaiming()
        {
            Console.WriteLine("Reclaimer up.");
            while (ContinueReclaim)
            {
                lock (ClientSockets.SyncRoot)
                {
                    for (int x = ClientSockets.Count - 1; x >= 0; x--)
                    {
                        Object Client = ClientSockets[x];
                        if (!((ClientHandler)Client).Alive)
                        {
                            ClientSockets.Remove(Client);
                            Console.WriteLine("Reclaimer: A client left. Thread closed.");
                        }
                    }
                }
                Thread.Sleep(2000); //miliseconds
            }
            Console.WriteLine("Not reclaiming anymore.");
        }

        public void Disable()
        {
            ContinueReclaim = false;
        }

        public void Enable()
        {
            ContinueReclaim = true;
        }
    }
}
