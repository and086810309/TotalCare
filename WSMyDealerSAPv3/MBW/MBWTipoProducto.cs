using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class MBWTipoProducto
    {
        int codtipoproducto;

        public int Codtipoproducto
        {
            get { return codtipoproducto; }
            set { codtipoproducto = value; }
        }
        string descripcion;

        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }

        int codgrupomaterial;

        public int Codgrupomaterial
        {
            get { return codgrupomaterial; }
            set { codgrupomaterial = value; }
        }
    }
}