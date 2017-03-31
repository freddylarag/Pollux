using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using WebServicesTest.Entities;

namespace WebServicesTest
{
    /// <summary>
    /// Summary description for WebServiceTest1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceTest1 : System.Web.Services.WebService
    {

        [WebMethod]
        public Producto ObtenerProducto(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
            {
                throw new ArgumentNullException(nameof(sku));
            }

            return Productos().Where(x => x.SKU == sku).FirstOrDefault();
        }

        [WebMethod]
        public List<Producto> ObtenerProductos()
        {
            return Productos();
        }

        [WebMethod]
        public Guid InsertarProducto(Producto producto)
        {
            return Guid.NewGuid();
        }


        private List<Producto> Productos()
        {
            List<Producto> lista = new List<Producto>() {
                 new Producto
                    {
                        Nombre = "Cuaderno Matematicas 100 Hojas",
                        Categoria = 70,
                        Precio = 1200,
                        ProductoID = Guid.Parse("625ff30e-4392-4af5-b119-1a2c59940e8d"),
                        SKU = "HJ5KS9876234DD340",
                        Stock = 200,
                    },
                 new Producto
                    {
                        Nombre = "Cuaderno Caligrafia 60 Hojas",
                        Categoria = 67,
                        Precio = 910,
                        ProductoID = Guid.Parse("d93d1823-d0b7-4721-b30b-2181ca41a69d"),
                        SKU = "HJ5KS9876234DD3431",
                        Stock = 360,
                    },
                 new Producto
                    {
                        Nombre = "Cuaderno Matematicas 150 Hojas",
                        Categoria = 70,
                        Precio = 1500,
                        ProductoID = Guid.Parse("c973dd5d-bef4-4894-982d-2ea62a5708cc"),
                        SKU = "HJ5KS9876234DD342",
                        Stock = 500,
                    },
            };

            return lista;
        }
    }
}
