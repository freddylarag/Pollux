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
        public ValidationCollections Validations { get; set; }
        

        public enum TypeProtocol
        {
            None,
            SOAP,
            REST,
        }

        public enum TypeMethod
        {
            None,
            GET,
            POST,
            PUT,
            DELETE,
        }

        public Config(string path, string url)
        {
            try
            {
                XDocument xml = XDocument.Load(path);

                if (xml != null)
                {
                    //Description
                    Description = xml.Elements("Config")?
                            .Elements("Description")?
                            .FirstOrDefault()?
                            .Value;
                    if(string.IsNullOrWhiteSpace(Description) || Description.Trim() == "?")
                    {
                        throw new NullReferenceException("La variable 'Description' no se encuentra definida.");
                    }

                    //TypeProtocol
                    TypeProtocol protocolo;
                    string textoProtocolo = xml.Elements("Config")?
                            .Elements("EndPoint")?
                            .Elements("Type")?
                            .FirstOrDefault()?
                            .Value;
                    if (string.IsNullOrWhiteSpace(textoProtocolo))
                    {
                        Type = TypeProtocol.SOAP;
                    }
                    else if (Enum.TryParse<TypeProtocol>(textoProtocolo, true, out protocolo))
                    {
                        Type = protocolo;
                    }
                    if (Type == TypeProtocol.None)
                    {
                        throw new NullReferenceException("La variable 'Type' no se encuentra definida.");
                    }

                    //Method
                    TypeMethod metodo;
                    string textoMetodo = xml.Elements("Config")?
                            .Elements("EndPoint")?
                            .Elements("Method")?
                            .FirstOrDefault()?
                            .Value;
                    if (string.IsNullOrWhiteSpace(textoMetodo))
                    {
                        if (Type == TypeProtocol.SOAP)
                        {
                            Method = TypeMethod.POST;
                        }
                        else
                        {
                            Method = TypeMethod.GET;
                        }
                    }
                    else if (Enum.TryParse<TypeMethod>(textoMetodo, true, out metodo))
                    {
                        Method = metodo;
                    }
                    if (Method == TypeMethod.None)
                    {
                        throw new NullReferenceException("La variable 'Method' no se encuentra definida.");
                    }


                    if (Type == TypeProtocol.REST)
                    {
                        //Parameters
                        Parameters = xml.Elements("Config")?
                                .Elements("EndPoint")?
                                .Elements("RESTParameters")?
                                .FirstOrDefault()?
                                .Value;
                    }


                    //Url
                    if (string.IsNullOrWhiteSpace(url))
                    {
                        url = xml.Elements("Config")?
                                .Elements("EndPoint")?
                                .Elements("Url")?
                                .FirstOrDefault()?
                                .Value;
                    }
                    if (string.IsNullOrWhiteSpace(url) || url.Trim() == "?")
                    {
                        throw new NullReferenceException("La variable 'Url' no se encuentra definida.");
                    }
                    Url = new Uri(url);



                    //encabezados
                    Headers = new Dictionary<string, string>();
                    var encabezados = xml.Elements("Config")?
                            .Elements("HeaderCollection")?
                            .Elements("Header");
                    foreach (var item in encabezados)
                    {
                        Headers.Add(item.Element("Code")?.Value, item.Element("Value")?.Value);
                    }
                    
                    if (Type == TypeProtocol.SOAP && string.IsNullOrWhiteSpace(Headers["SOAPAction"]))
                    {
                        throw new NullReferenceException("La variable 'SOAPAction' no se encuentra definida.");
                    }
                    var enc = Headers.FirstOrDefault(x => string.IsNullOrWhiteSpace(x.Value) || x.Value.Equals("?"));
                    if (enc.Key != null)
                    {
                        throw new NullReferenceException(string.Format("La variable '{0}' no se encuentra definida correctamente.", enc.Key));
                    }

                    //validaciones
                    Validations = new ValidationCollections();
                    var validaciones = xml.Elements("Config")?
                            .Elements("ValidationCollection").FirstOrDefault();
                    if (validaciones != null)
                    {
                        Validations.ValidationsFault = ValidationSection("Envelope.Body.Fault", validaciones?.Elements("Envelope.Body.Fault")?.Elements("Validation"));
                        Validations.ValidationsHeader = ValidationSection("Envelope.Header", validaciones?.Elements("Envelope.Header")?.Elements("Validation"));
                        Validations.ValidationsBody = ValidationSection("Envelope.Body", validaciones?.Elements("Envelope.Body")?.Elements("Validation"));
                        if (Validations.ValidationsBody?.Count == 0)  
                        {
                            Validations.ValidationsBody = ValidationSection("Envelope.Body", validaciones?.Elements("Validation"));
                        }
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                throw new NullReferenceException("El archivo .config no es correcto.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("El archivo.config no es correcto.", ex);
            }
        }

        static List<Validation> ValidationSection(string prefix, IEnumerable<XElement> section)
        {
            List<Validation> list = new List<Validation>();

            if (section != null)
            {
                foreach (var item in section)
                {
                    var validacion = new Validation(prefix)
                    {
                        Tag = item.Element("Tag")?.Value,
                        Operation = (ValidationOperation)Enum.Parse(typeof(ValidationOperation), item.Element("Operation")?.Value, true),
                    };
                    item.Elements("Value")?.ToList()?.ForEach(x => validacion?.Values.Add(x?.Value));
                    list.Add(validacion);
                }
            }

            return list;
        }

        public static void CreateTemplate(string path)
        {
            string xml = @"
<Config>
	<Description>?</Description>
	<EndPoint>
		<Url>?</Url>
		
		<!-- Opcional: Valores posibles SOAP | REST -->
		<!-- El valor por omisión: SOAP -->
		<!-- <Type>?</Type>--> 
		
		<!-- Opcional: Algunos de los valores posibles POST | GET | PUT | DELETE  -->
		<!-- El valor por omisión para SOAP: POST -->
		<!-- El valor por omisión para REST: GET -->
		<!--<Method>?</Method>--> 
		
		<!-- Opcional: Solo aplica para REST -->
		<!--<RESTParameters>?</RESTParameters>-->
	</EndPoint>
	
	<HeaderCollection>
        <!-- Válido solo para SOAP -->
        <Header>
			<Code>SOAPAction</Code>
			<Value>?</Value>
		</Header>

        <!-- Parámetros estándar -->
		<Header>
			<Code>Accept-Encoding</Code>
			<Value>gzip,deflate</Value>
		</Header>
		
		<!-- Algunos de los valores posibles: application/json | text/xml -->
		<Header>
			<Code>Content-Type</Code>
			<Value>?</Value>
		</Header>
		
		<!-- Opcional -->
		<!--
		<Header>
			<Code>Accept</Code>
			<Value>?</Value>
		</Header>
		-->
	</HeaderCollection>
	

    <!-- Ejemplos de Validaciones 
         'ValidationCollection' puede contener 3 estructuras:
            * Envelope.Header
            * Envelope.Body
            * Envelope.Body.Fault

        Los valores posibles para 'Operation':
            * Equals: Para valores alfanúmericos, puede usar ${null} para indicar valores nulos
            * NotEquals: Para valores alfanúmericos, puede usar ${null} para indicar valores nulos
            * Major: Para valores númericos
            * Minor: Para valores númericos
    -->
    <!--
	<ValidationCollection>

        <Envelope.Header>
            <Validation>
                <Tag>Section</Tag>
                <Operation>NotEquals</Operation>
                <Value>2800</Value>
            </Validation>
        </Envelope.Header>

        <Envelope.Body>
            <Validation>
                <Tag>ObtenerProductoResponse.ObtenerProductoResult.Precio</Tag>
                <Operation>NotEquals</Operation>
                <Value>3800</Value>
                <Value>2800</Value>
            </Validation>
            <Validation>
                <Tag>ObtenerProductoResponse.ObtenerProductoResult.Stock</Tag>
                <Operation>Major</Operation>
                <Value>100</Value>
            </Validation>
            <Validation>
                <Tag>ObtenerProductoResponse.ObtenerProductoResult.SKU</Tag>
                <Operation>NotEquals</Operation>
                <Value>${null}</Value>
            </Validation>
        </Envelope.Body>

        <Envelope.Body.Fault>
		    <Validation>
			    <Tag>detail.soapFault.tipoError</Tag>
			    <Operation>NotEquals</Operation>
			    <Value>TECNICO</Value>
		    </Validation>
		    <Validation>
			    <Tag>detail.soapFault.tipoError</Tag>
			    <Operation>NotEquals</Operation>
			    <Value>NEGOCIO</Value>
		    </Validation>
		</Envelope.Body.Fault>
	</ValidationCollection>
    -->

</Config>";

            System.IO.File.WriteAllText(path,xml);
        }
    }
}
