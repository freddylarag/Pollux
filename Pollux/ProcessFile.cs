using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    public class ProcessFile
    {
        public ProcessFileConfiguration CasosNegocio { get; set; }
        public ProcessFileConfiguration CasosBorde { get; set; }
    }

    public class ProcessFileConfiguration
    {
        public string Name
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(FileTemplate);
            }
        }
        public string FileConfig { get; set; }
        public string FileTemplate { get; set; }
        public string FileData { get; set; }
        public IExcel Excel { get; set; }
        public Config Config { get; set; }
        public Xml Xml { get; set; }
    }
}
