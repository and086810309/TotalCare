using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class MBWPrecios
    {
        string codproducto;

        public string CodProducto
        {
            get { return codproducto; }
            set { codproducto = value; }
        }

        string codtipocliente;

        public string CodTipoCliente
        {
            get { return codtipocliente; }
            set { codtipocliente = value; }
        }
        double precio;

        public double Precio
        {
            get { return precio; }
            set { precio = value; }
        }
    }
}
