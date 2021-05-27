using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace client
{
    public class KeepAlive
    {

        static TcpClient ClientSocket;
        static bool continueProcess = false;

        public KeepAlive(TcpClient client)
        {
            ClientSocket = client;
        }

        public static void Start()
        {
            continueProcess = true;
            Thread ThreadKeepAlive = new Thread(new ThreadStart(Process));
            ThreadKeepAlive.Start();
        }

        public static void Process()
        {
            NetworkStream networkStream = ClientSocket.GetStream();

            double delay = 2000; //miliseconds
            DateTime lastTime = System.DateTime.Now;

            while (continueProcess)
            {
                if (System.DateTime.Now >= lastTime.AddMilliseconds(delay))
                {
                    try
                    {
                        Console.WriteLine("Sending keep message");
                        string DataToSend = "keep";
                        Byte[] sendBytes = Encoding.ASCII.GetBytes(DataToSend);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);

                        byte[] bytes = new byte[ClientSocket.ReceiveBufferSize];
                        int BytesRead = networkStream.Read(bytes, 0, (int)ClientSocket.ReceiveBufferSize);
                        string returndata = Encoding.ASCII.GetString(bytes, 0, BytesRead);
                    }
                    catch (IOException)
                    {
                        Console.WriteLine("Server timeout.");
                        break;
                    }

                    lastTime = lastTime.AddMilliseconds(delay);
                }
            }
            Console.WriteLine("Keep Alive process ended.");
        }

        public void Stop()
        {
            continueProcess = false;
        }

    }
}
