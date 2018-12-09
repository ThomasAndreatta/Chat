using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;         // da aggiungere
using System.Net.Sockets; // da aggiungere
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoChatForm.Class
{   
    class riceviUDP
    {
       static byte[] data; 
       static IPEndPoint ipep;
       static UdpClient newsock;
       static IPEndPoint sender;
        public string ip = "";
        public  riceviUDP(int porta)
        {
            data = new byte[1024];
            ipep = new IPEndPoint(IPAddress.Any, porta);
            newsock = new UdpClient(ipep);

            string[] tmp = GetLocalIPAddress().Split('.');
            if (tmp[1].ToString() == "0")
            {

            }
            else
            {
                tmp[3] = "255";
                for (int i = 0; i < tmp.Length; i++)
                {
                    if (i < 3)
                    {
                        ip += tmp[i] + ".";
                    }
                    else
                        ip += tmp[i];
                }
                sender = new IPEndPoint(IPAddress.Parse(ip), 0);
            }
        }

        private string GetLocalIPAddress()
        {
            
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            
            return "0.0.0.0";
        }
        public string ricevi()
        {
                data = newsock.Receive(ref sender);
                return Encoding.ASCII.GetString(data, 0, data.Length);
        }
    }
}
