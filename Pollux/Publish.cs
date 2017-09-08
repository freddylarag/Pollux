using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    public static class Publish
    {
        private static string FileCss = string.Empty;
        private static DateTime FechaProceso;

        public static void Save(string path, ProcessFile process, DateTime fecha)
        {
            FechaProceso = fecha;

            //Copia el template CSS
            var css = ConfigurationManager.AppSettings["CSS"];
            if (!File.Exists(css))
            {
                css = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, css);
                if (!File.Exists(css))
                {
                    css = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TemplateClassic.css");
                }
            }
            FileCss = Path.GetFileName(css);
            File.Copy(css, Path.Combine(path, FileCss));


            //Crea el html
            PageCasos(Path.Combine(path, "Index.html"), process.CasosNegocio, Pagina.CasosNegocio);
            if (process.CasosNegocio.Config.AutomaticSpecialTest)
            {
                PageCasos(Path.Combine(path, "SpecialTests.html"), process.CasosBorde, Pagina.CasosBorde);
            }
            else
            {
                PageSinCasosBorde(Path.Combine(path, "SpecialTests.html"), process.CasosNegocio);
            }
            PageInfo(Path.Combine(path, "Info.html"), process.CasosNegocio);
        }

        #region Metodos transversales

        enum Pagina
        {
            CasosNegocio,
            CasosBorde,
            Info,
        }

        private static string Styles()
        {
            return string.Format("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\">", FileCss);
        }

        private static string TitleBrowser()
        {
            return string.Format("      <title>{0} - {1}</title>", Program.ApplicationName, Program.ApplicationDescription);
        }

        private static string Title(ProcessFileConfiguration process)
        {
            return "<h1 class=\"center\">" + process.Config.Description + "</h1>";
        }

        private static List<string> NavigationBar(ProcessFileConfiguration process, Pagina pagina)
        {
            List<string> html = new List<string>();

            html.Add("      <ul>");
            html.Add(string.Format("          <li><a {1} href=\"{0}\">Casos de Negocio</a></li>", "Index.html", pagina==Pagina.CasosNegocio ? "class=\"active\"" : ""));
            html.Add(string.Format("          <li><a {1} href=\"{0}\">Casos de Borde</a></li>", "SpecialTests.html", pagina == Pagina.CasosBorde ? "class=\"active\"" : ""));
            html.Add(string.Format("          <li><a {1} href=\"{0}\">Información de Servicio</a></li>", "Info.html", pagina == Pagina.Info ? "class=\"active\"" : ""));
            html.Add("          <li style=\"float:right\"><a href=\"#\">Acerca de " + Program.ApplicationName + "</a></li>");
            html.Add("      </ul>");

            return html;
        }

        #endregion

        #region Pagina Principal de hallazgos

        private static void PageCasos(string path, ProcessFileConfiguration process, Pagina pagina)
        {
            List<string> html = new List<string>();
            html.Add("<html>");
            html.Add("  <header>");
            html.Add(TitleBrowser());
            html.Add(Styles());
            html.Add("  </header>");
            html.Add("  <body>");
            html.AddRange(NavigationBar(process, pagina));
            html.Add("      <div class=\"content\">");
            html.Add(Title(process));
            html.AddRange(ResumenCasos(process));
            html.AddRange(Body(process, path));
            html.Add("      </div>");
            html.Add("  </body>");
            html.Add("</html>");
            File.WriteAllLines(path, html.ToArray(), Encoding.UTF8);
        }

        private static List<string> ResumenCasos(ProcessFileConfiguration process)
        {
            List<string> html = new List<string>();

            html.Add("<table>");
            html.Add("  <tr>");

            var estado = process.Excel.RequestXml.Where(x => !x.IsCorrect).Count() > 0 ? process.Excel.RequestXml.FirstOrDefault(x => !x.IsCorrect) : process.Excel.RequestXml.FirstOrDefault(x => x.IsCorrect);
            if (estado.IsCorrect)
            {
                html.Add("<td>");
                html.Add(string.Format("Estado: <b class=\"labelSucess\">{0}</b>", estado.Status));
                html.Add("</td>");
            }
            else
            {
                html.Add("<td>");
                html.Add(string.Format("Estado: <b class=\"labelDanger\">{0}</b>", estado.Status));
                html.Add("</td>");
            }

            html.Add(string.Format("      <td>Casos Ejecutados: <b>{0}</b></td>", process.Excel.RequestXml.Count));
            html.Add(string.Format("      <td>Casos Correctos: <b>{0}</b></td>", process.Excel.RequestXml.Where(x => x.IsCorrect).Count()));
            html.Add(string.Format("      <td>Casos Erroneos: <b>{0}</b></td>", process.Excel.RequestXml.Where(x => !x.IsCorrect).Count()));
            html.Add(string.Format("      <td>Fecha Ejecución: <b>{0}</b></td>", FechaProceso.ToString("dd/MM/yyyy HH:mm:ss")));
            html.Add("  </tr>");
            html.Add("<table>");
            html.Add("<br/><br/>");

            return html;
        }

        private static List<string> Body(ProcessFileConfiguration process, string path)
        {
            List<string> html = new List<string>();
            html.Add("<table>");

            //Encabezado
            html.Add("<tr>");
            html.Add(string.Format("<td><b>#</b></td>"));
            html.Add(string.Format("<td><b>Campos</b></td>"));
            html.Add(string.Format("<td><b>Tipo Dato</b></td>"));
            if (process.Excel?.Fields?.Count > 0)
            {
                for (var i = 0; i < process.Excel.Fields.First().Value.Count; i++)
                {
                    html.Add(string.Format("<th class=\"panelInfo\">Caso {0}</th>", i + 1));
                }
            }else
            {
                html.Add(string.Format("<th class=\"panelInfo\">Caso {0}</th>", 1));
            }
            html.Add("</tr>");

            //Datos
            if (process.Excel?.Fields?.Count > 0)
            {
                int j = 0;
                foreach (var item in process.Excel.Fields)
                {
                    j++;
                    html.Add("<tr>");
                    html.Add(string.Format("<td style='text-align: center;'>{0}</td>", j));
                    html.Add(string.Format("<th class=\"panelInfo left\">{0}</th>", item.Key));
                    html.Add(string.Format("<th class=\"panelInfo left\">{0}</th>", item.Value.FirstOrDefault()?.ToString()));
                    foreach (var caso in item.Value)
                    {
                        if (caso.IsTester)
                        {
                            html.Add(string.Format("<td class=\"cellTester\">{0}</td>", SpecialKey(caso)));
                        }
                        else
                        {
                            html.Add(string.Format("<td>{0}</td>", SpecialKey(caso)));
                        }
                    }
                    html.Add("</tr>");
                }
            }else
            {
                html.Add("<tr>");
                html.Add(string.Format("<td style='text-align: center;'>{0}</td>", 1));
                html.Add(string.Format("<th class=\"panelInfo left\">{0}</th>", ""));
                html.Add(string.Format("<td>{0}</td>", "No existen datos de prueba."));
                html.Add("</tr>");
            }

            //Evidencia de request
            html.Add("<tr>");
            html.Add(string.Format("<td style=\"text-align: center\" colspan=\"3\">Evidencia</td>"));
            foreach (var item in process.Excel.RequestXml)
            {
                html.Add("<td>");
                html.Add(string.Format("<a href=\"{0}\" target=\"_blank\"><button class=\"button buttonInfo fill slim\">Request</button></a>", item.Request.RelativePath(path)));
                if (item.IsCorrect)
                {
                    html.Add(string.Format("<a href=\"{0}\" target=\"_blank\"><button class=\"button buttonSucess fill slim\">Response</button></a>", item.Response.RelativePath(path)));
                }
                else
                {
                    html.Add(string.Format("<a href=\"{0}\" target=\"_blank\"><button class=\"button buttonDanger fill slim\">Response</button></a>", item.Response.RelativePath(path)));
                }
                html.Add("</td>");
            }
            html.Add("</tr>");

            //StatusCode
            html.Add("<tr>");
            html.Add(string.Format("<td style=\"text-align: center\" colspan=\"3\">StatusCode</td>"));
            foreach (var item in process.Excel.RequestXml)
            {
                html.Add("<td>");
                html.Add(string.Format("{0} ({1})", item.Response.StatusCode, (int)item.Response.StatusCode));
                html.Add("</td>");
            }
            html.Add("</tr>");

            //TimeOut
            html.Add("<tr>");
            html.Add(string.Format("<td style=\"text-align: center\" colspan=\"3\">TimeOut</td>"));
            foreach (var item in process.Excel.RequestXml)
            {
                html.Add("<td>");
                html.Add(string.Format("{0}", item.TimeOut));
                html.Add("</td>");
            }
            html.Add("</tr>");

            //Status
            html.Add("<tr>");
            html.Add(string.Format("<td style=\"text-align: center\" colspan=\"3\">Estado</td>"));
            foreach (var item in process.Excel.RequestXml)
            {
                if (item.IsCorrect)
                {
                    html.Add("<td class=\"panelSucess\">");
                    html.Add(string.Format("<b>{0}</b>", item.Status));
                    html.Add("</td>");
                }
                else
                {
                    html.Add("<td class=\"panelDanger\">");
                    html.Add(string.Format("<b>{0}</b>", item.Status));
                    html.Add("</td>");
                }
            }
            html.Add("</tr>");

            //Fin
            html.Add("</table>");

            return html;
        }

        private static string SpecialKey(ExcelField value)
        {
            if (value.Value == "" || value.Value.Trim().Equals(Excel.KeyEmpty, StringComparison.InvariantCultureIgnoreCase))
            {
                return "<div class=\"empty\">EMPTY</div>";
            }
            if (value.Value.Trim().Equals(Excel.KeyNull, StringComparison.InvariantCultureIgnoreCase))
            {
                return "<div class=\"null\">NULL</div>";
            }

            return value.Value;
        }

        #endregion

        #region Pagina de Pruebas de borde

        private static void PageSinCasosBorde(string path, ProcessFileConfiguration process)
        {
            List<string> html = new List<string>();

            html.Add("<html>");
            html.Add("  <header>");
            html.Add(TitleBrowser());
            html.Add(Styles());
            html.Add("  </header>");
            html.Add("  <body>");
            html.AddRange(NavigationBar(process, Pagina.CasosBorde));
            html.Add("      <div class=\"content\">");
            html.Add(Title(process));
            html.Add("          <h1>Los casos de borde fueron desactivados por configuración (AutomaticSpecialTest=False).</h1>");
            html.Add("      </div>");
            html.Add("  </body>");
            html.Add("</html>");

            File.WriteAllLines(path, html.ToArray(), Encoding.UTF8);
        }

        #endregion

        #region Pagina Informacion

        private static void PageInfo(string path, ProcessFileConfiguration process)
        {
            List<string> html = new List<string>();


            html.Add("<html>");
            html.Add("  <header>");
            html.Add(TitleBrowser());
            html.Add(Styles());
            html.Add("  </header>");
            html.Add("  <body>");
            html.AddRange(NavigationBar(process, Pagina.Info));
            html.Add("      <div class=\"content\">");
            html.Add(Title(process));

            //Configuraciones
            html.Add("<table>");
            html.Add("  <tr>");
            html.Add(string.Format("      <th class=\"panelInfo\" colspan=\"2\">Configuraciones</th>"));
            html.Add("  </tr>");
            html.Add("  <tr>");
            html.Add(string.Format("      <th class=\"panelInfo\">Campo</th>"));
            html.Add(string.Format("      <th class=\"panelInfo\">Valor</th>"));
            html.Add("  </tr>");

            html.Add("  <tr>");
            html.Add(string.Format("      <td>Description</td><td>{0}</td>", process.Config.Description));
            html.Add("  </tr>");

            html.Add("  <tr>");
            html.Add(string.Format("      <td>AutomaticSpecialTest</td><td>{0}</td>", process.Config.AutomaticSpecialTest));
            html.Add("  </tr>");

            html.Add("<table>");

            html.Add("<br/><br/>");

            
            //header
            html.Add("<table>");
            html.Add("  <tr>");
            html.Add(string.Format("      <th class=\"panelInfo\" colspan=\"2\">Header</th>"));
            html.Add("  </tr>");
            html.Add("  <tr>");
            html.Add(string.Format("      <th class=\"panelInfo\">Campo</th>"));
            html.Add(string.Format("      <th class=\"panelInfo\">Valor</th>"));
            html.Add("  </tr>");
            html.Add("  <tr>");
            html.Add(string.Format("      <td>URL</td><td>{0}</td>", process.Config.Url));
            html.Add("  </tr>");
            foreach (var item in process.Config.Headers)
            {
                html.Add("  <tr>");
                html.Add(string.Format("      <td>{0}</td><td>{1}</td>", item.Key, item.Value));
                html.Add("  </tr>");
            }
            html.Add("<table>");

            html.Add("<br/><br/>");

            //Validaciones
            html.AddRange(ValidationSections("Colección de validaciones para Header", process.Config.Validations.ValidationsHeader));
            html.AddRange(ValidationSections("Colección de validaciones para Body", process.Config.Validations.ValidationsBody));
            html.AddRange(ValidationSections("Colección de validaciones para Fault", process.Config.Validations.ValidationsFault));

            html.Add("      </div>");
            html.Add("  </body>");
            html.Add("</html>");

            File.WriteAllLines(path, html.ToArray(), Encoding.UTF8);
        }

        private static IEnumerable<string> ValidationSections(string title, List<Validation> validations)
        {
            List<string> html = new List<string>();
            if (validations?.Count > 0)
            {
                html.Add("<table>");
                html.Add("  <tr>");
                html.Add(string.Format("      <th class=\"panelInfo\" colspan=\"3\">{0}</th>", title));
                html.Add("  </tr>");
                html.Add("  <tr>");
                html.Add(string.Format("      <th class=\"panelInfo\">Tag</th>"));
                html.Add(string.Format("      <th class=\"panelInfo\">Operación</th>"));
                html.Add(string.Format("      <th class=\"panelInfo\">Valor</th>"));
                html.Add("  </tr>");
                foreach (var item in validations)
                {
                    html.Add("  <tr>");
                    if (item.Values.Count == 1)
                    {
                        html.Add(string.Format("      <td>{0}</td><td>{1}</td><td>{2}</td>", item.Tag, item.Operation, item.Values.FirstOrDefault()));
                    }
                    else
                    {
                        var values = string.Format("      <td>{0}</td><td>{1}</td><td>", item.Tag, item.Operation);
                        foreach (var itemValue in item.Values)
                        {
                            values = values + itemValue + "<br/>";
                        }
                        values = values + "</td>";
                        html.Add(values);
                    }
                    html.Add("  </tr>");
                }
                html.Add("</table>");
                html.Add("<br/><br/>");
            }
            return html;
        }

        #endregion

    }
}
