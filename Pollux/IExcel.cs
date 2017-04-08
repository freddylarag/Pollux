using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    public interface IExcel
    {
        Dictionary<string, IList<ExcelField>> Fields { get; }
        IList<Summary> RequestXml { get; }
    }
}
