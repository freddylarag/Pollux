using LinqToExcel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    class Program
    {
        static string  workspace=string.Empty;
        static List<string> resumen = new List<string>();
        static List<string[]> resumenCasosPrueba = new List<string[]>();

        static void Main(string[] args)
        {
            //    WebRequestHandler webRequestHandler = new WebRequestHandler();
            //    webRequestHandler.Proxy = new WebProxy("http://mataquito.ing.cl:8080", true, new string[] { }, new NetworkCredential("flarag", "Paulette.02"));// new PolluxProxy("http://mataquito.ing.cl",8080,"flarag","Paulette.02");
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




            try
            {
                Console.WriteLine("\n");
                Console.WriteLine("============================================================");
                Console.WriteLine("                 Pollux v0.4     28/02/2017                 ");
                Console.WriteLine("Automatización de casos de prueba para servicios SOAP y REST");
                Console.WriteLine("============================================================");
                Console.WriteLine();

                var input = new Parameters(args);
                Console.WriteLine("Espacio de trabajo:\n{0}", input.Workspace);
                workspace = input.Workspace;

                if (input.ProcessFiles.Count == 0)
                {
                    Console.WriteLine("\n************ PRECAUCION ************");
                    Console.WriteLine("No existen archivos que procesar.");
                }
                else
                {
                    Procesar(input);

                    if (resumen.Count > 0)
                    {
                        Console.WriteLine("\nResumen de errores detectados:");
                        foreach (var item in resumen)
                        {
                            Console.WriteLine(item);
                        }
                    }
                    if (resumenCasosPrueba.Count > 0)
                    {
                        Console.WriteLine("\nCasos de Prueba:");
                        foreach (var item in resumenCasosPrueba)
                        {
                            foreach (var subitem in item)
                            {
                                Console.WriteLine(subitem);
                                if (subitem.IndexOf("error", StringComparison.InvariantCultureIgnoreCase) >= 1)
                                {
                                    Environment.ExitCode = 1;
                                }
                            }
                        }

                    }
                }

                Console.WriteLine("\n");
                Console.WriteLine("====================================================================");
                Console.WriteLine("                         PROCESO FINALIZADO                         ");
                Console.WriteLine("====================================================================");
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("\nPresione una tecla...");
                Console.ReadKey();
            }

        }

        static void ShowError(Exception ex)
        {
            ConsoleColor colorError = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n");
            Console.WriteLine("====================================================================");
            Console.WriteLine("====================================================================");
            Console.WriteLine("                   PROCESO FINALIZADO CON ERROR                     ");
            Console.WriteLine();
            Console.WriteLine("Error: {0}", ex.Message);
            Console.WriteLine();
            Console.WriteLine("====================================================================");
            Console.WriteLine("====================================================================");
            Environment.ExitCode = 1;
            Console.ForegroundColor = colorError;
        }


        static void Procesar(Parameters input)
        {
            int proceso = 0;
            foreach (var fileItem in input.ProcessFiles)
            {
                proceso++;
                Console.WriteLine("\n-------------------------------- Proceso {0} Iniciado  ({1})--------------------------------", proceso, fileItem.Name);
                try
                {
                    input.Workspace = System.IO.Path.GetDirectoryName(fileItem.FileTemplate);
                    Console.WriteLine("Config:\n{0}", fileItem.FileConfig);
                    Console.WriteLine("Xml:\n{0}", fileItem.FileTemplate);
                    Console.WriteLine("Xls:\n{0}", fileItem.FileData);

                    //Config
                    Console.WriteLine("\nSOAP");
                    fileItem.Config = new Config(fileItem.FileConfig);
                    Console.WriteLine("\tURL: {0}", fileItem.Config.Url);
                    if (fileItem.Config.Headers.Count == 0)
                    {
                        Console.WriteLine("\tError: No existen encabezados soap.");
                    }
                    else
                    {
                        foreach (var item in fileItem.Config.Headers)
                        {
                            Console.WriteLine("\t{0}: {1}", item.Key, item.Value);
                        }
                    }

                    //XML
                    fileItem.Xml = new Xml(fileItem.FileTemplate);
                    if (fileItem.Xml.Fields.Count == 0 
                        && (
                                fileItem.Config.Type == Config.TypeProtocol.SOAP 
                                    || 
                                (fileItem.Config.Type == Config.TypeProtocol.REST 
                                    && 
                                    (fileItem.Config.Method==Config.TypeMethod.POST || fileItem.Config.Method == Config.TypeMethod.POST)
                                )
                            )
                        )
                    {
                        Console.WriteLine("\nCampos detectatos en XML: {0}", fileItem.Xml.Fields.Count);
                        Console.WriteLine("\tError: No existen campos mapeados.");
                    }
                    else
                    {
                        Console.WriteLine("\nCampos detectatos en XML: {0}", fileItem.Xml.Fields.Count);
                        if (fileItem.Xml.Fields.Count == 0)
                        {
                            foreach (var item in fileItem.Xml.Fields)
                            {
                                Console.WriteLine("\t{0}", item);
                            }
                        }else
                        {
                            Console.WriteLine("\tNo existen campos mapeados.");
                        }
                    }
                    

                    //EXCEL
                    fileItem.Excel = new Excel(fileItem.FileData, fileItem.Xml);

                    //Procesar
                    Console.WriteLine("\nEjecución de Casos de Prueba:");
                    resumenCasosPrueba.Add(Soap.Start(input.Workspace, fileItem));
                    Publish.Save(input.Workspace, fileItem);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nERROR:");
                    Console.WriteLine(ex.Message);
                    resumen.Add(string.Format("Proceso {0} ({1})", proceso, fileItem.Name));
                    resumen.Add("\t"+ex.Message);

                    if (ex.InnerException != null)
                    {
                        Console.WriteLine(ex.InnerException.Message);
                        resumen.Add("\t" + ex.InnerException.Message);
                    }
                }
                PublishSummary();
                Console.WriteLine("\n-------------------------------- Proceso {0} Finalizado  ({1})--------------------------------", proceso, fileItem.Name);
            }
        }

        private static void PublishSummary()
        {
            
        }
    }
}
