using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Servidor
{
    class Program
    {
        private const int MAX_BYTES = 1024;

        static void Main(string[] args)
        {
            // localhost 127.0.0.1
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ip_addr = host.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ip_addr, 11200);

            try
            {
                Socket listener = new Socket(ip_addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(10);

                Console.WriteLine("Esperando conexión...");

                Socket handler = listener.Accept();

                string msg_recvd = null;
                byte[] bytes_recvd = new byte[MAX_BYTES];
                int num_bytes_recvd;

                while(true)
                {
                    while (true)
                    {
                        num_bytes_recvd = handler.Receive(bytes_recvd);
                        msg_recvd = Encoding.ASCII.GetString(bytes_recvd, 0, num_bytes_recvd);

                        if (msg_recvd.IndexOf("<EOF>") > -1)
                            break;
                    }
                    string clean_data = msg_recvd[..^5];

                    global::System.Console.WriteLine("Cliente: " + clean_data);
                    byte[] response_bytes = Encoding.ASCII.GetBytes("Mensaje recibido.");
                    handler.Send(response_bytes);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }
    }
}
