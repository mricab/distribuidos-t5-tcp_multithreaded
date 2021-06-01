using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace client
{
    class MainClass
    {

        private const int portNum = 10116;

        static public void Main()
        {

            TcpClient tcpClient = new TcpClient();
            KeepAlive keepAlive;

            try
            {
                // Conecting to server
                tcpClient.Connect("localhost", portNum);
                tcpClient.ReceiveTimeout = 2000; //miliseconds
                NetworkStream networkStream = tcpClient.GetStream();

                // Keep Alive Thread
                keepAlive = new KeepAlive(tcpClient);
                KeepAlive.Start();

                if (networkStream.CanWrite && networkStream.CanRead)
                {

                    String DataToSend = "";

                    while (DataToSend != "quit")
                    {

                        Console.WriteLine("\nMessage:");
                        DataToSend = Console.ReadLine();
                        if (DataToSend.Length == 0) break;

                        Byte[] sendBytes = Encoding.ASCII.GetBytes(DataToSend);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);

                        // Reads the NetworkStream into byte buffer.
                        byte[] bytes = new byte[tcpClient.ReceiveBufferSize];
                        int BytesRead = networkStream.Read(bytes, 0, (int)tcpClient.ReceiveBufferSize);

                        // Returns the data received from the host to the console.
                        string returndata = Encoding.ASCII.GetString(bytes, 0, BytesRead);
                        Console.WriteLine("Returned by host: \r\n{0}", returndata);
                    }
                    KeepAlive.Stop();
                    networkStream.Close();
                    tcpClient.Close();
                }
                else if (!networkStream.CanRead)
                {
                    Console.WriteLine("You can not write data to this stream");
                    tcpClient.Close();
                }
                else if (!networkStream.CanWrite)
                {
                    Console.WriteLine("You can not read data from this stream");
                    tcpClient.Close();
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
        } // Main()

    } //MainClass()
}
