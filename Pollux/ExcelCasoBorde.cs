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
                    Casos(fieldNegocio, fieldsNegocio, CasosBordeNumerico(fieldNegocio));
                }
                else if(fieldNegocio.IsDate)
                {

                }
                else if (fieldNegocio.IsText)
                {
                    Casos(fieldNegocio, fieldsNegocio, CasosBordeTexto(fieldNegocio));
                }
            }
        }

        private Dictionary<string, IList<ExcelField>> CasosBordeNumerico(ExcelField field)
        {
            Dictionary<string, IList<ExcelField>> list = new Dictionary<string, IList<ExcelField>>();

            list.Add(field.Name, new List<ExcelField>() {
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value=field.MinValue,
                        },
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value="A",
                        },
                    });

            return list;
        }

        private Dictionary<string, IList<ExcelField>> CasosBordeTexto(ExcelField field)
        {
            Dictionary<string, IList<ExcelField>> list = new Dictionary<string, IList<ExcelField>>();

            list.Add(field.Name, new List<ExcelField>() {
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value=field.MinValue,
                        },
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value=Excel.KeyEmpty,
                        },
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value=Excel.KeyNull,
                        }
                    });

            return list;
        }

        private void Casos(ExcelField field, Dictionary<string, IList<ExcelField>> fieldsNegocio, Dictionary<string, IList<ExcelField>> casosBorde)
        {
            foreach (var item in fieldsNegocio)
            {
                var negocio = item.Value.FirstOrDefault();
                foreach (var itemBorde in casosBorde.FirstOrDefault().Value)
                {
                    var existente = Fields.Where(x => x.Key == item.Key);

                    if (existente?.Count() > 0)
                    {
                        var existe = existente.FirstOrDefault().Value;
                        if (itemBorde.Name == negocio.Name)
                        {
                            existe.Add(itemBorde);
                        }
                        else
                        {
                            var caso = new ExcelField()
                            {
                                Name = existe.FirstOrDefault().Name,
                                Value = existe.FirstOrDefault().MinValue,
                                Type = existe.FirstOrDefault().Type,
                            };
                            existe.Add(caso);
                        }
                    }
                    else
                    {
                        var existe = item.Value.FirstOrDefault();
                        var casos = new List<ExcelField>();
                        if (itemBorde.Name == negocio.Name)
                        {
                            casos.Add(itemBorde);
                            Fields.Add(item.Key, casos);
                        }
                        else
                        {
                            casos.Add(new ExcelField()
                            {
                                Name = existe.Name,
                                Value = existe.MinValue,
                                Type = existe.Type,
                            });
                            Fields.Add(item.Key, casos);
                        }
                    }
                }
            }

        }

    }
}
