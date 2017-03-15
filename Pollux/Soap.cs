using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Pollux
{
    public static class Soap
    {
        #region Validaciones
        private static bool ValidateResponse(string responde, List<Validation> validations){
            XDocument xml=null;
            try
            {
                xml = XDocument.Parse(responde);
            }
            catch (Exception ex)
            {
                Console.WriteLine("La respuesta del servicio no se reconoce como un XML válido.\n{0}", ex.Message);
            }

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

        private static ValidationValue ExtractValue(XDocument xml, string tag)
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
                        string xmlns = node?.FirstOrDefault()?.Descendants()?.FirstOrDefault()?.Name?.NamespaceName;
                        if (!string.IsNullOrWhiteSpace(xmlns))
                        {
                            xmlns = "{" + xmlns + "}";
                        }
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

        private static void ValidationState(ValidationValue valor, Validation validationsItem)
        {
            if (valor != null && !valor.IsExist)
            {
                validationsItem.Status = false;
            }
            else if(valor != null)
            {
                foreach (var value in validationsItem.Values)
                {
                    if (validationsItem.Operation == ValidationOperation.Equals)
                    {
                        if (value.Equals("NULL", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (string.IsNullOrEmpty(valor.Value))
                            {
                                validationsItem.Status = true;
                            }
                        }
                        else if (value.Equals(valor.Value, StringComparison.InvariantCultureIgnoreCase))
                        {
                            validationsItem.Status = true;
                        }
                    }
                    else if (validationsItem.Operation == ValidationOperation.NotEquals)
                    {
                        if (value.Equals("NULL", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (!string.IsNullOrEmpty(valor.Value))
                            {
                                validationsItem.Status = true;
                            }
                        }
                        else if (!value.Equals(valor.Value, StringComparison.InvariantCultureIgnoreCase))
                        {
                            validationsItem.Status = true;
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
                                validationsItem.Status = true;
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
                                validationsItem.Status = true;
                            }
                        }
                    }
                }
            }        
        }

        #endregion

        public static string[] Start(string path, ProcessFile processFile)
        {
            List<string> result = new List<string>();
            int i = 1;

            foreach (var xmlRequest in processFile.Excel.RequestXml)
            {
                Notify(i, xmlRequest, processFile, path);
                i++;
            }

            return result.ToArray();
        }

        private static string Notify(int i,Summary xmlRequest, ProcessFile processFile, string path)
        {
            string fechaEjecucion = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string mensaje = string.Empty;
            xmlRequest.Request.Path = WriteXml(i.ToString(), path, xmlRequest.Request?.ContentArray ?? new string[] {""}, fechaEjecucion, processFile.Name, "Request");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            xmlRequest.Response = new FileXml(HttpCall(xmlRequest.Request.ContentArray, processFile.Config));
            watch.Stop();

            xmlRequest.IsCorrect = ValidateResponse(xmlRequest.Response.ContentString, processFile.Config.Validations);
            xmlRequest.TimeOut = watch.Elapsed;

            if (xmlRequest.IsCorrect)
            {
                xmlRequest.Response.Path = WriteXml(i.ToString(), path, xmlRequest.Response.ContentString, fechaEjecucion, processFile.Name, "Response_Ok");
                mensaje = string.Format("Caso {0} procesado\tOk\tTimeout: {1}", i, watch.Elapsed);
                Console.WriteLine(mensaje);
            }
            else
            {
                xmlRequest.Response.Path = WriteXml(i.ToString(), path, xmlRequest.Response.ContentString, fechaEjecucion, processFile.Name, "Response_Error");
                mensaje = string.Format("Caso {0} procesado\tError\tTimeout: {1}", i, watch.Elapsed);
                Console.WriteLine(mensaje);
            }

            return mensaje;
        }
        
        private static string HttpCall(string[] xml, Config config)
        {
            string content = string.Empty;
            foreach (var item in xml)
            {
                content += item + "\n";
            }

            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            System.Net.Http.HttpClient http = new System.Net.Http.HttpClient(handler);            
            foreach (var item in config.Headers)
            {
                if (!item.Key.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!string.IsNullOrWhiteSpace(item.Value))
                    {
                        http.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }
            }
            http.DefaultRequestHeaders.Add("User-Agent",Program.ApplicationName);

            var response = http.PostAsync(config.Url, new StringContent(content, System.Text.Encoding.UTF8, config.Headers["Content-Type"])).Result;
            response.EnsureSuccessStatusCode();
            string responText = response.Content.ReadAsStringAsync().Result;
            return responText;
        }

        //private static string HttpCall(string[] xml, Config config)
        //{
        //    string request = string.Empty;
        //    foreach (var item in xml)
        //    {
        //        request += item + "\n";
        //    }

        //    //Creating the X.509 certificate.
        //    //X509Certificate2 certificate = new X509Certificate2(requestDto.CertificatePath, requestDto.CertificatePassword);
        //    //Initialize HttpWebRequest.
        //    //HttpWebRequest requestss = (HttpWebRequest)WebRequest.Create("");
        //    //Set the Timeout.
        //    //requestss.Timeout = requestDto.TimeoutMilliseconds;
        //    //Add certificate to request.
        //    //requestss.ClientCertificates.Add();

        //    var dfff = new X509Certificate(@"E:\Documentos\Respaldo Gonzalo\Pershing\Fase 1\keystore\Qa\pershing.pem");

        //    //WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
        //    System.Net.Http.HttpClient http = new System.Net.Http.HttpClient();
        //    foreach (var item in config.Headers)
        //    {
        //        if (!item.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
        //        {
        //            if (!string.IsNullOrWhiteSpace(item.Value))
        //            {
        //                http.DefaultRequestHeaders.Add(item.Key, item.Value);
        //            }
        //        }
        //    }
        //    var response = http.PostAsync(config.Url, new StringContent(request, System.Text.Encoding.UTF8, config.Headers["Content-Type"])).Result;
        //    string responText = response.Content.ReadAsStringAsync().Result;
        //    return responText;
        //}

        private static string WriteXml(string iteracion, string path, string[] xml, string fechaEjecucion, string prefijo, string sufijo)
        {
            string outputPath = System.IO.Path.Combine(path, string.Format(@"{0}_{1}", prefijo, fechaEjecucion));
            if (!System.IO.Directory.Exists(outputPath))
            {
                System.IO.Directory.CreateDirectory(outputPath);
            }
            string filepath=System.IO.Path.Combine(outputPath, string.Format("{0}_{1}.xml", iteracion, sufijo));
            System.IO.File.WriteAllLines(filepath, xml);
            return filepath;
        }

        private static string WriteXml(string iteracion, string path, string xml, string fechaEjecucion, string prefijo, string sufijo)
        {
            string outputPath = System.IO.Path.Combine(path, string.Format(@"{0}_{1}", prefijo, fechaEjecucion));
            if (!System.IO.Directory.Exists(outputPath))
            {
                System.IO.Directory.CreateDirectory(outputPath);
            }

            string filepath = string.Empty;
            try
            {
                filepath = System.IO.Path.Combine(outputPath, string.Format("{0}_{1}.xml", iteracion, sufijo));
                XDocument xDoc = XDocument.Parse(xml);
                xDoc.Save(filepath);
            }catch(Exception ex)
            {
                filepath = System.IO.Path.Combine(outputPath, string.Format("{0}_{1}.txt", iteracion, sufijo));
                System.IO.File.WriteAllText(filepath, xml);
            }
            return filepath;
        }
    }
}
