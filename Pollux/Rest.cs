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
    public static class Rest
    {
        private static bool ValidateResponse(string responde, List<Validation> validations){
            try
            {
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

                    XDocument xml = XDocument.Parse(responde);
                    foreach (var validationsItem in validations)
                    {
                        string value=ExtractValue(xml,validationsItem.Tag);
                        ValidationState(value, validationsItem);
                    }
                }
            }
            catch(Exception){
                Console.WriteLine("La regla de validacion no es correcta.");
            }

            return !validations.Any(x=> x.Status==false);
        }

        private static string ExtractValue(XDocument xml, string tag)
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
                        string xmlns = node.FirstOrDefault().Descendants().FirstOrDefault().Name.NamespaceName;
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
                    return node.FirstOrDefault().Value;
                }
            }

            return null;
        }

        private static void ValidationState(string valor, Validation validationsItem)
        {
            if (valor != null)
            {
                if (validationsItem.Operation == ValidationOperation.Equals)
                {
                    if (validationsItem.Value.Equals("NULL", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if(string.IsNullOrEmpty(valor))
                        {
                            validationsItem.Status = true;
                        }
                    }
                    else if (validationsItem.Value.Equals(valor, StringComparison.InvariantCultureIgnoreCase))
                    {
                        validationsItem.Status = true;
                    }
                }
                else if (validationsItem.Operation == ValidationOperation.NotEquals)
                {
                    if (validationsItem.Value.Equals("NULL", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if(!string.IsNullOrEmpty(valor))
                        {
                            validationsItem.Status = true;
                        }
                    }
                    else if (!validationsItem.Value.Equals(valor, StringComparison.InvariantCultureIgnoreCase))
                    {
                        validationsItem.Status = true;
                    }
                }
                else if (validationsItem.Operation == ValidationOperation.Major)
                {
                    double numeric1 = 0;
                    double numeric2 = 0;
                    if (double.TryParse(valor, out numeric1) && double.TryParse(validationsItem.Value, out numeric2))
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
                    if (double.TryParse(valor, out numeric1) && double.TryParse(validationsItem.Value, out numeric2))
                    {
                        if (numeric1 < numeric2)
                        {
                            validationsItem.Status = true;
                        }
                    }
                }
            }        
        }

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


        //X509Certificate2 certificate = new X509Certificate2(requestDto.CertificatePath, requestDto.CertificatePassword);
        //Initialize HttpWebRequest.
        //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://apigateway.qa.bnymellon.com/download-investor-documents/v2/document-folders");
        //request.ClientCertificates.Add(certificate);
        //request.UserAgent = requestDto.UserAgent;
        //request.Headers.Add("Content-Type", "application/json");
        //request.Headers.Add("Authorization", "Bearer a973e0c7e4de593b2e8d3749f180343c");
        //    request.Method = "GET";
        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //Stream receiveStream = response.GetResponseStream();

        //StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

        //Console.WriteLine("Response stream received.");
        //    Console.WriteLine(readStream.ReadToEnd());
        //    response.Close();
        //    readStream.Close();
        private static string HttpCall(string[] xml, Config config)
        {
            string content = string.Empty;
            foreach (var item in xml)
            {
                content += item + "\n";
            }


            //var byteArray = Encoding.ASCII.GetBytes("username:password1234");
            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            //WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            WebRequestHandler webRequestHandler = new WebRequestHandler();
            webRequestHandler.Proxy = new WebProxy("http://mataquito.ing.cl:8080",true,new string[] { },new NetworkCredential("flarag","Paulette.02"));// new PolluxProxy("http://mataquito.ing.cl",8080,"flarag","Paulette.02");
            webRequestHandler.UseProxy = true;
            //webRequestHandler.ClientCertificates.Add(new X509Certificate2(@"E:\Documentos\Respaldo Gonzalo\Pershing\Fase 1\keystore\Qa\pershing.pem"));
            
            System.Net.Http.HttpClient http = new System.Net.Http.HttpClient(webRequestHandler);            
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
            var requestContent = new StringContent(content, System.Text.Encoding.UTF8, config.Headers["Content-Type"]);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, config.Url);

            //.ContinueWith(responseTask =>
            //{
            //    Console.WriteLine("Response: {0}", responseTask.Result);
            //});

            ////var response = http.GetAsync(config.Url).Result;
            ////string responText = response.Content.ReadAsStringAsync().Result;
            //string cc = http.SendAsync(request).Result.Content.ReadAsStringAsync().Result;
            string cc = http.GetAsync(config.Url).Result.Content.ReadAsStringAsync().Result;
            return cc;
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


        private async static Task<string> HttpCallAsync(Uri url, string[] xml, Dictionary<string, string> headers)
        {
            string request = string.Empty;
            foreach (var item in xml)
            {
                request += item + "\n";
            }

            System.Net.Http.HttpClient http = new System.Net.Http.HttpClient();
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    http.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            var response = await http.PostAsync(url, new StringContent(request, System.Text.Encoding.UTF8, "application/xml"));
            string responText = await response.Content.ReadAsStringAsync();
            return responText;
        }

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

            XDocument xDoc=XDocument.Parse(xml);
            string filepath=System.IO.Path.Combine(outputPath, string.Format("{0}_{1}.xml", iteracion, sufijo));
            xDoc.Save(filepath);
            return filepath;
        }
    }
}
