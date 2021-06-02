using System;
using System.Text;
using System.Net.Sockets;

namespace client
{
    public class Client
    {
        private int ServerPort;
        private static TcpClient TcpClient;

        public Client(int ServerPort)
        {
            this.ServerPort = ServerPort;

            //Initializations
            TcpClient = new TcpClient();
        }

        public void Start()
        {

            try
            {
                // Conecting to server
                TcpClient.Connect("localhost", ServerPort);
                TcpClient.ReceiveTimeout = 2000; //miliseconds
                NetworkStream networkStream = TcpClient.GetStream();

                // Keep Alive Thread
                KeepAlive KeepAlive = new KeepAlive(TcpClient);
                KeepAlive.Start();

                if (networkStream.CanWrite && networkStream.CanRead)
                {

                    String DataToSend = "";

                    while (DataToSend != "quit")
                    {

                        Console.WriteLine("\nMessage> ");
                        DataToSend = Console.ReadLine();
                        if (DataToSend.Length == 0) break;

                        Byte[] sendBytes = Encoding.ASCII.GetBytes(DataToSend);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);

                        // Reads the NetworkStream into byte buffer.
                        byte[] bytes = new byte[TcpClient.ReceiveBufferSize];
                        int BytesRead = networkStream.Read(bytes, 0, (int)TcpClient.ReceiveBufferSize);

                        // Returns the data received from the host to the console.
                        string returndata = Encoding.ASCII.GetString(bytes, 0, BytesRead);
                        Console.WriteLine("Returned by host: \r\n{0}", returndata);
                    }
                    KeepAlive.Stop();
                    networkStream.Close();
                    TcpClient.Close();
                }
                else if (!networkStream.CanRead)
                {
                    Console.WriteLine("You can not write data to this stream");
                    TcpClient.Close();
                }
                else if (!networkStream.CanWrite)
                {
                    Console.WriteLine("You can not read data from this stream");
                    TcpClient.Close();
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Server not available!");
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("Server not available!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }
}
