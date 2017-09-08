using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    public class FileXml
    {
        public string[] ContentArray { get; private set; }
        public string ContentString { get; private set; }
        public string Path { get; set; }

        public string RelativePath(string workspace)
        { 
            string relative = Path.ToLower().Replace((System.IO.Path.GetDirectoryName(workspace)).ToLower(), "");
            if(relative.Substring(0,1)=="\\")
            {
                relative = relative.Substring(1);
            }
            relative= relative.Replace("\\", "/");
            return relative;
        }

        public FileXml(string[] content)
        {
            ContentArray = content;
            ContentString = content.ToString();
        }
        public FileXml(string content)
        {
            ContentArray = content.Split(new string[]{"\n"},StringSplitOptions.RemoveEmptyEntries);
            ContentString = content;
        }

    }
}
