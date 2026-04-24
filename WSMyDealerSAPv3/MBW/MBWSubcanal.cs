using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class MBWSubcanal
    {
        int codsubcanal;

        public int Codsubcanal
        {
            get { return codsubcanal; }
            set { codsubcanal = value; }
        }
        string descripcion;

        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }
    }
}