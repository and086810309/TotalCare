using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class items
    {
        private string codigo;
        private string descripcion;

        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }


    }
}