using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    public class Excel : ExcelBase
    {

        private static int StartColumnData=2;

        //public Excel(string fileExcel, Xml inputXml) : base(inputXml)
        //{
        //    ProcessFiels(fileExcel, inputXml.Fields);
        //    ProcessXml();

        //    if (RequestXml.Count == 0)
        //    {
        //        RequestXml = new List<Summary>() { new Summary() };
        //    }
        //}

        public Excel(string fileExcel, Xml inputXml, int startColumnData) : base(inputXml)
        {
            StartColumnData = startColumnData;
            ProcessFiels(fileExcel, inputXml.Fields);
            ProcessXml();

            if (RequestXml.Count == 0)
            {
                RequestXml = new List<Summary>() { new Summary() };
            }
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
                        List<ExcelField> list = new List<ExcelField>();
                        for (int i = StartColumnData; i < item.Count; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(header[i]))
                            {
                                var field = new ExcelField
                                {
                                    Name = item[0],
                                    Value = item[i]
                                };

                                //Valida el tipo
                                ExcelFieldType tipo = ExcelFieldType.None;
                                Enum.TryParse(item[1]?.Value?.ToString()?.Replace(":",""), true, out tipo);
                                field.Type = tipo;

                                list.Add(field);
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
