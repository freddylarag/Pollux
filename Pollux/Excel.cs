﻿using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    public class Excel
    {
        private static int StartColumnData=2;
        public Dictionary<string,IList<string>> Fields { get; private set; }

        //public IList<string[]> RequestXml { get; private set; }
        public IList<Summary> RequestXml { get; private set; }

        private string[] _xml { get; set; }

        public int CountFiles { get; private set; }
        
        public Excel(string fileExcel, Xml inputXml)
        {
            Fields = new Dictionary<string,IList<string>>();
            RequestXml = new List<Summary>();
            //if (inputXml != null)
            //{
                _xml = inputXml.Request;
                ProcessFiels(fileExcel, inputXml.Fields);
                ProcessXml();
            //}else
            //{
            //    
            //}
            if (RequestXml.Count == 0)
            {
                RequestXml = new List<Summary>() { new Summary() };
            }
        }

        public Excel(string fileExcel, Xml inputXml, int startColumnData)
        {
            StartColumnData = startColumnData;

            Fields = new Dictionary<string, IList<string>>();
            RequestXml = new List<Summary>();
            _xml = inputXml.Request;
            ProcessFiels(fileExcel, inputXml.Fields);
            ProcessXml();
        }

        private void ProcessXml()
        {
            for (int i = 0; i < CountFiles; i++)
            {
                string[] xmlRequest=new string[_xml.Length];
                _xml.CopyTo(xmlRequest,0);
                foreach (var row in Fields)
                {
                    for (int fila = 0; fila < _xml.Length; fila++)
                    {
                        try
                        {
                            string line = _xml[fila];
                            if (line.IndexOf(string.Format("{0}{1}{2}", Xml.PrefixField, row.Key, Xml.SuffixField)) >= 0)
                            {
                                if (string.IsNullOrWhiteSpace(row.Value[i]))
                                {
                                    xmlRequest[fila] = "";
                                }
                                else
                                {
                                    xmlRequest[fila] = line.Replace(string.Format("{0}{1}{2}", Xml.PrefixField, row.Key, Xml.SuffixField), row.Value[i]);
                                }
                                break;
                            }
                            else if (_xml[fila].IndexOf(Xml.PrefixComment) >= 0)
                            {
                                int posStart = line.IndexOf(Xml.PrefixComment);
                                if(posStart>=0){
                                    int length = line.IndexOf(Xml.SuffixComment)+3-posStart;
                                    if (length > 0)
                                    {
                                        line =line.Replace(line.Substring(posStart, length),"");
                                        if (string.IsNullOrWhiteSpace(line))
                                        {
                                            xmlRequest[fila] = "";
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: {0}", ex.Message);
                        }
                    }
                }
                CleanTagEmpty(ref xmlRequest);
                RequestXml.Add(new Summary { 
                    Request = new FileXml(CleanSpace(xmlRequest)) 
                });
            }
        }

        private void CleanTagEmpty(ref string[] xmlRequest)
        {
            int eliminados = 0;
            xmlRequest=CleanSpace(xmlRequest);

            for (int i = 0; i < xmlRequest.Length; i++)
            {
                if (i-1>=0)
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

        private void ProcessFiels(string fileExcel, IList<string> fielsXml)
        {
            using (var excel = new ExcelQueryFactory(fileExcel)) {
                var header = (from c in excel.WorksheetNoHeader(0)
                            select c).FirstOrDefault();
                var data = (from c in excel.Worksheet(0)
                            select c).ToList();

                foreach (var item in data)
                {
                    if (fielsXml.Any(x => x == item[0]))
                    {
                        List<string> list = new List<string>();
                        for (int i = StartColumnData; i < item.Count; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(header[i]))
                            {
                                list.Add(item[i]);
                            }
                        }
                        Fields.Add(item[0], list);
                        CountFiles = list.Count;
                    }
                }
            }
        }
    }
}
