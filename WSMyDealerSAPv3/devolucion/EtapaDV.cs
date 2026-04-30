using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3.devolucion
{
    public class EtapaDV
    {
        public int idetapa { get; set; }
        public int idmodelo { get; set; }
        public string descripcion { get; set; }
        public string proceso_final { get; set; }
        public string aprobador { get; set; }
        public string solicitante { get; set; }
        public int orden { get; set; }
        public string siguiente_idetapa { get; set; }
        public string estado_etapa { get; set; }
        public string crea_nc { get; set; }
        public string crea_st { get; set; }
        public string crea_ls { get; set; }
        public string mensaje { get; set; }
        public string archivo { get; set; }
        public string notifica_cliente { get; set; }
        public string usuario_creacion { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string usuario_actualizacion { get; set; }
        public string fecha_actualizacion { get; set; }
        public int keyorganizacion { get; set; }
    }
}