using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    public class Xml
    {
        public static readonly string PrefixField = "[[";
        public static readonly string SuffixField = "]]";
        public static readonly string PrefixComment = "<!--";
        public static readonly string SuffixComment = "-->";

        public string[] Request { get; private set; }
        public IList<string> Fields { get; private set; }

        public Xml(string fileXml)
        {
            Request = System.IO.File.ReadAllLines(fileXml);
            Fields = new List<string>();
            ProcessFiels();
        }

        private void ProcessFiels()
        {
            for (int i = 0; i < Request.Length; i++)
            {
                int posStart = Request[i].IndexOf(PrefixField);
                if (posStart >= 0)
                {
                    int length = Request[i].IndexOf(SuffixField) - 2 - posStart;
                    if(length > 0){
                        Fields.Add(Request[i].Substring(posStart + 2, length).Trim());
                    }
                }
            }
        }
    }
}
