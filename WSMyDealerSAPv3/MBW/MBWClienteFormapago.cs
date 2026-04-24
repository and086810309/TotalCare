using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class MBWClienteFormapago
    {
        string codcliente;

        public string Codcliente
        {
            get { return codcliente; }
            set { codcliente = value; }
        }
        int codformapago;

        public int Codformapago
        {
            get { return codformapago; }
            set { codformapago = value; }
        }
    }
} // md_clientesformaspago