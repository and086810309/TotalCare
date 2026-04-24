using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data.Odbc;

namespace WSMyDealerSAPv3
{
    public class DBSqlServer
    {
        private static OdbcConnection conexion = null;

        public static OdbcConnection Conexion
        {
            get { return conexion; }
        }


        private static int error;
        public static int IError
        {
            get { return DBSqlServer.error; }
            set { DBSqlServer.error = value; }
        }

        private static string mensaje;
        public static string SError
        {
            get { return DBSqlServer.mensaje; }
            set { DBSqlServer.mensaje = value; }
        }

        private static Respuesta respuesta = new Respuesta();

        public static Respuesta Respuesta
        {
            get { return DBSqlServer.respuesta; }
            set { DBSqlServer.respuesta = value; }
        }

        private static void conectar()
        {
            try
            {
                if (conexion == null)
                {
                    conexion = new OdbcConnection();
                    System.Threading.Thread.Sleep(750);
                    //logs.grabarLog("DATA_AUX", "Conexion NULL");
                }

                if (conexion.State == System.Data.ConnectionState.Closed || conexion.State == System.Data.ConnectionState.Broken)
                {
                    if (conexion.State == System.Data.ConnectionState.Broken) conexion.Close();

                    //conexion.ConnectionString = "Driver={HDBODBC};ServerNode=" + DatosEnlace.ipBaseDatos + ":" + DatosEnlace.puertoBaseDatos
                    //    + ";CurrentSchema=" + DatosEnlace.nombreBaseDatos
                    //    + ";UID=" + DatosEnlace.usuarioBaseDatos
                    //    + ";Pwd=" + DatosEnlace.passwordBaseDatos;
                    //conexion.ConnectionString = "Driver={HDBODBC};ServerNode=192.168.0.32:30013;CurrentSchema=DB_PRUEBAS03;UID=MYDEALER;Pwd=TQZzSrN2";
                    string dns_code = "SAPHANNA32"; //usar para correr por codigo 
                    string dns_iis = "SAPHANNA";
                    conexion.ConnectionString = "DSN="+ dns_iis + ";ServerNode=" + DatosEnlace.ipBaseDatos.Replace("NDB@", "") + ":" + DatosEnlace.puertoBaseDatos
                        +";CurrentSchema="+ DatosEnlace.nombreBaseDatos 
                        + ";UID="+ DatosEnlace.usuarioBaseDatos + ";Pwd="+DatosEnlace.passwordBaseDatos;
                }

                if (conexion.State == System.Data.ConnectionState.Closed)
                    conexion.Open();

                if (conexion.State == System.Data.ConnectionState.Broken)
                {
                    conexion.Open();
                }

                if (conexion.State == System.Data.ConnectionState.Connecting)
                {
                    while (true)
                    {
                        if (conexion.State == System.Data.ConnectionState.Open)
                            break;
                        System.Threading.Thread.Sleep(450);
                    }
                }

                respuesta.Exito = true;
            }
            catch (Exception e)
            {
                respuesta.Exito = false;
                respuesta.CodigoError = "Problema al abri la conexion";
                respuesta.CodigoRespuesta = "CON001";
                respuesta.DescripcionError = "Error al conectar a la Base de Datos. " + e.Message;
                Console.WriteLine(e.Message);
                logs.grabarLog("DATA", e.Message);
                logs.grabarLog("DATA_DEBUG", e.StackTrace);
            }
        }

        public static void ConectaDB()
        {
            conectar();
        }

        public static void DesconectaDB()
        {
            try
            {
                respuesta.Exito = true;
            }
            catch (Exception e)
            {
                respuesta.Exito = false;
                respuesta.CodigoError = "CON002";
                respuesta.DescripcionError = "Error al desconectar a la Compañia. " + e.Message;
            }
        }

    }
}