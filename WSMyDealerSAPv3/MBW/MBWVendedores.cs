using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class MBWVendedores
    {
        int codvendedor;

        public int Codvendedor
        {
            get { return codvendedor; }
            set { codvendedor = value; }
        }
        string descripcion;

        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }

        string codbodegadef;

        public string Codbodegadef
        {
            get { return codbodegadef; }
            set { codbodegadef = value; }
        }
        string codbodegaalt;

        public string Codbodegaalt
        {
            get { return codbodegaalt; }
            set { codbodegaalt = value; }
        }
    }
}