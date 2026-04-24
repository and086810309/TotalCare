using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAPbobsCOM;

namespace WSMyDealerSAPv3
{
    public class DataBase
    {
        /*private static SAPbobsCOM.Company company = null;
        public static SAPbobsCOM.Company Company
        {
            get { return DataBase.company; }
            set { DataBase.company = value; }
        }*/


        private static int error;
        public static int IError
        {
            get { return DataBase.error; }
            set { DataBase.error = value; }
        }


        private static string mensaje;
        public static string SError
        {
            get { return DataBase.mensaje; }
            set { DataBase.mensaje = value; }
        }


        private static Respuesta respuesta = new Respuesta();
        public static Respuesta Respuesta
        {
            get { return DataBase.respuesta; }
            set { DataBase.respuesta = value; }
        }


        public static Company conectar()
        {
            Company company = null;
            try
            {
                respuesta.Exito = false;

                company = new Company();

                company.Server = DatosEnlace.ipBaseDatos + ":" + DatosEnlace.puertoBaseDatos; // "SAPSRVBBDD";

                company.CompanyDB = DatosEnlace.nombreBaseDatos; // "DB_CAPACITACION";
                company.UserName = DatosEnlace.usuarioSAP; // "mydealer";
                company.Password = DatosEnlace.passwordSAP; // "myd321";
                company.DbUserName = DatosEnlace.usuarioBaseDatos; // "mydealer";
                company.DbPassword = DatosEnlace.passwordBaseDatos; // "myd321";
                company.language = BoSuppLangs.ln_Spanish_La;

                switch (DatosEnlace.tipoBaseDatos)
                {
                    case "dst_MSSQL2008":
                        company.DbServerType = BoDataServerTypes.dst_MSSQL2008;
                        break;

                    case "dst_MSSQL2012":
                        company.DbServerType = BoDataServerTypes.dst_MSSQL2012;
                        break;

                    case "dst_MSSQL2005":
                        company.DbServerType = BoDataServerTypes.dst_MSSQL2005;
                        break;

                    case "dst_HANADB":
                        company.DbServerType = BoDataServerTypes.dst_HANADB;
                        break;

                }

                company.LicenseServer = DatosEnlace.ipServidorLicencia + ":" + DatosEnlace.puertoServidorLicencia;

                company.UseTrusted = false;
                error = company.Connect();
                if (error != 0)
                {
                    company.GetLastError(out error, out mensaje);
                    throw new Exception(error + ": " + mensaje);
                }
                respuesta.Exito = true;
            }
            catch (Exception e)
            {
                company = null;
                respuesta.Exito = false;
                respuesta.CodigoError = error.ToString();
                respuesta.CodigoRespuesta = "COM001";
                respuesta.DescripcionError = mensaje; //  "Error al conectar a la Compañia";
                //Console.WriteLine(e.Message);
                logs.grabarLog("SAP_BDD", e.Message);
                logs.grabarLog("SAP_BDD_DEBUG", e.StackTrace);
            }

            return company;
        }


        /*public static void ConectaDB()
        {
            if ( company==null || !company.Connected)
                conectar();
        }*/


        public static void DesconectaDB(SAPbobsCOM.Company company)
        {
            try
            {
                if (company.Connected)
                {
                    company.Disconnect();
                    //if (company != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(company);
                    company = null;
                }

                respuesta.Exito = true;
            }
            catch (Exception e)
            {
                company = null;

                respuesta.Exito = false;
                respuesta.CodigoError = "COM002";
                respuesta.DescripcionError = "Error al desconectar a la Compañia";
                logs.grabarLog("DSAP_BDD", e.Message);
                logs.grabarLog("DSAP_BDD_DEBUG", e.StackTrace);
            }
        }

    }
}