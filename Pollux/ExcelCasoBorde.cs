using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    public class ExcelCasoBorde : ExcelBase
    {

        public ExcelCasoBorde(Dictionary<string, IList<ExcelField>> fields, Xml inputXml) : base(inputXml)
        {
            CreateTest(fields, inputXml);
            ProcessXml();

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
                else if (fieldNegocio.IsGuid)
                {
                    Casos(fieldNegocio, fieldsNegocio, CasosBordeGuid(fieldNegocio));
                }
                else if (fieldNegocio.IsDate)
                {
                    Casos(fieldNegocio, fieldsNegocio, CasosBordeFecha(fieldNegocio));
                }
                else if (fieldNegocio.IsText)
                {
                    Casos(fieldNegocio, fieldsNegocio, CasosBordeTexto(fieldNegocio));
                }
            }
            CountFiles = Fields.FirstOrDefault().Value.Count();
        }

        private Dictionary<string, IList<ExcelField>> CasosBordeGuid(ExcelField field)
        {
            Dictionary<string, IList<ExcelField>> list = new Dictionary<string, IList<ExcelField>>();

            list.Add(field.Name, new List<ExcelField>() {
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value=Guid.NewGuid().ToString(),
                            IsTester=true,
                        },
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value="${empty}",
                            IsTester=true,
                        },
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value="${null}",
                            IsTester=true,
                        },
                    });

            return list;
        }


        private Dictionary<string, IList<ExcelField>> CasosBordeFecha(ExcelField field)
        {
            Dictionary<string, IList<ExcelField>> list = new Dictionary<string, IList<ExcelField>>();

            list.Add(field.Name, new List<ExcelField>() {
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value=DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                            IsTester=true,
                        },
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value=DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss"),
                            IsTester=true,
                        },
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value=DateTime.MaxValue.ToString("yyyy-MM-ddTHH:mm:ss"),
                            IsTester=true,
                        },
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value="${empty}",
                            IsTester=true,
                        },
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value="${null}",
                            IsTester=true,
                        },
                    });

            return list;
        }

        private Dictionary<string, IList<ExcelField>> CasosBordeNumerico(ExcelField field)
        {
            Dictionary<string, IList<ExcelField>> list = new Dictionary<string, IList<ExcelField>>();

            list.Add(field.Name, new List<ExcelField>() {
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value=field.MinValue,
                            IsTester=true,
                        },
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value="A",
                            IsTester=true,
                        },
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value="-1",
                            IsTester=true,
                        },
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value="+1",
                            IsTester=true,
                        },
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value="1.1",
                            IsTester=true,
                        },
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value="1,1",
                            IsTester=true,
                        },
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value="1 1",
                            IsTester=true,
                        },
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value="01",
                            IsTester=true,
                        },
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value="${empty}",
                            IsTester=true,
                        },
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value="${null}",
                            IsTester=true,
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
                            IsTester=true,
                        },
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value=Excel.KeyEmpty,
                            IsTester=true,
                        },
                        new ExcelField {
                            Name = field.Name ,
                            Type=field.Type,
                            Value=Excel.KeyNull,
                            IsTester=true,
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
