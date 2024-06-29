using System;
using System.Windows.Forms;
using System.Net;

namespace NetworkApp_TcpClient
{
    public partial class MainForm : Form
    {
        UserConnection client;
        public MainForm()
        {
            client = null;
            InitializeComponent();
            FormClosing += MainForm_FormClosing;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if(client != null)
                {
                    client.Disconnect();
                }
            }
            catch { }
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                btnConnect.Enabled = false;
                client = new UserConnection(edUsername.Text, IPAddress.Parse(edAddress.Text), (int)edPort.Value);
                client.ConnectionEstablished += Client_ConnectionEstablished;
                client.IncomingMessage += Client_IncomingMessage;
                await client.ConnectAsync();
                await client.ReadMessageAsync();
            }
            catch (Exception ex)
            {
                client = null;
                btnConnect.Enabled = true;
                lsbMessages.Items.Add("ERROR: " + ex.Message);
            }
        }

        private void Client_IncomingMessage(string message)
        {
            Action action = () => 
            { 
                lsbMessages.Items.Add(message); 
            };
            Invoke(action);
        }

        private void Client_ConnectionEstablished(bool IsConnected)
        {
            Action action = () =>
            {
                if (IsConnected)
                {
                    btnConnect.Enabled = false;
                    btnDisconnect.Enabled = true;
                    edAddress.Enabled = false;
                    edPort.Enabled = false;
                    edUsername.Enabled = false;
                    grSendMessage.Enabled = true;
                }
                else
                {
                    btnConnect.Enabled = true;
                    btnDisconnect.Enabled = false;
                    edAddress.Enabled = true;
                    edPort.Enabled = true;
                    edUsername.Enabled = true;
                    grSendMessage.Enabled = false;
                    lsbMessages.Items.Add("Disconnected!!!");
                }
            };

            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }

        private async void btnSendMessage_Click(object sender, EventArgs e)
        {
            if ((client == null) || !client.Connected
                || string.IsNullOrEmpty(edMessage.Text))
                return;

            btnSendMessage.Enabled = false;
            await client.SendMessageAsync(edMessage.Text);
            edMessage.Text = "";
            edMessage.Focus();
            
            await client.ReadMessageAsync();
            btnSendMessage.Enabled = true;
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (client!=null && client.Connected)
            {
                try
                {
                    client.Disconnect();
                }
                catch { }
            }
        }
    }
}
