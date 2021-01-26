using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestMessaging
{
    class MainClass
    {

        private static int port = 100;

        private static TcpListener server;

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            while (true)
            {

                await SetupServerAsync();
            }
        }


        private static async Task SetupServerAsync()
        {
            Console.Write("Starting server.." + Environment.NewLine);
            server = new TcpListener(IPAddress.Any, port);

            while (true)
            {
                server.Start();
                Console.Write("New server listener socket opened, listening for incoming connections.." + Environment.NewLine);
                Socket socket = await server.AcceptSocketAsync();

                //send initial message to client, to identify etc
                byte[] connectionMessage = Encoding.Default.GetBytes("You are now connected to the messaging server, at: " + DateTime.Now.ToString("HH:mm:ss:fff"));
                socket.Send(connectionMessage);

                //update ui
                Console.Write("Connection made, reading data.." + Environment.NewLine);


                //set timeout
                //socket.ReceiveTimeout = 30000;

                byte[] b = new byte[10000];

                //initial read
                int k = socket.Receive(b);

                while (k != 0)
                {
                    string data = Encoding.Default.GetString(b, 0, k);
                    Console.Write(data + Environment.NewLine);

                    //when we have read a packet, respond
                    byte[] responseMessageBytes = Encoding.Default.GetBytes("Hey there!");
                    socket.Send(responseMessageBytes);

                    //re-read while data is available
                    k = socket.Receive(b);
                }

                socket.Close();
                Console.Write("Server listener socket closed.." + Environment.NewLine);
            }

        }

    }
}
