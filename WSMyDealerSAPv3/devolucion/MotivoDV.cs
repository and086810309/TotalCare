using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3.devolucion
{
    public class MotivoDV
    {
        public int idmotivo { get; set; }
        public string motivo { get; set; }
        public string estado_motivo { get; set; }
        public int keyorganizacion { get; set; }
    }
}