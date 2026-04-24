using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class MBWBodegas
    {
        string codbodega;

        public string Codbodega
        {
            get { return codbodega; }
            set { codbodega = value; }
        }
        string descripcion;

        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }

        String montoMinimo;
        public string MontoMinimo
        {
            get { return montoMinimo; }
            set { montoMinimo = value; }
        }
    }
}