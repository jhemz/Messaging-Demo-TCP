using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Messageing_Client
{
    class MainClass
    {
        private static TcpClient client;
        private static NetworkStream stream;
        private static bool connected;
        private static Timer timer;
        private static int port = 100;


        public static async Task Main(string[] args)
        {
            Write("Messaging Client", ConsoleColor.Yellow);

            while (true)
            {
                while (!connected)
                {
                    string command = Console.ReadLine();

                    switch (command)
                    {
                        case "connect":
                            await Connect();
                            break;
                        default:

                            Write("Unknown command", ConsoleColor.Magenta);
                            break;
                    }
                }

                while (connected)
                {
                    string command = Console.ReadLine();

                    switch (command)
                    {
                        case "disconnect":
                            Disconnect();
                            break;
                        case "message":
                            Message("Test");
                            break;
                        case "poll":
                            SendUpdates();
                            break;
                        default:
                            Write("Unknown command", ConsoleColor.Magenta);
                            break;
                    }
                }
            }
        }


        private static void SendUpdates()
        {
            // Create a timer with a 0.1 second interval.
            timer = new Timer(10);
            // Hook up the Elapsed event for the timer. 
            timer.Elapsed += SendUpdate;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private static void SendUpdate(object sender, ElapsedEventArgs e)
        {
            Message(DateTime.Now.ToString("HH:mm:ss:fff"));
        }

        private static void Message(string message)
        {
            if (client != null)
            {
                //convert string message to byte array
                byte[] messageBytes = Encoding.Default.GetBytes(message);

                //write to the listener
                stream.Write(messageBytes, 0, messageBytes.Length);

                Write("Message Sent: " + message, ConsoleColor.White);

                byte[] b = new byte[10000];

                //read reply
                int k = stream.Read(b, 0, 10000); //code will hang here waiting for a reply, we need a reply from the server to continue, so we know whats going on
                
                string data = Encoding.Default.GetString(b, 0, k);

                Write("Reply from server: " + data, ConsoleColor.Cyan);
            }
        }


        private static void Disconnect()
        {
            client.Close();
            Write("Disconnected", ConsoleColor.Red);

            //set class variable to false for the puposes of the switch statement in 'Main'
            connected = false;
        }





        private static async Task Connect()
        {
            //create client
            client = new TcpClient();

            //connect
            await client.ConnectAsync(GetLocalIP(), port);

            //update ui
            Write("Connected", ConsoleColor.Green);

            //initialise the network stream upon connection
            stream = client.GetStream();

            //set buffer to read into
            byte[] b = new byte[10000];

            //read the connection message sent by the server into the buffer, we wont set a timout, as we know the server will send something
            int k = stream.Read(b, 0, 10000);//k is the length of the message
            string data = Encoding.Default.GetString(b, 0, k);//decode the byte array of length k into a human readable ascii string

            Write("Reply from server: " + data, ConsoleColor.Cyan);

            //set class variable to true for the puposes of the switch statement in 'Main'
            connected = true;

        }





        private static void Write(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static string GetLocalIP()
        {
            //get local ip address
            List<string> IPAddresses = new List<string>();
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] ip = host.AddressList;

            for (int j = 0; j < ip.Length; j++)
            {
                if (ip[j].ToString().Contains("."))
                {
                    IPAddresses.Add(ip[j].ToString());
                }
            }

            return IPAddresses.First();
        }


    }
}
