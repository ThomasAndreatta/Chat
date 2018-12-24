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
    class InvioUDP
    {
        static byte[] data;       
        static UdpClient server;
        private string ipServer = "";
        private string username = " ";
        private string ip = " ";

        public string IpServer { get => ipServer;  }
        public string Username { get => username;  }
        public string Ip { get => ip;  }

        public InvioUDP()
        {
            data = new byte[1024];
            // invio in broadcast
            ip = GetLocalIPAddress();
            string[] tmp = GetLocalIPAddress().Split('.');
            tmp[3] = "255";
            for (int i = 0; i < tmp.Length; i++)
            {
                if (i<3)
                {
                    ipServer += tmp[i] + ".";
                }
                else
                    ipServer += tmp[i];
            }
            server = new UdpClient(IpServer, 9050);       
            
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
           
            return "127.0.0.1";
        }
        public void CambiaUsername(string nome)
        {
            username = nome;
        }
       
        public void Invia(string msg,int n)
        {
            if (n == 1)//msg norm
            {
                data = Encoding.ASCII.GetBytes(username + ">" +"justalk"+ msg);
                server.Send(data, data.Length);
            }
            else if(n== 2)//entra
            {
                data = Encoding.ASCII.GetBytes(ip + "|" + username + "|" + msg);
                server.Send(data, data.Length);
            }
            else if(n==3)//struct
            {
                data = Encoding.ASCII.GetBytes(msg);
                server.Send(data, data.Length);
            }
            else if (n == 4)//file
            {
                data = Encoding.ASCII.GetBytes("file|"+ip+"|"+ msg);
                server.Send(data, data.Length);
            }

        }
       
    }
}
