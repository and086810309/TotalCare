using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace WSMyDealerSAPv3
{
    public class DatosEnlace
    {
        public static string ipBaseDatos = ConfigurationManager.AppSettings["ipBaseDatos"]; // ip de la base de datos de SBO
        public static string puertoBaseDatos = ConfigurationManager.AppSettings["puertoBaseDatos"];

        public static string nombreBaseDatos = ConfigurationManager.AppSettings["nombreBaseDatos"]; // nombre de la base de datos SBO-COMMON
        public static string usuarioBaseDatos = ConfigurationManager.AppSettings["usuarioBaseDatos"]; // usuario de la base de datos de SAP
        public static string passwordBaseDatos = ConfigurationManager.AppSettings["passwordBaseDatos"]; // clave de la base de datos de SAP

        public static string ipServidorLicencia = ConfigurationManager.AppSettings["ipServidorLicencia"];
        public static string puertoServidorLicencia = ConfigurationManager.AppSettings["puertoServidorLicencia"];

        public static string usuarioSAP = ConfigurationManager.AppSettings["usuarioSAP"]; // usuario SAP (usuario registrado en SBO)
        public static string passwordSAP = ConfigurationManager.AppSettings["passwordSAP"]; // clave del usuario SAP (registrado en SBO)
        public static string tipoBaseDatos = ConfigurationManager.AppSettings["tipoBaseDatos"]; // dialecto de la base de datos
        public static string paisSistema = ConfigurationManager.AppSettings["paisSistema"]; // pais del sistema, define comportamientos especificos
        public static string aprobacion = ConfigurationManager.AppSettings["aprobacion"];   // indicador de si debe enviarse o no los parametros de aprobacion del pedido a SAP
        public static int serieRetencion = int.Parse(ConfigurationManager.AppSettings["serieRetencion"]); // serie usada en el caso retenciones (cobranzas)

        public static string glosaNotaCredito = ConfigurationManager.AppSettings["glosaNotaCredito"]; // pais del sistema, define comportamientos especificos
        public static string cuetaNotaCredito = ConfigurationManager.AppSettings["cuetaNotaCredito"];   // indicador de si debe enviarse o no los parametros de aprobacion del pedido a SAP
        public static string cuetaNotaAnticipoCobranza = ConfigurationManager.AppSettings["cuetaNotaAnticipoCobranza"];   // indicador de si debe enviarse o no los parametros de aprobacion del pedido a SAP

        public static void release_variables()
        {
            try
            {
                HttpContext.Current.Session.RemoveAll();
            }
            catch (Exception)
            {
            }

        }

    }
}