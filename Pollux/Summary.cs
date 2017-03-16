using SoapTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    public class Summary
    {
        public Summary()
        {
            Parameters = new List<string>();
            Template = new FileXml("");
            Request = new FileXml("");
            Response = new Response();
        }

        public List<string> Parameters { get; set; }
        public FileXml Template { get; set; }
        public FileXml Request { get; set; }
        public Response Response { get; set; }
        public TimeSpan TimeOut { get; set; }
        public bool IsCorrect { get; set; }
        public string Status {
            get
            {
                if(IsCorrect){
                    return "Aprobado";
                }
                return "Rechazado";
            }
        }
    }
}
