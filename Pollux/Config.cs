using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Pollux
{
    public class Config
    {
        public string Description { get; set; }
        public string Parameters { get; set; }
        public TypeProtocol Type { get; set; }
        public TypeMethod Method { get; set; }
        public Uri Url { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public List<Validation> Validations { get; set; }

        public enum TypeProtocol
        {
            SOAP,
            REST,
        }

        public enum TypeMethod
        {
            GET,
            POST,
            PUT,
            DELETE,
        }

        public Config(string path)
        {
            XDocument xml=XDocument.Load(path);

            if (xml != null)
            {
                //Description
                Description = xml.Elements("Config")
                        .Elements("Description")
                        .FirstOrDefault()
                        .Value;

                //TypeProtocol
                TypeProtocol protocolo;
                string textoProtocolo = xml.Elements("Config")
                        .Elements("EndPoint")
                        .Elements("Type")
                        .FirstOrDefault()
                        .Value;
                if(Enum.TryParse<TypeProtocol>(textoProtocolo, true, out protocolo))
                {
                    Type = protocolo;
                }else
                {
                    Type = TypeProtocol.SOAP;
                }

                //Method
                TypeMethod metodo;
                string textoMetodo = xml.Elements("Config")
                        .Elements("EndPoint")
                        .Elements("Method")
                        .FirstOrDefault()
                        .Value;
                if (Enum.TryParse<TypeMethod>(textoMetodo, true, out metodo))
                {
                    Method = metodo;
                }
                else
                {
                    Method = TypeMethod.POST;
                }


                if (Type == TypeProtocol.REST)
                {
                    //Parameters
                    Parameters = xml.Elements("Config")
                            .Elements("EndPoint")
                            .Elements("Parameters")
                            .FirstOrDefault()
                            .Value;
                }
                
                //Url
                Url =new Uri(xml.Elements("Config")
                        .Elements("EndPoint")
                        .Elements("Url")
                        .FirstOrDefault()
                        .Value ?? "");

                //encabezados
                Headers = new Dictionary<string, string>();
                var encabezados=xml.Elements("Config")
                        .Elements("HeaderCollection")
                        .Elements("Header");
                foreach (var item in encabezados)
	            {
                    Headers.Add(item.Element("Code").Value, item.Element("Value").Value);
	            }

                //validaciones
                Validations = new List<Validation>();
                var validaciones = xml.Elements("Config")
                        .Elements("ValidationCollection")
                        .Elements("Validation");
                if (validaciones != null)
                {
                    foreach (var item in validaciones)
                    {
                        Validations.Add(new Validation{
                            Tag = item.Element("Tag").Value,
                            Operation = (ValidationOperation)Enum.Parse(typeof(ValidationOperation), item.Element("Operation").Value, true),
                            Value = item.Element("Value").Value,
                        });
                    }
                }
            }
        }
    }
}
