using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    public class HttpHelperMock : IHttpHelper
    {
        public Response HttpCall(string[] xml, Config config)
        {
            return new Response
            {
                Content = System.IO.File.ReadAllText(@"C:\Users\Freddy\Desktop\ConsultarDeudaContrato (1)\ConsultarDeudaContrato\Reports\ConsultarDeudaContrato_20170410_145659\Results_Basics\1_Response_Ok.xml"),
                StatusCode = HttpStatusCode.OK,
                IsSuccessStatusCode = true,
            };
        }
    }
}
