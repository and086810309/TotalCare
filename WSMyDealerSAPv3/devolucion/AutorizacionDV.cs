using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3.devolucion
{
    public class AutorizacionDV
    {
        public int iddevaut { get; set; }
        public int idmodelo { get; set; }
        public int idetapa { get; set; }
        public int iddevcab { get; set; }
        public string tipo_aprobador { get; set; }
        public string aprobador { get; set; }
        public string tipo_solicitante { get; set; }
        public string solicitante { get; set; }
        public string estado { get; set; }
        public string observacion { get; set; }
        public string mensaje { get; set; }
        public string mensaje_respuesta { get; set; }
        public string archivo { get; set; }
        public string documento { get; set; }
        public string tipo_creacion { get; set; }
        public string usuario_creacion { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string tipo_actualizacion { get; set; }
        public string usuario_actualizacion { get; set; }
        public string fecha_actualizacion { get; set; }
        public int keyorganizacion { get; set; }
    }
}