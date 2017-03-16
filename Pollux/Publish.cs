using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    public static class Publish
    {
        private static string Styles()
        {
            string style = @"
    <style>
        body {
            font-family: sansation;
            font-size: 16px;
	        margin: 0;
  	        padding: 0;
        }

        .content{
  	        padding: 10;
        }

        table {
            border-collapse: collapse;
            width: 100%;
        }

        table, th, td {
            border: 1px solid #ddd;
        }

        th {
            height: 40px;background-color: #4CAF50;
            color: white;
        }

        tr:hover {
            background-color: #f5f5f5
        }

        th, td {
            padding: 10px; 
            text-align: left;
        }

        .button {
            background-color: #4CAF50; /* Green */
            border: none;
            color: white;
            padding: 16px 32px;
            text-align: center;
            text-decoration: none;
            display: inline-block;
            font-size: 16px;
            margin: 4px 2px;
            -webkit-transition-duration: 0.4s; /* Safari */
            transition-duration: 0.4s;
            cursor: pointer;
        }

        .fill{
            width: 100%;
        }
        
        .slim{
            padding: 5px 32px;
        }

        .buttonSucess {
            background-color: white; 
            color: black; 
            border: 2px solid #4CAF50;
        }

        .buttonSucess:hover {
            background-color: #4CAF50;
            color: white;
        }

        .buttonInfo {
            background-color: white; 
            color: black; 
            border: 2px solid #008CBA;
        }

        .buttonInfo:hover {
            background-color: #008CBA;
            color: white;
        }

        .buttonDanger {
            background-color: white; 
            color: black; 
            border: 2px solid #f44336;
        }

        .buttonDanger:hover {
            background-color: #f44336;
            color: white;
        }

        .button4 {
            background-color: white;
            color: black;
            border: 2px solid #e7e7e7;
        }

        .button4:hover {background-color: #e7e7e7;}

        .button5 {
            background-color: white;
            color: black;
            border: 2px solid #555555;
        }

        .button5:hover {
            background-color: #555555;
            color: white;
            text-align: center;
        }

        .panelDanger{
            background-color: #f44336;
            color: white;
            text-align: center;
        }

        .panelInfo {
            background-color: #008CBA;
            color: white;
            text-align: center;
        }

        .panelSucess {
            background-color: #4CAF50;
            color: white;
            text-align: center;
        }

        .center{
            text-align: center;
        }
        .left{
            text-align: left;
        }
        .right{
            text-align: right;
        }

        /*Inicio Navigation Bar*/
        ul {
            list-style-type: none;
            margin: 0;
            padding: 0;
            overflow: hidden;
            background-color: #333;
        }

        li {
            float: left;
            border-right: 1px solid #bbb;
        }

        li a {
            display: block;
            color: white;
            text-align: center;
            padding: 14px 16px;
            text-decoration: none;
        }

        /* Change the link color to #111 (black) on hover */
        li a:hover:not(.active) {
            background-color: #111;
        }
        .active {
            background-color: #4CAF50;
        }
        /*Fin Navigation Bar*/

</style>
";
            return style;
        }

        public static void Save(string path, ProcessFile process){
            List<string> html = new List<string>();
            html.Add("<html>");
            html.Add("  <header>");
            html.Add("      <title>Automatización de pruebas - Pollux</title>");
            html.Add(Styles());
            html.Add("  </header>");
            html.Add("  <body>");
            html.AddRange(NavigationBar(process, Pagina.CasosNegocio));
            html.Add("      <div class=\"content\">");
            html.Add(Title(process));
            html.AddRange(Body(process, path));
            html.Add("      </div>");
            html.Add("  </body>");
            html.Add("</html>");
            File.WriteAllLines(Path.Combine(path, process.Name+".html"), html.ToArray(), Encoding.UTF8);

            Info(path, process,Pagina.Info);
            PruebasBorde(path, process,Pagina.CasosBorde);
        }

        private static string Title(ProcessFile process)
        {
            return "<h1 class=\"center\">" + process.Config.Description + "</h1>";
        }

        enum Pagina{
            CasosNegocio,
            CasosBorde,
            Info,
        }
        private static List<string> NavigationBar(ProcessFile process, Pagina pagina)
        {
            List<string> html = new List<string>();

            html.Add("      <ul>");
            html.Add(string.Format("          <li><a {1} href=\"{0}\">Casos de Negocio</a></li>", process.Name + ".html", pagina==Pagina.CasosNegocio ? "class=\"active\"" : ""));
            html.Add(string.Format("          <li><a {1} href=\"{0}\">Casos de Borde</a></li>", process.Name + "_borde.html", pagina == Pagina.CasosBorde ? "class=\"active\"" : ""));
            html.Add(string.Format("          <li><a {1} href=\"{0}\">Información de Servicio</a></li>", process.Name + "_info.html", pagina == Pagina.Info ? "class=\"active\"" : ""));
            html.Add("          <li style=\"float:right\"><a href=\"#\">Acerca de Pollux</a></li>");
            html.Add("      </ul>");

            return html;
        }

        private static void PruebasBorde(string path, ProcessFile process, Pagina pagina)
        {
            List<string> html = new List<string>();

            html.Add("<html>");
            html.Add("  <header>");
            html.Add("      <title>Automatización de pruebas - Pollux</title>");
            html.Add(Styles());
            html.Add("  </header>");
            html.Add("  <body>");
            html.AddRange(NavigationBar(process,pagina));
            html.Add("      <div class=\"content\">");
            html.Add(Title(process));
            html.Add("          <h1>En construcción</h1>");
            html.Add("      </div>");
            html.Add("  </body>");
            html.Add("</html>");

            File.WriteAllLines(Path.Combine(path, process.Name + "_borde.html"), html.ToArray(), Encoding.UTF8);
        }
        private static void Info(string path, ProcessFile process, Pagina pagina)
        {
            List<string> html = new List<string>();


            html.Add("<html>");
            html.Add("  <header>");
            html.Add("      <title>Automatización de pruebas - Pollux</title>");
            html.Add(Styles());
            html.Add("  </header>");
            html.Add("  <body>");
            html.AddRange(NavigationBar(process,pagina));
            html.Add("      <div class=\"content\">");
            html.Add(Title(process));

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
            html.Add(string.Format("      <td>URL</td><td>{0}</td>",process.Config.Url));
            html.Add("  </tr>");
            foreach (var item in process.Config.Headers)
            {
                html.Add("  <tr>");
                html.Add(string.Format("      <td>{0}</td><td>{1}</td>", item.Key,item.Value));
                html.Add("  </tr>");
            }
            html.Add("<table>");

            html.Add("<br/><br/>");

            //Validaciones
            html.Add("<table>");
            html.Add("  <tr>");
            html.Add(string.Format("      <th class=\"panelInfo\" colspan=\"3\">Validaciones</th>"));
            html.Add("  </tr>");
            html.Add("  <tr>");
            html.Add(string.Format("      <th class=\"panelInfo\">Tag</th>"));
            html.Add(string.Format("      <th class=\"panelInfo\">Operación</th>"));
            html.Add(string.Format("      <th class=\"panelInfo\">Valor</th>"));
            html.Add("  </tr>");
            foreach (var item in process.Config.Validations)
            {
                html.Add("  <tr>");
                if (item.Values.Count == 1)
                {
                    html.Add(string.Format("      <td>{0}</td><td>{1}</td><td>{2}</td>", item.Tag, item.Operation, item.Values));
                }else
                {
                    var values=string.Format("      <td>{0}</td><td>{1}</td><td>", item.Tag, item.Operation);
                    foreach (var itemValue in item.Values)
                    {
                        values= values + itemValue + "<br/>";
                    }
                    values = values + "</td>";
                    html.Add(values);
                }
                html.Add("  </tr>");
            }
            html.Add("</table>");

            html.Add("      </div>");
            html.Add("  </body>");
            html.Add("</html>");

            File.WriteAllLines(Path.Combine(path, process.Name + "_info.html"), html.ToArray(), Encoding.UTF8);
        }

        private static List<string> Body(ProcessFile process, string path)
        {
            List<string> html = new List<string>();

            html.Add("<table>");


            //Encabezado
            html.Add("<tr>");
            html.Add(string.Format("<td>#</td>"));
            html.Add(string.Format("<td>Campos</td>"));
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
                    foreach (var caso in item.Value)
                    {
                        html.Add(string.Format("<td>{0}</td>", caso));
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
            html.Add(string.Format("<td></td><td style=\"text-align: center\">Evidencia</td>"));
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

            //TimeOut
            html.Add("<tr>");
            html.Add(string.Format("<td></td><td style=\"text-align: center\">StatusCode</td>"));
            foreach (var item in process.Excel.RequestXml)
            {
                html.Add("<td>");
                html.Add(string.Format("{0} ({1})", item.Response.StatusCode, (int)item.Response.StatusCode));
                html.Add("</td>");
            }
            html.Add("</tr>");

            //TimeOut
            html.Add("<tr>");
            html.Add(string.Format("<td></td><td style=\"text-align: center\">TimeOut</td>"));
            foreach (var item in process.Excel.RequestXml)
            {
                html.Add("<td>");
                html.Add(string.Format("{0}", item.TimeOut));
                html.Add("</td>");
            }
            html.Add("</tr>");

            //Status
            html.Add("<tr>");
            html.Add(string.Format("<td></td><td style=\"text-align: center\">Estado</td>"));
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
    }
}
