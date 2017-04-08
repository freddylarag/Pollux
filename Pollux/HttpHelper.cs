using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    public class HttpHelper
    {
        public Response HttpCall(string[] xml, Config config)
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
            http.DefaultRequestHeaders.Add("User-Agent", Program.ApplicationName);

            var response = http.PostAsync(config.Url, new StringContent(content, System.Text.Encoding.UTF8, config.Headers["Content-Type"])).Result;
            return new Response
            {
                Content = response.Content.ReadAsStringAsync().Result,
                StatusCode = response.StatusCode,
                IsSuccessStatusCode = response.IsSuccessStatusCode,
            };
        }

        //    WebRequestHandler webRequestHandler = new WebRequestHandler();
        //    webRequestHandler.Proxy = new WebProxy("http://mataquito.cl:8080", true, new string[] { }, new NetworkCredential("flarag", "Paulette.02"));// new PolluxProxy("http://mataquito.cl",8080,"flarag","Paulette.02");
        //    webRequestHandler.UseProxy = true;
        //    //webRequestHandler.ClientCertificates.Add(new X509Certificate2(@"E:\Documentos\Respaldo Gonzalo\Pershing\Fase 1\keystore\Qa\pershing.pem"));

        //    HttpClient client = new HttpClient(webRequestHandler);
        //    //var requestContent = new StringContent("", System.Text.Encoding.UTF8, "application/json");
        //    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://apigateway.qa.bnymellon.com/download-investor-documents/v2/criteria-fields/");
        //    //request.Content = requestContent;
        //    //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

        //    client.DefaultRequestHeaders.Clear();
        //    ////HttpRequestMessage request = new HttpRequestMessage();
        //    client.DefaultRequestHeaders.Add("Accept", "application/json");
        //    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate");
        //    client.DefaultRequestHeaders.Add("User-Agent", "Apache-HttpClient/4.1.1 (java 1.5)");

        //    client.DefaultRequestHeaders.Add("Authorization", "Bearer a973e0c7e4de593b2e8d3749f180343c");
        //    HttpResponseMessage response = client.GetAsync("https://apigateway.qa.bnymellon.com/download-investor-documents/v2/criteria-fields", HttpCompletionOption.ResponseContentRead).Result;
        //    response.EnsureSuccessStatusCode();

        //    //var responseBody = client.SendAsync(request).Result.Content.ReadAsByteArrayAsync().Result;
        //    var responseBody = response.Content.ReadAsByteArrayAsync().Result;
        //    string s = System.Text.Encoding.UTF8.GetString(responseBody);
        //    Console.WriteLine(s);


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
    }
}
