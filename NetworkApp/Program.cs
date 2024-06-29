using System.Net.Sockets;
using System.Net;
using System.Text;

namespace NetworkApp_Server
{
    class Server
    {
        static void Main(string[] args)
        {
            Console.Title = "Network Test - Server";

            StartServer();

            Console.WriteLine("press any key...");
            Console.ReadKey();

            Console.WriteLine("Wait stop server...");
        }

        static void StartServer()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1000);
            //IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 1000);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 1000);
            socket.Bind(endPoint);
            socket.Listen(10);
            Console.WriteLine("Wait new connection...");
            socket.BeginAccept(AcceptCallback, socket);
        }

        static void AcceptCallback(IAsyncResult result)
        {
            Socket? server = result.AsyncState as Socket;
            if (server == null)
                throw new Exception("Unknow result socket");

            Socket client = server.EndAccept(result);
            server.BeginAccept(AcceptCallback, server);

            Console.WriteLine($"Accept connection: {client.RemoteEndPoint} at {DateTime.Now.ToLongTimeString()}");

            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            byte[] buffer = new byte[256];
            do
            {
                bytes = client.Receive(buffer);
                builder.Append(Encoding.UTF8.GetString(buffer, 0, bytes));
            } while (client.Available > 0);

            Console.WriteLine(builder.ToString());

            builder.Clear();
            builder.Append($"You message read at {DateTime.Now.ToLongTimeString()}");
            buffer = Encoding.UTF8.GetBytes(builder.ToString());
            client.Send(buffer);

            client.Shutdown(SocketShutdown.Both);
            client.Close();

        }
    }
}
