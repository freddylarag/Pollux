using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace Pollux
{
    public static class SoapManager
    {
        public static string VALOR_NULO1 = "NULL";
        public static string VALOR_NULO2 = Excel.KeyNull;
        private static HttpHelper Http=new HttpHelper();


        #region Validaciones

        public static bool ValidateSection(string response, ValidationCollections validations)
        {
            XDocument xml = null;
            try
            {
                xml = XDocument.Parse(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("La respuesta del servicio no se reconoce como un XML válido.\n{0}", ex.Message);
            }

            var body = true;
            var header = true;
            var fault = true;

            if (validations?.ValidationsBody?.Count > 0)
            {
                if (ExtractValue(xml, "Envelope.Body").IsExist && !ExtractValue(xml, "Envelope.Body.Fault").IsExist)
                {
                    body = ValidateResponse(xml, validations.ValidationsBody);
                }
                else
                {
                    body = true;
                }
            }
            if (validations?.ValidationsHeader?.Count > 0)
            {
                if (ExtractValue(xml, "Envelope.Header").IsExist)
                {
                    header = ValidateResponse(xml, validations.ValidationsHeader);
                }else
                {
                    header = true;
                }
            }

            if (validations?.ValidationsFault?.Count == 0)
            {
                if (ExtractValue(xml, "Envelope.Body.Fault").IsExist)
                {
                    fault = false;
                }
            }
            else    
            {
                if (ExtractValue(xml, "Envelope.Body.Fault").IsExist)
                {
                    fault = ValidateResponse(xml, validations.ValidationsFault);
                }
                else
                {
                    fault = true;
                }
            }

            return body && header && fault;
        }

        private static bool ValidateResponse(XDocument xml, List<Validation> validations){
            try
            {
                if (xml == null)
                {
                    return false;
                }

                if (validations == null || validations.Count==0)
                {
                    return true;
                }
                else
                {
                    foreach (var item in validations)
                    {
                        item.Status = false;
                    }

                    foreach (var validationsItem in validations)
                    {
                        var value=ExtractValue(xml,validationsItem.Tag);
                        ValidationState(value, validationsItem);
                    }
                }
            }
            catch(Exception ex){
                Console.WriteLine("La regla de validación no es correcta.\n{0}",ex.Message);
            }

            return !validations.Any(x=> x.Status==false);
        }

        public class ValidationValue
        {
            public string Tag { get; set; }
            public bool IsExist { get; set; }
            public string Value { get; set; }
        }

        public static ValidationValue ExtractValue(XDocument xml, string tag)
        {
            IEnumerable<XElement> node = null;
            if (xml != null && !string.IsNullOrWhiteSpace(tag))
            {
                var structure = tag.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in structure)
                {
                    if (node == null)
                    {
                        string xmlns = xml.Root.Name.NamespaceName;
                        if (!string.IsNullOrWhiteSpace(xmlns))
                        {
                            xmlns = "{" + xmlns + "}";
                        }
                        node = xml.Elements(xmlns + item);
                    }
                    else if (node.Count() > 0)
                    {
                        string xmlns = IdentifyXmlns(item, node);
                        //if (!string.IsNullOrWhiteSpace(xmlns))
                        //{
                        //    xmlns = "{" + xmlns + "}";
                        //}
                        node = node.Elements(xmlns + item);
                    }
                    else
                    {
                        break;
                    }
                }

                if (node != null && node.Count() > 0)
                {
                    return new ValidationValue
                    {
                        Tag = tag,
                        IsExist = true,
                        Value = node?.FirstOrDefault()?.Value,
                    };
                }
            }

            return new ValidationValue
            {
                Tag = tag,
                IsExist = false,
                Value = null,
            };
        }

        private static string IdentifyXmlns(string itemTag, IEnumerable<XElement> node)
        {
            string xmlns = string.Empty;

            foreach (var item in node)
            {
                var xnode=item.FirstNode;
                while (xnode != null)
                {
                    xmlns = (xnode as XElement).Name.NamespaceName;
                    if (!string.IsNullOrWhiteSpace(xmlns))
                    {
                        xmlns = "{" + xmlns + "}";
                    }

                    var existe = node.Elements(xmlns + itemTag).FirstOrDefault();
                    if (existe!=null)
                    {
                        return xmlns;
                    }
                    if (xnode.Equals(item.NextNode ?? item.LastNode))
                    {
                        xnode = null;
                    }else
                    {
                        xnode = item.NextNode ?? item.LastNode;
                    }
                }
            }

            return xmlns;
        }

        private static void ValidationState(ValidationValue valor, Validation validationsItem)
        {
            if (valor != null && !valor.IsExist)
            {
                validationsItem.Status = false;
            }
            else if(valor != null)
            {
                List<bool> estadoValores = new List<bool>();
                foreach (var value in validationsItem.Values)
                {
                    if (validationsItem.Operation == ValidationOperation.Equals)
                    {
                        if (value.Equals(VALOR_NULO1, StringComparison.InvariantCultureIgnoreCase) || value.Equals(VALOR_NULO2, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (string.IsNullOrEmpty(valor.Value))
                            {
                                estadoValores.Add(true);
                            }
                        }
                        else if (value.Equals(valor.Value, StringComparison.InvariantCultureIgnoreCase))
                        {
                            estadoValores.Add(true);
                        }
                    }
                    else if (validationsItem.Operation == ValidationOperation.NotEquals)
                    {
                        if (value.Equals(VALOR_NULO1, StringComparison.InvariantCultureIgnoreCase) || value.Equals(VALOR_NULO2, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (!string.IsNullOrEmpty(valor.Value))
                            {
                                estadoValores.Add(true);
                            }
                        }
                        else if (!value.Equals(valor.Value, StringComparison.InvariantCultureIgnoreCase))
                        {
                            estadoValores.Add(true);
                        }
                    }
                    else if (validationsItem.Operation == ValidationOperation.Major)
                    {
                        double numeric1 = 0;
                        double numeric2 = 0;
                        if (double.TryParse(valor.Value, out numeric1) && double.TryParse(value, out numeric2))
                        {
                            if (numeric1 > numeric2)
                            {
                                estadoValores.Add(true);
                            }
                        }
                    }
                    else if (validationsItem.Operation == ValidationOperation.Minor)
                    {
                        double numeric1 = 0;
                        double numeric2 = 0;
                        if (double.TryParse(valor.Value, out numeric1) && double.TryParse(value, out numeric2))
                        {
                            if (numeric1 < numeric2)
                            {
                                estadoValores.Add(true);
                            }
                        }
                    }
                }

                //rellenar lista
                for (int i = estadoValores.Count; i < validationsItem.Values.Count(); i++)
                {
                    estadoValores.Add(false);
                }

                if (validationsItem.Operation == ValidationOperation.Equals)
                {
                    validationsItem.Status = estadoValores.Any(x => x == true);
                }
                else if (validationsItem.Operation == ValidationOperation.NotEquals)
                {
                    validationsItem.Status = !estadoValores.Any(x => x == false);
                }
                else if (validationsItem.Operation == ValidationOperation.Major)
                {
                    validationsItem.Status = !estadoValores.Any(x => x == false);
                }
                else if (validationsItem.Operation == ValidationOperation.Minor)
                {
                    validationsItem.Status = !estadoValores.Any(x => x == false);
                }

            }
        }

        #endregion

        public static string[] Start(string path, ProcessFileConfiguration processFile)
        {
            List<string> result = new List<string>();
            int i = 1;

            if (processFile?.Excel?.RequestXml?.Count > 0) {
                foreach (var xmlRequest in processFile.Excel.RequestXml)
                {
                    Notify(i, xmlRequest, processFile, System.IO.Path.Combine(path, "Results"));
                    i++;
                }
            }

            return result.ToArray();
        }

        private static string Notify(int i,Summary xmlRequest, ProcessFileConfiguration processFile, string path)
        {
            string mensaje = string.Empty;
            xmlRequest.Request.Path = WriteXml(i.ToString(), path, xmlRequest.Request?.ContentArray ?? new string[] {""}, "Request");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            xmlRequest.Response = Http.HttpCall(xmlRequest.Request.ContentArray, processFile.Config);
            watch.Stop();

            xmlRequest.IsCorrect = ValidateSection(xmlRequest.Response.Content, processFile.Config.Validations);
            xmlRequest.TimeOut = watch.Elapsed;

            if (xmlRequest.IsCorrect)
            {
                xmlRequest.Response.Path = WriteXml(i.ToString(), path, xmlRequest.Response.Content, "Response_Ok");
                mensaje = string.Format("Caso {0} procesado\tOk\tTimeout: {1}", i, watch.Elapsed);
                Console.WriteLine(mensaje);
            }
            else
            {
                xmlRequest.Response.Path = WriteXml(i.ToString(), path, xmlRequest.Response.Content, "Response_Error");
                mensaje = string.Format("Caso {0} procesado\tError\tTimeout: {1}", i, watch.Elapsed);
                Console.WriteLine(mensaje);
            }

            return mensaje;
        }
 
        private static string WriteXml(string iteracion, string path, string[] xml, string sufijo)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            string filepath=System.IO.Path.Combine(path, string.Format("{0}_{1}.xml", iteracion, sufijo));
            System.IO.File.WriteAllLines(filepath, xml);
            return filepath;
        }

        private static string WriteXml(string iteracion, string path, string xml, string sufijo)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            string filepath = string.Empty;
            try
            {
                filepath = System.IO.Path.Combine(path, string.Format("{0}_{1}.xml", iteracion, sufijo));
                XDocument xDoc = XDocument.Parse(xml);
                xDoc.Save(filepath);
            }catch(Exception ex)
            {
                filepath = System.IO.Path.Combine(path, string.Format("{0}_{1}.txt", iteracion, sufijo));
                System.IO.File.WriteAllText(filepath, xml);
            }
            return filepath;
        }
    }
}
