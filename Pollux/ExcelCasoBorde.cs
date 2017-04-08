using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    public class ExcelCasoBorde : IExcel
    {
        public Dictionary<string,IList<ExcelField>> Fields { get; private set; }

        public IList<Summary> RequestXml { get; private set; }

        public ExcelCasoBorde(Dictionary<string, IList<ExcelField>> fields, Xml inputXml)
        {
            Fields = new Dictionary<string,IList<ExcelField>>();
            RequestXml = new List<Summary>();

            CreateTest(fields, inputXml);

            if (RequestXml.Count == 0)
            {
                RequestXml = new List<Summary>() { new Summary() };
            }
        }

        private void CreateTest(Dictionary<string, IList<ExcelField>> fieldsNegocio, Xml inputXml)
        {
            foreach (var item in fieldsNegocio)
            {
                var fieldNegocio = item.Value?.FirstOrDefault();
                if (fieldNegocio.IsNumeric)
                {
                    Fields.Add(fieldNegocio.Name, new List<ExcelField>() {
                        new ExcelField {
                            Name = fieldNegocio.Name ,
                            Type=fieldNegocio.Type,
                            Value="A",
                        }
                    });
                }
                else if(fieldNegocio.IsDate)
                {

                }
                else if (fieldNegocio.IsText)
                {
                    Fields.Add(fieldNegocio.Name, new List<ExcelField>() {
                        new ExcelField {
                            Name = fieldNegocio.Name ,
                            Type=fieldNegocio.Type,
                            Value="A",
                        },
                        new ExcelField {
                            Name = fieldNegocio.Name ,
                            Type=fieldNegocio.Type,
                            Value=Excel.KeyEmpty,
                        },
                        new ExcelField {
                            Name = fieldNegocio.Name ,
                            Type=fieldNegocio.Type,
                            Value=Excel.KeyNull,
                        }
                    });
                }
            }
        }

    }
}
