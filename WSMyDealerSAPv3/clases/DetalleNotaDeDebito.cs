using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class DetalleNotaDeDebito
    {
        public string Wsc { get; set; }
        public string CodigoProducto { get; set; }
        public double CantidadProducto { get; set; }
        public decimal PrecioProducto { get; set; }
        // Agrega cualquier otro campo necesario para el detalle de la nota de débito
        public DetalleNotaDeDebito() { }
        //public DetalleNotaDeDebito(string wsc, string codigoProducto, double cantidad, decimal precio)
        //{
        //    Wsc = wsc;
        //    CodigoProducto = codigoProducto;
        //    CantidadProducto = cantidad;
        //    PrecioProducto = precio;
        //}
    }
}