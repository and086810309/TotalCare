using System;
using System.Data.SqlClient;

namespace WSMyDealerSAPv3
{
    public class DBSqlServer
    {
        public static SqlConnection Conexion { get; private set; }

        private static int error;
        public static int IError
        {
            get { return error; }
            set { error = value; }
        }

        private static string mensaje;
        public static string SError
        {
            get { return mensaje; }
            set { mensaje = value; }
        }

        private static Respuesta respuesta = new Respuesta();
        public static Respuesta Respuesta
        {
            get { return respuesta; }
            set { respuesta = value; }
        }

        /// <summary>
        /// Cadena de conexión SQL SERVER
        /// </summary>
        private static string GetConnectionString()
        {
            string servidor = DatosEnlace.ipBaseDatos.Replace("NDB@", "");
            string baseDatos = DatosEnlace.nombreBaseDatos;
            string usuario = DatosEnlace.usuarioBaseDatos;
            string password = DatosEnlace.passwordBaseDatos;

            // Puerto SQL Server
            string puerto = "1433";

            return $"Server={servidor},{puerto};" +
                   $"Database={baseDatos};" +
                   $"User Id={usuario};" +
                   $"Password={password};" +
                   $"TrustServerCertificate=True;" +
                   $"Connection Timeout=15;";
        }

        /// <summary>
        /// Obtiene conexión abierta
        /// </summary>
        public static SqlConnection GetConnection()
        {
            try
            {
                SqlConnection connection = new SqlConnection(GetConnectionString());
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                logs.grabarLog("SQL_CONN_ERROR", ex.Message);
                logs.grabarLog("SQL_CONN_ERROR_DEBUG", ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Prueba de conexión desde el WS
        /// </summary>
        public static Respuesta ProbarConexion()
        {
            Respuesta resp = new Respuesta();

            try
            {
                using (SqlConnection cn = GetConnection())
                {
                    resp.Exito = true;
                    resp.CodigoRespuesta = "OK";
                    resp.DescripcionError = "Conexión exitosa a SQL Server";
                    logs.grabarLog("SQL_TEST", "CONEXION EXITOSA SQL SERVER");
                }
            }
            catch (Exception ex)
            {
                resp.Exito = false;
                resp.CodigoError = "CON001";
                resp.CodigoRespuesta = "Error de conexión SQL Server";
                resp.DescripcionError = ex.Message;

                logs.grabarLog("SQL_TEST_ERROR", ex.Message);
                logs.grabarLog("SQL_TEST_ERROR_DEBUG", ex.StackTrace);
            }

            return resp;
        }

        /// <summary>
        /// Método opcional de verificación manual
        /// </summary>
        public static void ConectaDB()
        {
            try
            {
                if (Conexion != null)
                {
                    if (Conexion.State != System.Data.ConnectionState.Closed)
                    {
                        Conexion.Close();
                    }

                    Conexion.Dispose();
                }

                Conexion = GetConnection();
                respuesta.Exito = true;
                respuesta.CodigoError = "";
                respuesta.CodigoRespuesta = "OK";
                respuesta.DescripcionError = "";
            }
            catch (Exception ex)
            {
                respuesta.Exito = false;
                respuesta.CodigoError = "CON001";
                respuesta.CodigoRespuesta = "Error de conexión SQL Server";
                respuesta.DescripcionError = ex.Message;
            }
        }

        public static void DesconectaDB()
        {
            if (Conexion != null)
            {
                if (Conexion.State != System.Data.ConnectionState.Closed)
                {
                    Conexion.Close();
                }

                Conexion.Dispose();
                Conexion = null;
            }

            respuesta.Exito = true;
        }
    }
}
