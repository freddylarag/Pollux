using System;
using System.Collections.Generic;
using System.IO;

namespace Pollux
{
    class Program
    {
        static string  workspace=string.Empty;
        static List<string> resumen = new List<string>();
        static List<string[]> resumenCasosPrueba = new List<string[]>();
        static List<string[]> resumenCasosBorde = new List<string[]>();
        public static string ApplicationName = "Pollux v0.17 Beta";
        public static string ApplicationDescription = "Automatización de casos de prueba para servicios SOAP y REST";
        public static string ApplicationBuild = "build 09/04/2017";
        public static string Autor = "Freddy Lara - freddylarag@gmail.com";

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("\n");
                Console.WriteLine("====================================================================");
                Console.WriteLine("                   {0}        {1}              ",ApplicationName, ApplicationBuild);
                Console.WriteLine("    {0}    ", ApplicationDescription);
                Console.WriteLine("                {0}",Autor);
                Console.WriteLine("====================================================================");
                Console.WriteLine();

                var input = new Parameters(args);
                Console.WriteLine("Espacio de trabajo:\n{0}", input.Workspace);
                workspace = input.Workspace;

                if (input.Init)
                {
                    string plantilla = Path.Combine(input.Workspace, Guid.NewGuid().ToString() + ".config");
                    Config.CreateTemplate(plantilla);
                    Console.WriteLine("\nPlantilla creada:\n{0}", plantilla);
                }
                else
                {
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
                Console.WriteLine("\n-------------------------------- Proceso {0} Iniciado  ({1})--------------------------------", proceso, fileItem.CasosNegocio.Name);
                try
                {
                    ProcesarCasos(input, fileItem);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nERROR:");
                    Console.WriteLine(ex.Message);
                    resumen.Add(string.Format("Proceso {0} ({1})", proceso, fileItem.CasosNegocio.Name));
                    resumen.Add("\t"+ex.Message);

                    if (ex.InnerException != null)
                    {
                        Console.WriteLine(ex.InnerException.Message);
                        resumen.Add("\t" + ex.InnerException.Message);
                    }
                }
                PublishSummary();
                Console.WriteLine("\n-------------------------------- Proceso {0} Finalizado  ({1})--------------------------------", proceso, fileItem.CasosNegocio.Name);
            }
        }

        private static void ProcesarCasos(Parameters input, ProcessFile fileItem)
        {
            input.Workspace = System.IO.Path.GetDirectoryName(fileItem.CasosNegocio.FileTemplate);
            Console.WriteLine("Config:\t{0}", fileItem.CasosNegocio.FileConfig.Replace(input.Workspace, ""));
            Console.WriteLine("Xml:\t{0}", fileItem.CasosNegocio.FileTemplate.Replace(input.Workspace, ""));
            Console.WriteLine("Xls:\t{0}", fileItem.CasosNegocio.FileData.Replace(input.Workspace, ""));

            //Config
            Console.WriteLine("\nSOAP");
            fileItem.CasosNegocio.Config = new Config(fileItem.CasosNegocio.FileConfig, input.Url);
            Console.WriteLine("\tURL: {0}", fileItem.CasosNegocio.Config.Url);
            if (fileItem.CasosNegocio.Config.Headers.Count == 0)
            {
                Console.WriteLine("\tError: No existen encabezados soap.");
            }
            else
            {
                foreach (var item in fileItem.CasosNegocio.Config.Headers)
                {
                    Console.WriteLine("\t{0}: {1}", item.Key, item.Value);
                }
            }

            //XML
            fileItem.CasosNegocio.Xml = new Xml(fileItem.CasosNegocio.FileTemplate);
            if (fileItem.CasosNegocio.Xml.Fields.Count == 0
                && (
                        fileItem.CasosNegocio.Config.Type == Config.TypeProtocol.SOAP
                            ||
                        (fileItem.CasosNegocio.Config.Type == Config.TypeProtocol.REST
                            &&
                            (fileItem.CasosNegocio.Config.Method == Config.TypeMethod.POST || fileItem.CasosNegocio.Config.Method == Config.TypeMethod.POST)
                        )
                    )
                )
            {
                Console.WriteLine("\nCampos detectatos en XML: {0}", fileItem.CasosNegocio.Xml.Fields.Count);
                Console.WriteLine("\tError: No existen campos mapeados.");
            }
            else
            {
                Console.WriteLine("\nCampos detectatos en XML: {0}", fileItem.CasosNegocio.Xml.Fields.Count);
                if (fileItem.CasosNegocio.Xml.Fields.Count > 0)
                {
                    foreach (var item in fileItem.CasosNegocio.Xml.Fields)
                    {
                        Console.WriteLine("\t{0}", item);
                    }
                }
                else
                {
                    Console.WriteLine("\tNo existen campos mapeados.");
                }
            }

            var fecha = DateTime.Now;
            string fechaEjecucion = fecha.ToString("yyyyMMdd_HHmmss");

            //Procesar casos de negocio
            Console.WriteLine("\nEjecución de Casos de Prueba:");
            fileItem.CasosNegocio.Excel = new Excel(fileItem.CasosNegocio.FileData, fileItem.CasosNegocio.Xml, 2);
            resumenCasosPrueba.Add(SoapManager.Start(Path.Combine(input.Workspace, "Reports", $"{fileItem.CasosNegocio.Name}_{fechaEjecucion}","Results_Basics"), fileItem.CasosNegocio));

            if (fileItem.CasosNegocio.Config.AutomaticSpecialTest)
            {
                //Procesar casos de borde
                Console.WriteLine("\nEjecución de Casos de Borde:");
                fileItem.CasosBorde.Config = fileItem.CasosNegocio.Config;
                fileItem.CasosBorde.Xml = fileItem.CasosNegocio.Xml;
                fileItem.CasosBorde.FileData = fileItem.CasosNegocio.FileData;
                fileItem.CasosBorde.Excel = new ExcelCasoBorde(fileItem.CasosNegocio.Excel.Fields, fileItem.CasosBorde.Xml);
                resumenCasosBorde.Add(SoapManager.Start(Path.Combine(input.Workspace, "Reports", $"{fileItem.CasosNegocio.Name}_{fechaEjecucion}", "Results_Specials"), fileItem.CasosBorde));
            }

            //Publicar informe
            Publish.Save(Path.Combine(input.Workspace, "Reports", $"{fileItem.CasosNegocio.Name}_{fechaEjecucion}"), fileItem, fecha);
        }

        private static void PublishSummary()
        {
            
        }
    }
}
