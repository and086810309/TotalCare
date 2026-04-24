using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WSMyDealerSAPv3
{
    public partial class demoClienteNuevo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            WSIntegracion ws = new WSIntegracion();

            string xml = "<Clientes> <CodigoCliente><![CDATA[C0918004407]]></CodigoCliente> <Identificacion><![CDATA[0918004407]]>" + 
                "</Identificacion> <Nombres><![CDATA[Kevin]]></Nombres> <NombreComercial><![CDATA[Kevin S.A.]]></NombreComercial> " + 
                "<DireccionEnvio><![CDATA[por ahi]]></DireccionEnvio> <DireccionEstablecimiento><![CDATA[ninguna]]></DireccionEstablecimiento>" + 
                "<Telefono><![CDATA[0967839558]]></Telefono> <ObligadoContabilidad><![CDATA[N]]></ObligadoContabilidad> " + 
                "<LocalPropio><![CDATA[S]]></LocalPropio> <AniosAntiguedad><![CDATA[5]]></AniosAntiguedad> <Provincia><![CDATA[18]]></Provincia> <Ciudad><![CDATA[Guayaquil]]></Ciudad> <Parroquia><![CDATA[3]]></Parroquia> <GrupoClientes><![CDATA[100]]></GrupoClientes> <DiasVisita><![CDATA[8]]></DiasVisita> <CondicionPago><![CDATA[-1]]></CondicionPago> <CopiaCedula><![CDATA[N]]></CopiaCedula> <SoporteBienes><![CDATA[N]]></SoporteBienes> <Pagare><![CDATA[N]]></Pagare> <Factura><![CDATA[N]]></Factura> <Foto><![CDATA[N]]></Foto> <Observaciones><![CDATA[Ninguna]]></Observaciones></Clientes>";
            //txtMsg.Text = ws.registrarCllienteNuevo(xml);

        }
    }
}