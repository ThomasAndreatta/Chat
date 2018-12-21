using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DemoChatForm.Classi
{
    public class Listone
    {
        public List<string> nomi ;
        public List<IPAddress> ip ;
        public Listone(List<string> nominativi,List<IPAddress> indirizzi)
        {
            nomi = nominativi;
            ip  = indirizzi;
        }
        public Listone()
        {
            nomi = new List<string>();
            ip = new List<IPAddress>();
        }

        public void AddIp(IPAddress ipp)
        {

        }
    }
}
