using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3.devolucion
{
    public class MantenimientoDevolucion
    {
        public static RespuestaDV registrarModelo(ModeloDV modelo)
        {
            RespuestaDV respuesta = new RespuestaDV();
            respuesta.Estado = 0;
            respuesta.Mensaje = "";

            // Establecer conexion

            DBSqlServer.ConectaDB();

            if (!DBSqlServer.Respuesta.Exito)
            {
                respuesta.Estado = 0;
                respuesta.Mensaje = DBSqlServer.Respuesta.DescripcionError;

                logs.grabarLog("registrarModelo", DBSqlServer.Respuesta.DescripcionError);

                return respuesta;
            }

            try
            {
                // Validar si el modelo existe

                int total = 0;

                SqlCommand com = new SqlCommand(" SELECT COUNT(*) AS total FROM " +
                    " dev_aut_modelo " +
                    " WHERE idmodelo = " + modelo.idmodelo +
                    " AND keyorganizacion = " + modelo.keyorganizacion + " ", DBSqlServer.GetConnection());
                com.CommandType = CommandType.Text;

                SqlDataReader record = com.ExecuteReader();

                if (record.HasRows)
                {
                    if (record.Read())
                    {
                        total = int.Parse(record.GetValue(0).ToString());
                    }
                }

                record.Close();
                record.Dispose();
                record = null;
                com.Dispose();

                if (total > 0)
                {
                    SqlCommand eliminar_modelo = new SqlCommand(" DELETE FROM dev_aut_modelo " +
                    " WHERE idmodelo = " + modelo.idmodelo +
                    " AND keyorganizacion = " + modelo.keyorganizacion, DBSqlServer.GetConnection());
                    eliminar_modelo.ExecuteNonQuery();

                    logs.grabarLog("registrarModelo", "El modelo ( " + modelo.idmodelo + " ), ha sido eliminado");
                }

                // Registrar el modelo

                SqlCommand insertar_modelo = new SqlCommand(" INSERT INTO dev_aut_modelo ( idmodelo, nombre, estado_modelo, validado, usuario_creacion, fecha_creacion, usuario_actualizacion, fecha_actualizacion, keyorganizacion, fecha_sincronizacion ) " +
                " VALUES ( @idmodelo, @nombre, @estado_modelo, @validado, @usuario_creacion, @fecha_creacion, @usuario_actualizacion, @fecha_actualizacion, @keyorganizacion, GETDATE()  ) ", DBSqlServer.GetConnection());

                insertar_modelo.Parameters.AddWithValue("@idmodelo", modelo.idmodelo);
                insertar_modelo.Parameters.AddWithValue("@nombre", modelo.nombre);
                insertar_modelo.Parameters.AddWithValue("@estado_modelo", modelo.estado_modelo);
                insertar_modelo.Parameters.AddWithValue("@validado", modelo.validado);
                insertar_modelo.Parameters.AddWithValue("@usuario_creacion", modelo.usuario_creacion);
                insertar_modelo.Parameters.AddWithValue("@fecha_creacion", modelo.fecha_creacion);

                if (String.IsNullOrEmpty(modelo.usuario_actualizacion))
                {
                    insertar_modelo.Parameters.AddWithValue("@usuario_actualizacion", DBNull.Value);
                }
                else
                {
                    insertar_modelo.Parameters.AddWithValue("@usuario_actualizacion", modelo.usuario_actualizacion);
                }

                if (String.IsNullOrEmpty(modelo.fecha_actualizacion))
                {
                    insertar_modelo.Parameters.AddWithValue("@fecha_actualizacion", DBNull.Value);
                }
                else
                {
                    insertar_modelo.Parameters.AddWithValue("@fecha_actualizacion", modelo.fecha_actualizacion);
                }

                insertar_modelo.Parameters.AddWithValue("@keyorganizacion", modelo.keyorganizacion);

                insertar_modelo.ExecuteNonQuery();

                // Todo salio bien

                respuesta.Estado = 1;
                respuesta.Mensaje = "El modelo ( " + modelo.idmodelo + " ), ha sido registrado";

                logs.grabarLog("registrarModelo", "El modelo ( " + modelo.idmodelo + " ), ha sido registrado");

            }
            catch (Exception e)
            {
                respuesta.Estado = 0;
                respuesta.Mensaje = e.Message;

                logs.grabarLog("registrarModelo", e.Message);
                logs.grabarLog("registrarModelo", e.StackTrace);
            }
            finally
            {
                DBSqlServer.DesconectaDB();
            }

            return respuesta;
        }

        public static RespuestaDV registrarEtapa(EtapaDV etapa)
        {
            RespuestaDV respuesta = new RespuestaDV();
            respuesta.Estado = 0;
            respuesta.Mensaje = "";

            // Establecer conexion

            DBSqlServer.ConectaDB();

            if (!DBSqlServer.Respuesta.Exito)
            {
                respuesta.Estado = 0;
                respuesta.Mensaje = DBSqlServer.Respuesta.DescripcionError;

                logs.grabarLog("registrarEtapa", DBSqlServer.Respuesta.DescripcionError);

                return respuesta;
            }

            try
            {
                // Validar si la etapa existe

                int total = 0;

                SqlCommand com = new SqlCommand(" SELECT COUNT(*) AS total FROM " +
                    " dev_aut_etapa " +
                    " WHERE idetapa = " + etapa.idetapa +
                    " AND keyorganizacion = " + etapa.keyorganizacion + " ", DBSqlServer.GetConnection());
                com.CommandType = CommandType.Text;

                SqlDataReader record = com.ExecuteReader();

                if (record.HasRows)
                {
                    if (record.Read())
                    {
                        total = int.Parse(record.GetValue(0).ToString());
                    }
                }

                record.Close();
                record.Dispose();
                record = null;
                com.Dispose();

                if (total > 0)
                {
                    SqlCommand eliminar_etapa = new SqlCommand(" DELETE FROM dev_aut_etapa " +
                    " WHERE idetapa = " + etapa.idetapa +
                    " AND keyorganizacion = " + etapa.keyorganizacion, DBSqlServer.GetConnection());
                    eliminar_etapa.ExecuteNonQuery();

                    logs.grabarLog("registrarEtapa", "La etapa ( " + etapa.idetapa + " ), ha sido eliminada");
                }

                // Registrar la etapa

                /*
	            [] [varchar](2) NOT NULL,
	            [] [char](2) NOT NULL,
                */

                SqlCommand insertar_etapa = new SqlCommand(" INSERT INTO dev_aut_etapa ( idetapa, idmodelo, descripcion, proceso_final, aprobador, solicitante, orden, siguiente_idetapa, estado_etapa, crea_nc, crea_st, crea_ls, mensaje, archivo, notifica_cliente, usuario_creacion, fecha_creacion, usuario_actualizacion, fecha_actualizacion, keyorganizacion, fecha_sincronizacion ) " +
                " VALUES ( @idetapa, @idmodelo, @descripcion, @proceso_final, @aprobador, @solicitante, @orden, @siguiente_idetapa, @estado_etapa, @crea_nc, @crea_st, @crea_ls, @mensaje, @archivo, @notifica_cliente, @usuario_creacion, @fecha_creacion, @usuario_actualizacion, @fecha_actualizacion, @keyorganizacion, GETDATE()  ) ", DBSqlServer.GetConnection());

                insertar_etapa.Parameters.AddWithValue("@idetapa", etapa.idetapa);
                insertar_etapa.Parameters.AddWithValue("@idmodelo", etapa.idmodelo);
                insertar_etapa.Parameters.AddWithValue("@descripcion", etapa.descripcion);

                if (String.IsNullOrEmpty(etapa.proceso_final))
                {
                    insertar_etapa.Parameters.AddWithValue("@proceso_final", DBNull.Value);
                }
                else
                {
                    insertar_etapa.Parameters.AddWithValue("@proceso_final", etapa.proceso_final);
                }

                insertar_etapa.Parameters.AddWithValue("@aprobador", etapa.aprobador);
                insertar_etapa.Parameters.AddWithValue("@solicitante", etapa.solicitante);
                insertar_etapa.Parameters.AddWithValue("@orden", etapa.orden);
                insertar_etapa.Parameters.AddWithValue("@siguiente_idetapa", etapa.siguiente_idetapa);
                insertar_etapa.Parameters.AddWithValue("@estado_etapa", etapa.estado_etapa);
                insertar_etapa.Parameters.AddWithValue("@crea_nc", etapa.crea_nc);
                insertar_etapa.Parameters.AddWithValue("@crea_st", etapa.crea_st);
                insertar_etapa.Parameters.AddWithValue("@crea_ls", etapa.crea_ls);

                if (String.IsNullOrEmpty(etapa.mensaje))
                {
                    insertar_etapa.Parameters.AddWithValue("@mensaje", DBNull.Value);
                }
                else
                {
                    insertar_etapa.Parameters.AddWithValue("@mensaje", etapa.mensaje);
                }

                insertar_etapa.Parameters.AddWithValue("@archivo", etapa.archivo);
                insertar_etapa.Parameters.AddWithValue("@notifica_cliente", etapa.notifica_cliente);
                insertar_etapa.Parameters.AddWithValue("@usuario_creacion", etapa.usuario_creacion);
                insertar_etapa.Parameters.AddWithValue("@fecha_creacion", etapa.fecha_creacion);

                if (String.IsNullOrEmpty(etapa.usuario_actualizacion))
                {
                    insertar_etapa.Parameters.AddWithValue("@usuario_actualizacion", DBNull.Value);
                }
                else
                {
                    insertar_etapa.Parameters.AddWithValue("@usuario_actualizacion", etapa.usuario_actualizacion);
                }

                if (String.IsNullOrEmpty(etapa.fecha_actualizacion))
                {
                    insertar_etapa.Parameters.AddWithValue("@fecha_actualizacion", DBNull.Value);
                }
                else
                {
                    insertar_etapa.Parameters.AddWithValue("@fecha_actualizacion", etapa.fecha_actualizacion);
                }

                insertar_etapa.Parameters.AddWithValue("@keyorganizacion", etapa.keyorganizacion);

                insertar_etapa.ExecuteNonQuery();

                // Todo salio bien

                respuesta.Estado = 1;
                respuesta.Mensaje = "La etapa ( " + etapa.idetapa + " ), ha sido registrada";

                logs.grabarLog("registrarEtapa", "La etapa ( " + etapa.idetapa + " ), ha sido registrada");

            }
            catch (Exception e)
            {
                respuesta.Estado = 0;
                respuesta.Mensaje = e.Message;

                logs.grabarLog("registrarEtapa", e.Message);
                logs.grabarLog("registrarEtapa", e.StackTrace);
            }
            finally
            {
                DBSqlServer.DesconectaDB();
            }

            return respuesta;
        }

        public static RespuestaDV registrarMotivo(MotivoDV motivo)
        {
            RespuestaDV respuesta = new RespuestaDV();
            respuesta.Estado = 0;
            respuesta.Mensaje = "";

            // Establecer conexion

            DBSqlServer.ConectaDB();

            if (!DBSqlServer.Respuesta.Exito)
            {
                respuesta.Estado = 0;
                respuesta.Mensaje = DBSqlServer.Respuesta.DescripcionError;

                logs.grabarLog("registrarMotivo", DBSqlServer.Respuesta.DescripcionError);

                return respuesta;
            }

            try
            {
                // Validar si el motivo existe

                int total = 0;

                SqlCommand com = new SqlCommand(" SELECT COUNT(*) AS total FROM " +
                    " dev_motivos " +
                    " WHERE idmotivo = " + motivo.idmotivo +
                    " AND keyorganizacion = " + motivo.keyorganizacion + " ", DBSqlServer.GetConnection());
                com.CommandType = CommandType.Text;

                SqlDataReader record = com.ExecuteReader();

                if (record.HasRows)
                {
                    if (record.Read())
                    {
                        total = int.Parse(record.GetValue(0).ToString());
                    }
                }

                record.Close();
                record.Dispose();
                record = null;
                com.Dispose();

                if (total > 0)
                {
                    SqlCommand eliminar_motivo = new SqlCommand(" DELETE FROM dev_motivos " +
                    " WHERE idmotivo = " + motivo.idmotivo +
                    " AND keyorganizacion = " + motivo.keyorganizacion, DBSqlServer.GetConnection());
                    eliminar_motivo.ExecuteNonQuery();

                    logs.grabarLog("registrarMotivo", "El motivo ( " + motivo.idmotivo + " ), ha sido eliminado");
                }

                // Registrar el motivo

                SqlCommand insertar_motivo = new SqlCommand(" INSERT INTO dev_motivos ( idmotivo, motivo, estado_motivo, keyorganizacion, fecha_sincronizacion ) " +
                " VALUES ( @idmotivo, @motivo, @estado_motivo, @keyorganizacion, GETDATE()  ) ", DBSqlServer.GetConnection());

                insertar_motivo.Parameters.AddWithValue("@idmotivo", motivo.idmotivo);
                insertar_motivo.Parameters.AddWithValue("@motivo", motivo.motivo);
                insertar_motivo.Parameters.AddWithValue("@estado_motivo", motivo.estado_motivo);
                insertar_motivo.Parameters.AddWithValue("@keyorganizacion", motivo.keyorganizacion);

                insertar_motivo.ExecuteNonQuery();

                // Todo salio bien

                respuesta.Estado = 1;
                respuesta.Mensaje = "El motivo ( " + motivo.idmotivo + " ), ha sido registrado";

                logs.grabarLog("registrarMotivo", "El motivo ( " + motivo.idmotivo + " ), ha sido registrado");

            }
            catch (Exception e)
            {
                respuesta.Estado = 0;
                respuesta.Mensaje = e.Message;

                logs.grabarLog("registrarMotivo", e.Message);
                logs.grabarLog("registrarMotivo", e.StackTrace);
            }
            finally
            {
                DBSqlServer.DesconectaDB();
            }

            return respuesta;
        }
    }
}