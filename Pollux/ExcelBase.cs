using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    public class ExcelBase
    {
        public ExcelBase(Xml inputXml)
        {
            Fields = new Dictionary<string, IList<ExcelField>>();
            RequestXml = new List<Summary>();

            _xml = inputXml.Request;
        }
        public static string KeyNull = "${null}";
        public static string KeyEmpty = "${empty}";

        public Dictionary<string, IList<ExcelField>> Fields { get; protected set; }

        public IList<Summary> RequestXml { get; protected set; }

        public int CountFiles { get; set; }

        private string[] _xml { get; set; }

        protected void ProcessXml()
        {
            for (int i = 0; i < CountFiles; i++)
            {
                string[] xmlRequest = new string[_xml.Length];
                _xml.CopyTo(xmlRequest, 0);
                foreach (var row in Fields)
                {
                    for (int fila = 0; fila < _xml.Length; fila++)
                    {
                        try
                        {
                            string line = _xml[fila];
                            if (line.IndexOf(string.Format("{0}{1}{2}", Xml.PrefixField, row.Key, Xml.SuffixField)) >= 0)
                            {
                                if (row.Value[i].Value.Trim().Equals(KeyEmpty, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    xmlRequest[fila] = line.Replace(string.Format("{0}{1}{2}", Xml.PrefixField, row.Key, Xml.SuffixField), "");
                                }
                                else if (row.Value[i].Value.Trim().Equals(KeyNull, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    xmlRequest[fila] = "<!--" + line.Replace(string.Format("{0}{1}{2}", Xml.PrefixField, row.Key, Xml.SuffixField), "").Trim() + "-->";
                                }
                                else
                                {
                                    xmlRequest[fila] = line.Replace(string.Format("{0}{1}{2}", Xml.PrefixField, row.Key, Xml.SuffixField), row.Value[i].Value);
                                }
                                break;
                            }
                            //else if (_xml[fila].IndexOf(Xml.PrefixComment) >= 0)
                            //{
                            //    int posStart = line.IndexOf(Xml.PrefixComment);
                            //    if(posStart>=0){
                            //        int length = line.IndexOf(Xml.SuffixComment)+3-posStart;
                            //        if (length > 0)
                            //        {
                            //            line =line.Replace(line.Substring(posStart, length),"");
                            //            if (string.IsNullOrWhiteSpace(line))
                            //            {
                            //                xmlRequest[fila] = "";
                            //            }
                            //        }
                            //    }
                            //}
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: {0}", ex.Message);
                        }
                    }
                }
                CleanTagEmpty(ref xmlRequest);
                RequestXml.Add(new Summary
                {
                    Request = new FileXml(CleanSpace(xmlRequest))
                });
            }
        }

        private void CleanTagEmpty(ref string[] xmlRequest)
        {
            int eliminados = 0;
            xmlRequest = CleanSpace(xmlRequest);

            for (int i = 0; i < xmlRequest.Length; i++)
            {
                if (i - 1 >= 0)
                {
                    if (!string.IsNullOrWhiteSpace(xmlRequest[i - 1]) && xmlRequest[i - 1].Trim() == xmlRequest[i].Trim().Replace("/", ""))
                    {
                        xmlRequest[i - 1] = "";
                        xmlRequest[i] = "";
                        eliminados++;
                    }
                }
            }

            if (eliminados != 0)
            {
                CleanTagEmpty(ref xmlRequest);
            }

        }

        private string[] CleanSpace(string[] xmlRequest)
        {
            List<string> result = new List<string>();
            foreach (var item in xmlRequest)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    result.Add(item);
                }
            }
            return result.ToArray();
        }
    }
}
