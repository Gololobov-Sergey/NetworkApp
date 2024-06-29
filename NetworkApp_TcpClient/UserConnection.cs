using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace NetworkApp_TcpClient
{
    public class UserConnection : IDisposable
    {
        TcpClient client;
        IPAddress remoteAddress;
        int remotePort;
        string name;
        public string Name {
            get => name;
            set
            {
                if (!name.Equals(value))
                {
                    name = value;
                }
            }
        }

        public bool Connected => (client != null && client.Connected);

        public event Action<bool> ConnectionEstablished;
        public event Action<string> IncomingMessage;
        public event Action<int> SendComplete;

        public UserConnection(string name, IPAddress address, int port)
        {
            client = null;
            client = new TcpClient();
            remoteAddress = address;
            remotePort = port;
            this.name = name;
        }
        public override string ToString()
        {
            return Name;
        }
        public async Task ConnectAsync() => await Task.Factory.StartNew(Connect);

        public void Connect()
        {
            if (client != null && client.Connected)
                return;

            try
            {
                client.Connect(remoteAddress, remotePort);
                ConnectionEstablished?.Invoke(true);
            }
            catch (Exception)
            {
                client = null;
                ConnectionEstablished?.Invoke(false);
            }
        }

        public void Disconnect()
        {
            try
            {
                if (client != null && client.Connected)
                {
                    SendMessage("exit");
                    client.Close();
                }
            }
            finally
            {
                client = null;
                ConnectionEstablished?.Invoke(false);
            }
        }

        public void SendMessage(string message)
        {
            if (client == null && !client.Connected)
                return;

            NetworkStream ns = client.GetStream();
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                ns.Write(buffer, 0, buffer.Length);
                SendComplete?.Invoke(buffer.Length);
            }
            catch 
            { 
                SendComplete?.Invoke(-1);
            }
        }

        public async Task SendMessageAsync(string message)=> await Task.Run(()=> SendMessage(message));

        public void ReadMessage()
        {
            if (client == null || !client.Connected)
                return;

            NetworkStream ns = client.GetStream();
            int bytes;
            byte[] buffer = new byte[256];
            StringBuilder sb = new StringBuilder();

            do
            {
                bytes = ns.Read(buffer, 0, buffer.Length);
                sb.Append(Encoding.UTF8.GetString(buffer, 0, bytes));
            } while (ns.DataAvailable);
            IncomingMessage?.Invoke(sb.ToString());
        }

        public async Task ReadMessageAsync() => await Task.Run(ReadMessage);

        public void Dispose()
        {
            try
            {
                Disconnect();
            }
            catch { }
        }
    }
}
