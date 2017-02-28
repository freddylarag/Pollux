using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    public class Parameters
    {
        public Parameters(string[] args)
        {
            Workspace = Parse(args, "/Workspace:");
            if (string.IsNullOrWhiteSpace(Workspace))
            {
                Workspace = Path.GetFullPath(@".");
            }

            string[] xml = System.IO.Directory.GetFiles(Workspace,"*.*",SearchOption.AllDirectories).Where(t => t.ToLower().Contains((".xml").ToLower())).ToArray();
            string[] xlsx = System.IO.Directory.GetFiles(Workspace, "*.*", SearchOption.AllDirectories).Where(t => t.ToLower().Contains((".xlsx").ToLower())).ToArray();
            string[] xls = System.IO.Directory.GetFiles(Workspace, "*.*", SearchOption.AllDirectories).Where(t => t.ToLower().Contains((".xls").ToLower())).ToArray();
            string[] config = System.IO.Directory.GetFiles(Workspace, "*.*", SearchOption.AllDirectories).Where(t => t.ToLower().Contains((".config").ToLower())).ToArray();
            ProcessFiles = new List<ProcessFile>();

            if (xml.Length > 0)
            {
                foreach (var item in xml)
                {
                    if (item.IndexOf("/.svn/") < 0)
                    {
                        string fileXlsx = xlsx.FirstOrDefault(x => x.Replace(".xlsx", ".xml").ToUpper() == item.ToUpper());
                        string fileXls = xls.FirstOrDefault(x => x.Replace(".xls", ".xml").ToUpper() == item.ToUpper());
                        string fileConfig = config.FirstOrDefault(x => x.Replace(".config", ".xml").ToUpper() == item.ToUpper());
                        if (!string.IsNullOrWhiteSpace(fileXlsx))
                        {
                            ProcessFiles.Add(new ProcessFile() { FileTemplate = item, FileData = fileXlsx, FileConfig = fileConfig });
                        }
                        else if (!string.IsNullOrWhiteSpace(fileXls))
                        {
                            ProcessFiles.Add(new ProcessFile() { FileTemplate = item, FileData = fileXls, FileConfig = fileConfig });
                        }
                    }
                }
            }
        }

        public string Workspace { get; set; }
        public List<ProcessFile> ProcessFiles { get; set; }

        private string Parse(string[] args, string nombreParametro)
        {

            string valor = "";
            try
            {
                valor = args.Where(x => x.Contains(nombreParametro)).SingleOrDefault();
                if (valor == null)
                {
                    valor = "";
                }
                else
                {
                    valor = valor.Replace(nombreParametro, "").ToUpper();
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error al ejecutar la consola: {0}", ex);
            }
            return valor;
        }

    }
}
