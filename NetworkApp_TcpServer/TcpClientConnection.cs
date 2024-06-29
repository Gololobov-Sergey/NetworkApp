using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkApp_TcpServer
{
    public class TcpClientConnection
    {
        static int count = 0;

        TcpClient client;

        public event Action<TcpClientConnection, string>? IncomingMessage;

        public event Action<TcpClientConnection>? Disconnect;
        public int Id { get; private set; }
        public TcpClientConnection(TcpClient client)
        {
            Id = ++count;
            this.client = client;
        }
        public override string ToString()
        {
            return $"Connection {Id}";
        }
        public Task DoWorkAsync() => Task.Run(DoWork);

        public void DoWork()
        {
            if (client == null || !client.Connected)
                return;

            NetworkStream ns = client.GetStream();

            byte[] buffer = Encoding.UTF8.GetBytes("Hello, current time " + DateTime.Now.ToLongTimeString());
            ns.Write(buffer, 0, buffer.Length);

            buffer = new byte[256];
            int bytes;
            StringBuilder sb = new StringBuilder();
            try
            {
                while (true)
                {
                    do
                    {
                        bytes = ns.Read(buffer, 0, buffer.Length);
                        sb.Append(Encoding.UTF8.GetString(buffer, 0, bytes));
                    } while (ns.DataAvailable);
                    IncomingMessage?.Invoke(this, sb.ToString());

                    if ("exit".CompareTo(sb.ToString().ToLower()) == 0)
                        break;
                    else
                    {
                        buffer = Encoding.UTF8.GetBytes("Message delivered");
                        ns.Write(buffer, 0, buffer.Length);
                    }
                    sb.Clear();
                }
            }
            catch { }
            try { client.Close(); } catch { }
            Disconnect?.Invoke(this);
        }
    }
}
