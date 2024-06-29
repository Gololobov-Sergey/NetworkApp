using System.Net.Sockets;
using System.Net;
using System.Text;

namespace NetworkApp_UDP
{
    class ProgramUdp
    {
        static int localPort;   // порт читання
        static int remotePort;  // порт відправки
        static IPAddress? remoteIp;
        static Socket? readSocket = null;

        // multiple clients
        static List<EndPoint> clients = new List<EndPoint>();

        static void Main(string[] args)
        {
            Console.Title = "Lan Test - UDP";
            Console.Write("Local port: ");
            localPort = int.Parse(Console.ReadLine()!);
            Console.Write("Remote port: ");
            remotePort = int.Parse(Console.ReadLine()!);
            Console.Write("Remote IP: ");
            remoteIp = IPAddress.Parse(Console.ReadLine()!);

            // one client
            //EndPoint remoteEndPoint = new IPEndPoint(remoteIp, remotePort);

            // multiple clients
            clients.Add(new IPEndPoint(remoteIp, remotePort));

            try
            {
                readSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                Task taskRead = Task.Factory.StartNew(ReadMessage);

                string message = string.Empty;
                byte[] buffer;
                Console.Write("Write your message: ");
                while (true)
                {
                    message = Console.ReadLine()!;
                    buffer = Encoding.UTF8.GetBytes(message);

                    // one client
                    //readSocket.SendTo(buffer, remoteEndPoint);

                    // multiple clients
                    foreach (EndPoint ep in clients)
                    {
                        readSocket.SendTo(buffer, ep);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                CloseConnection();
            }
        }

        static void ReadMessage()
        {
            try
            {
                IPEndPoint localEP = new IPEndPoint(IPAddress.Any, localPort);
                readSocket!.Bind(localEP);

                while (true)
                {
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    byte[] buffer = new byte[32];
                    EndPoint receiveEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    do
                    {
                        bytes = readSocket.ReceiveFrom(buffer, ref receiveEndPoint);
                        builder.Append(Encoding.UTF8.GetString(buffer, 0, bytes));
                    } while (readSocket.Available > 0);

                    // multiple clients
                    if (!clients.Contains(receiveEndPoint))
                        clients.Add(receiveEndPoint);

                    IPEndPoint? endPoint = receiveEndPoint as IPEndPoint;
                    Console.WriteLine($"Receive from {endPoint!.Address}:{endPoint.Port}, Message: " + builder.ToString());
                }
            }
            finally
            {
                CloseConnection();
            }
        }

        static void CloseConnection()
        {
            if (readSocket != null)
            {
                readSocket.Shutdown(SocketShutdown.Both);
                readSocket.Close();
                readSocket = null;
            }
        }
    }
}
