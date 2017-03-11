using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServicesTest.Entities
{
    [Serializable]
    public class Producto
    {
        public Guid ProductoID { get; set; }
        public string Nombre { get; set; }
        public string SKU { get; set; }
        public short Categoria { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
}