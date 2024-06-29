using System.Net.Sockets;
using System.Net;
using System.Text;

namespace NetworkApp_Client
{
    class Client
    {
        // Lesson 1
        static void Main(string[] args)
        {
            Console.Title = "Lan Test - Client";
            Console.Write("IP Address: ");
            string? address = Console.ReadLine();
            Console.Write("Port: ");
            int port = int.Parse(Console.ReadLine()!);


            // адрес и порт для подключения к серверу
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(address!), port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                Console.WriteLine("Start connection...");
                socket.Connect(serverEndPoint);
                if (socket.Connected)
                    Console.WriteLine("Connection established!");
                else
                    throw new Exception("Not connected!!!");

                Console.Write("Write your message: ");
                string? message = Console.ReadLine();
                byte[] buffer = Encoding.UTF8.GetBytes(message!);

                Console.WriteLine("Sending message");
                socket.Send(buffer);

                Console.WriteLine("Wait answer...");
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                buffer = new byte[256];
                do
                {
                    bytes = socket.Receive(buffer);
                    builder.Append(Encoding.UTF8.GetString(buffer, 0, bytes));
                } while (socket.Available > 0);

                Console.WriteLine("\nAccept answer: " + builder.ToString());

                socket.Shutdown(SocketShutdown.Both);

                socket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Press ENTER...");
            Console.ReadLine();
        }
    }
}
