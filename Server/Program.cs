using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OzToGLib; // converter component library
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Server
{
    class Program
    {
        private static readonly int PORT = 12;
        static void Main(string[] args)
        {
            IPAddress localAdress = IPAddress.Loopback;
            TcpListener serversocket = new TcpListener(localAdress, PORT);
            Console.WriteLine("IP Server runing on port: " + PORT);
            while (true)
            {
                serversocket.Start();
                TcpClient client = serversocket.AcceptTcpClient();
                Console.WriteLine($"Inocming client");

                Task clientTask = new Task(() => ProcessClient(client)); 
                clientTask.Start();

            }

        }

        private static void ProcessClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };
            while (true)
            {
                try
                {
                    string msg = reader.ReadLine();
                    string command = msg.Substring(0, msg.IndexOf(" "));
                    string value = msg.Substring(msg.IndexOf(" ") + 1);
                    string result = ConvertCommand(command, value);
                    writer.WriteLine($"Server response: {result}");
                    Console.WriteLine($"Server response: {result}");
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine($"Client has been disconected");
                    break;
                }
            }

        }
        private static string ConvertCommand(string command, string value)
        {
            string returned = "";
            command = command.ToUpper();
            if (Double.TryParse(value, out double input))
            {
                if (command == "TOGRAM")
                {
                    returned = OzToGLib.Convert.OzToG(input).ToString();
                }
                else if (command == "TOOUNCES")
                {
                    returned = OzToGLib.Convert.GToOz(input).ToString();
                }
                else
                {
                    returned = "Unexpected command";
                }
            }
            else
            {
                returned = "Unexpected value. Did you use dot instead of a coma?";
            }
            Console.WriteLine("Conversion handled");
            return returned;
        }
    }
}
