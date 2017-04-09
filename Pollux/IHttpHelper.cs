using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    public interface IHttpHelper
    {
        Response HttpCall(string[] xml, Config config);
    }
}
