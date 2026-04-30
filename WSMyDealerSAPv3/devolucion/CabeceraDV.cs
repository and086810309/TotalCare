using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3.devolucion
{
    public class CabeceraDV
    {
        public int iddevcab { get; set; }
        public int docentry { get; set; }
        public int docnum { get; set; }
        public string foliopref { get; set; }
        public int folionum { get; set; }
        public string documento_base { get; set; }
        public int idmotivo { get; set; }
        public int idmodelo { get; set; }
        public int idetapa_previa { get; set; }
        public int idetapa { get; set; }
        public int iddevaut { get; set; }
        public string estado { get; set; }
        public string proceso_final { get; set; }
        public string observacion { get; set; }
        public string cardcode { get; set; }
        public string cardname { get; set; }
        public double subtotal { get; set; }
        public double vatsum { get; set; }
        public double doctotal { get; set; }
        public string comentario { get; set; }
        public string contacto_nombre { get; set; }
        public string contacto_email { get; set; }
        public string contacto_celular { get; set; }
        public string puntoemision { get; set; }
        public string direccion { get; set; }
        public string ciudad { get; set; }
        public int bultos { get; set; }
        public string estado_nc { get; set; }
        public int docentry_nc_preliminar { get; set; }
        public int docnum_nc_preliminar { get; set; }
        public string fecha_nc_preliminar { get; set; }
        public int docentry_nc { get; set; }
        public int docnum_nc { get; set; }
        public string foliopref_nc { get; set; }
        public int folionum_nc { get; set; }
        public string fecha_nc { get; set; }
        public string sincronizar_nc { get; set; }
        public string codvendedor { get; set; }
        public string U_tipo_pedido { get; set; }
        public string U_Agencia { get; set; }
        public string U_Direccion { get; set; }
        public string U_LLP_PFinal { get; set; }
        public string U_DIR_PTO_ORIGEN { get; set; }
        public int docentry_st { get; set; }
        public int docnum_st { get; set; }
        public string sincronizar_st { get; set; }
        public int callid_ls { get; set; }
        public int docnum_ls { get; set; }
        public string sincronizar_ls { get; set; }
        public string tipo_creacion { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string tipo_actualizacion { get; set; }
        public string usuario_actualizacion { get; set; }
        public string fecha_actualizacion { get; set; }
        public int keyorganizacion { get; set; }
    }
}