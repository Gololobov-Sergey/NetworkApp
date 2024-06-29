using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class Program
{

    static int RemotePort;
    static int LocalPort;
    static IPAddress RemoteIPAddr;

    [STAThread]
    static void Main(string[] args)
    {
        try
        {
            Console.SetWindowSize(40, 20);
            Console.Title = "Chat";
            Console.WriteLine("enter remote IP");
            RemoteIPAddr = IPAddress.Parse(Console.ReadLine());
            Console.WriteLine("enter remote port");
            RemotePort = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("enter local port");
            LocalPort = Convert.ToInt32(Console.ReadLine());

            Thread thread = new Thread(
                   new ThreadStart(ThreadFuncReceive)
            );

            thread.IsBackground = true;

            thread.Start();
            Console.ForegroundColor = ConsoleColor.Red;
            while (true)
            {
                SendData(Console.ReadLine());
            }
        }
        catch (FormatException formExc)
        {
            Console.WriteLine("Conversion impossible :" + formExc);
        }
        catch (Exception exc)
        {
            Console.WriteLine("Exception : " + exc.Message);
        }
    }

    static void ThreadFuncReceive()
    {
        try
        {
            while (true)
            {

                UdpClient uClient = new UdpClient(LocalPort);
                IPEndPoint ipEnd = null;

                byte[] responce = uClient.Receive(ref ipEnd);

                string strResult = Encoding.Unicode.GetString(responce);
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine(strResult);
                Console.ForegroundColor = ConsoleColor.Red;
                uClient.Close();
            }
        }
        catch (SocketException sockEx)
        {
            Console.WriteLine("Socket exception: " + sockEx.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception : " + ex.Message);
        }
    }

    static void SendData(string datagramm)
    {
        UdpClient uClient = new UdpClient();

        IPEndPoint ipEnd = new IPEndPoint(RemoteIPAddr, RemotePort);
        try
        {
            byte[] bytes = Encoding.Unicode.GetBytes(datagramm);
            uClient.Send(bytes, bytes.Length, ipEnd);
        }
        catch (SocketException sockEx)
        {
            Console.WriteLine("Socket exception: " + sockEx.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception : " + ex.Message);
        }
        finally
        {
            //close the UdpClient class instance
            uClient.Close();
        }
    }
}

