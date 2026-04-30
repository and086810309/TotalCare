using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3.devolucion
{
    public class ModeloDV
    {
        public int idmodelo { get; set; }
        public string nombre { get; set; }
        public string estado_modelo { get; set; }
        public string validado { get; set; }
        public string usuario_creacion { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string usuario_actualizacion { get; set; }
        public string fecha_actualizacion { get; set; }
        public int keyorganizacion { get; set; }
    }
}