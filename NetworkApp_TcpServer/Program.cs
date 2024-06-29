namespace NetworkApp_TcpServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Network Test - TcpListener (Server)";
            Console.Write("Enter port number: ");
            int port;
            port = int.Parse(Console.ReadLine()!);
            TcpServer tcpServer = new TcpServer(System.Net.IPAddress.Any, port);
            tcpServer.MessageString += TcpServer_MessageString;

            tcpServer.StartServerAsync();
            string cmd;
            do
            {
                cmd = Console.ReadLine()!;
                if (cmd.ToLower().Equals("list"))
                    Console.WriteLine(tcpServer.GetActiveConnections());

            } while (!cmd.ToLower().Equals("exit"));

            Console.WriteLine("press ENTER for exit...");
            Console.ReadLine();
        }

        private static void TcpServer_MessageString(string message)
        {
            Console.WriteLine(message);
        }
    }
}
