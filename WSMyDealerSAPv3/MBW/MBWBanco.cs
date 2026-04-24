using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class MBWBanco
    {
        string codbanco;

        public string Codbanco
        {
            get { return codbanco; }
            set { codbanco = value; }
        }
        string descripcion;

        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }
    }
}