using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class MBWDescuentos
    {
        int _codtipocliente;
        public int Codtipocliente
        {
            get { return _codtipocliente; }
            set { _codtipocliente = value; }
        }

        int _codtipoproducto;
        public int Codtipoproducto
        {
            get { return _codtipoproducto; }
            set { _codtipoproducto = value; }
        }

        int _base;
        public int Base
        {
            get { return _base; }
            set { _base = value; }
        }

        String _descuento;
        public String Descuento
        {
            get { return _descuento; }
            set { _descuento = value; }
        }

    }
}