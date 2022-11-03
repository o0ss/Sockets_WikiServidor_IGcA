using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Servidor
{
    class Program
    {
        private const int MAX_BYTES = 10240;

        static void Python_wiki_script(string qry)
        {
            Process proc = new();
            proc.StartInfo = new ProcessStartInfo()
            {
                FileName = "python",
                Arguments = string.Format("{0} {1}", "getwiki.py", qry),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = false
            };
            proc.Start();
            string proc_out = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            Console.WriteLine("Python output: " + proc_out);
            return;
        }

        static void Main(string[] args)
        {
            System.Console.Title = "Servidor";
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

                string msg_recvd, wiki_out;
                byte[] bytes_recvd = new byte[MAX_BYTES], response_bytes = new byte[MAX_BYTES];
                int num_bytes_recvd;

                while (true)
                {
                    while (true)
                    {
                        num_bytes_recvd = handler.Receive(bytes_recvd);
                        msg_recvd = Encoding.ASCII.GetString(bytes_recvd, 0, num_bytes_recvd);

                        if (msg_recvd.IndexOf("<EOF>") > -1)
                            break;
                    }
                    string clean_query = msg_recvd[..^5];

                    global::System.Console.WriteLine("Cliente: " + clean_query);

                    if( clean_query != "exit")
                    {
                        Python_wiki_script(clean_query);

                        // read wiki output from wiki_out.temp into string
                        wiki_out = System.IO.File.ReadAllText("wiki_out.tmp");
                        response_bytes = Encoding.UTF8.GetBytes(wiki_out);
                        handler.Send(response_bytes);
                    }
                    else
                    {
                        response_bytes = Encoding.UTF8.GetBytes("Bye!");
                        handler.Send(response_bytes);
                        break;
                    }
                    
                }
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }
    }
}
