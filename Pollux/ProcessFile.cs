using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    public class ProcessFile
    {
        public string Name {
            get {
                return System.IO.Path.GetFileNameWithoutExtension(FileTemplate);
            }
        }
        public string FileConfig { get; set; }
        public string FileTemplate { get; set; }
        public string FileData { get; set; }
        public Excel Excel { get; set; }
        public Config Config { get; set; }
        public Xml Xml { get; set; } 
    }
}
