using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3.devolucion
{
    public class Devolucion
    {
        public static RespuestaDV registrarDevolucion(CabeceraDV cabecera, DetalleDV[] detalles, AutorizacionDV[] autorizaciones)
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

                logs.grabarLog("registrarDevolucion", DBSqlServer.Respuesta.DescripcionError);

                return respuesta;
            }

            try
            {
                // Validar si la devolucion existe

                int total = 0;

                SqlCommand com = new SqlCommand(" SELECT COUNT(*) AS total FROM " +
                    " dev_cab " +
                    " WHERE iddevcab = " + cabecera.iddevcab +
                    " AND keyorganizacion = " + cabecera.keyorganizacion + " ", DBSqlServer.GetConnection());
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
                    SqlCommand eliminar_ordencab = new SqlCommand(" DELETE FROM dev_cab " +
                    " WHERE iddevcab = " + cabecera.iddevcab +
                    " AND keyorganizacion = " + cabecera.keyorganizacion, DBSqlServer.GetConnection());
                    eliminar_ordencab.ExecuteNonQuery();

                    SqlCommand eliminar_ordendet = new SqlCommand(" DELETE FROM dev_det " +
                    " WHERE iddevcab = " + cabecera.iddevcab +
                    " AND keyorganizacion = " + cabecera.keyorganizacion, DBSqlServer.GetConnection());
                    eliminar_ordendet.ExecuteNonQuery();

                    SqlCommand eliminar_ordenaut = new SqlCommand(" DELETE FROM dev_aut " +
                    " WHERE iddevcab = " + cabecera.iddevcab +
                    " AND keyorganizacion = " + cabecera.keyorganizacion, DBSqlServer.GetConnection());
                    eliminar_ordenaut.ExecuteNonQuery();

                    logs.grabarLog("registrarDevolucion", "La devolucion ( " + cabecera.iddevcab + " ), ha sido eliminada");
                }

                // Registrar la cabecera

                SqlCommand insertar_devolucion = new SqlCommand(" INSERT INTO dev_cab ( iddevcab, docentry, docnum, foliopref, folionum, documento_base, idmotivo, idmodelo, idetapa_previa, idetapa, iddevaut, estado, proceso_final, observacion, cardcode, cardname, subtotal, vatsum, doctotal, comentario, contacto_nombre, contacto_email, contacto_celular, puntoemision, direccion, ciudad, bultos, estado_nc, docentry_nc_preliminar, docnum_nc_preliminar, fecha_nc_preliminar, docentry_nc, docnum_nc, foliopref_nc, folionum_nc, fecha_nc, sincronizar_nc, codvendedor, U_tipo_pedido, U_Agencia, U_Direccion, U_LLP_PFinal, U_DIR_PTO_ORIGEN, docentry_st, docnum_st, sincronizar_st, callid_ls, docnum_ls, sincronizar_ls, tipo_creacion, fecha_creacion, tipo_actualizacion, usuario_actualizacion, fecha_actualizacion, keyorganizacion, fecha_sincronizacion ) " +
                " VALUES ( @iddevcab, @docentry, @docnum, @foliopref, @folionum, @documento_base, @idmotivo, @idmodelo, @idetapa_previa, @idetapa, @iddevaut, @estado, @proceso_final, @observacion, @cardcode, @cardname, @subtotal, @vatsum, @doctotal, @comentario, @contacto_nombre, @contacto_email, @contacto_celular, @puntoemision, @direccion, @ciudad, @bultos, @estado_nc, @docentry_nc_preliminar, @docnum_nc_preliminar, @fecha_nc_preliminar, @docentry_nc, @docnum_nc, @foliopref_nc, @folionum_nc, @fecha_nc, @sincronizar_nc, @codvendedor, @U_tipo_pedido, @U_Agencia, @U_Direccion, @U_LLP_PFinal, @U_DIR_PTO_ORIGEN, @docentry_st, @docnum_st, @sincronizar_st, @callid_ls, @docnum_ls, @sincronizar_ls, @tipo_creacion, @fecha_creacion, @tipo_actualizacion, @usuario_actualizacion, @fecha_actualizacion, @keyorganizacion, GETDATE() ) ", DBSqlServer.GetConnection());

                insertar_devolucion.Parameters.AddWithValue("@iddevcab", cabecera.iddevcab);
                insertar_devolucion.Parameters.AddWithValue("@docentry", cabecera.docentry);
                insertar_devolucion.Parameters.AddWithValue("@docnum", cabecera.docnum);
                insertar_devolucion.Parameters.AddWithValue("@foliopref", cabecera.foliopref);
                insertar_devolucion.Parameters.AddWithValue("@folionum", cabecera.folionum);
                insertar_devolucion.Parameters.AddWithValue("@documento_base", cabecera.documento_base);
                insertar_devolucion.Parameters.AddWithValue("@idmotivo", cabecera.idmotivo);
                insertar_devolucion.Parameters.AddWithValue("@idmodelo", cabecera.idmodelo);
                insertar_devolucion.Parameters.AddWithValue("@idetapa_previa", cabecera.idetapa_previa);
                insertar_devolucion.Parameters.AddWithValue("@idetapa", cabecera.idetapa);
                insertar_devolucion.Parameters.AddWithValue("@iddevaut", cabecera.iddevaut);
                insertar_devolucion.Parameters.AddWithValue("@estado", cabecera.estado);

                if (String.IsNullOrEmpty(cabecera.proceso_final))
                {
                    insertar_devolucion.Parameters.AddWithValue("@proceso_final", DBNull.Value);
                }
                else
                {
                    insertar_devolucion.Parameters.AddWithValue("@proceso_final", cabecera.proceso_final);
                }

                if (String.IsNullOrEmpty(cabecera.observacion))
                {
                    insertar_devolucion.Parameters.AddWithValue("@observacion", DBNull.Value);
                }
                else
                {
                    insertar_devolucion.Parameters.AddWithValue("@observacion", cabecera.observacion);
                }

                insertar_devolucion.Parameters.AddWithValue("@cardcode", cabecera.cardcode);
                insertar_devolucion.Parameters.AddWithValue("@cardname", cabecera.cardname);
                insertar_devolucion.Parameters.AddWithValue("@subtotal", cabecera.subtotal);
                insertar_devolucion.Parameters.AddWithValue("@vatsum", cabecera.vatsum);
                insertar_devolucion.Parameters.AddWithValue("@doctotal", cabecera.doctotal);
                insertar_devolucion.Parameters.AddWithValue("@comentario", cabecera.comentario);

                if (String.IsNullOrEmpty(cabecera.contacto_nombre))
                {
                    insertar_devolucion.Parameters.AddWithValue("@contacto_nombre", DBNull.Value);
                }
                else
                {
                    insertar_devolucion.Parameters.AddWithValue("@contacto_nombre", cabecera.contacto_nombre);
                }

                if (String.IsNullOrEmpty(cabecera.contacto_email))
                {
                    insertar_devolucion.Parameters.AddWithValue("@contacto_email", DBNull.Value);
                }
                else
                {
                    insertar_devolucion.Parameters.AddWithValue("@contacto_email", cabecera.contacto_email);
                }

                if (String.IsNullOrEmpty(cabecera.contacto_celular))
                {
                    insertar_devolucion.Parameters.AddWithValue("@contacto_celular", DBNull.Value);
                }
                else
                {
                    insertar_devolucion.Parameters.AddWithValue("@contacto_celular", cabecera.contacto_celular);
                }

                if (String.IsNullOrEmpty(cabecera.puntoemision))
                {
                    insertar_devolucion.Parameters.AddWithValue("@puntoemision", DBNull.Value);
                }
                else
                {
                    insertar_devolucion.Parameters.AddWithValue("@puntoemision", cabecera.puntoemision);
                }

                if (String.IsNullOrEmpty(cabecera.direccion))
                {
                    insertar_devolucion.Parameters.AddWithValue("@direccion", DBNull.Value);
                }
                else
                {
                    insertar_devolucion.Parameters.AddWithValue("@direccion", cabecera.direccion);
                }

                if (String.IsNullOrEmpty(cabecera.ciudad))
                {
                    insertar_devolucion.Parameters.AddWithValue("@ciudad", DBNull.Value);
                }
                else
                {
                    insertar_devolucion.Parameters.AddWithValue("@ciudad", cabecera.ciudad);
                }

                insertar_devolucion.Parameters.AddWithValue("@bultos", cabecera.bultos);

                if (String.IsNullOrEmpty(cabecera.estado_nc))
                {
                    insertar_devolucion.Parameters.AddWithValue("@estado_nc", DBNull.Value);
                }
                else
                {
                    insertar_devolucion.Parameters.AddWithValue("@estado_nc", cabecera.estado_nc);
                }

                insertar_devolucion.Parameters.AddWithValue("@docentry_nc_preliminar", cabecera.docentry_nc_preliminar);
                insertar_devolucion.Parameters.AddWithValue("@docnum_nc_preliminar", cabecera.docnum_nc_preliminar);

                if (String.IsNullOrEmpty(cabecera.fecha_nc_preliminar))
                {
                    insertar_devolucion.Parameters.AddWithValue("@fecha_nc_preliminar", DBNull.Value);
                }
                else
                {
                    insertar_devolucion.Parameters.AddWithValue("@fecha_nc_preliminar", cabecera.fecha_nc_preliminar);
                }

                insertar_devolucion.Parameters.AddWithValue("@docentry_nc", cabecera.docentry_nc);
                insertar_devolucion.Parameters.AddWithValue("@docnum_nc", cabecera.docnum_nc);

                if (String.IsNullOrEmpty(cabecera.foliopref_nc))
                {
                    insertar_devolucion.Parameters.AddWithValue("@foliopref_nc", DBNull.Value);
                }
                else
                {
                    insertar_devolucion.Parameters.AddWithValue("@foliopref_nc", cabecera.foliopref_nc);
                }

                insertar_devolucion.Parameters.AddWithValue("@folionum_nc", cabecera.folionum_nc);

                if (String.IsNullOrEmpty(cabecera.fecha_nc))
                {
                    insertar_devolucion.Parameters.AddWithValue("@fecha_nc", DBNull.Value);
                }
                else
                {
                    insertar_devolucion.Parameters.AddWithValue("@fecha_nc", cabecera.fecha_nc);
                }

                insertar_devolucion.Parameters.AddWithValue("@sincronizar_nc", cabecera.sincronizar_nc);
                insertar_devolucion.Parameters.AddWithValue("@codvendedor", cabecera.codvendedor);
                insertar_devolucion.Parameters.AddWithValue("@U_tipo_pedido", cabecera.U_tipo_pedido);
                insertar_devolucion.Parameters.AddWithValue("@U_Agencia", cabecera.U_Agencia);
                insertar_devolucion.Parameters.AddWithValue("@U_Direccion", cabecera.U_Direccion);
                insertar_devolucion.Parameters.AddWithValue("@U_LLP_PFinal", cabecera.U_LLP_PFinal);
                insertar_devolucion.Parameters.AddWithValue("@U_DIR_PTO_ORIGEN", cabecera.U_DIR_PTO_ORIGEN);
                insertar_devolucion.Parameters.AddWithValue("@docentry_st", cabecera.docentry_st);
                insertar_devolucion.Parameters.AddWithValue("@docnum_st", cabecera.docnum_st);
                insertar_devolucion.Parameters.AddWithValue("@sincronizar_st", cabecera.sincronizar_st);
                insertar_devolucion.Parameters.AddWithValue("@callid_ls", cabecera.callid_ls);
                insertar_devolucion.Parameters.AddWithValue("@docnum_ls", cabecera.docnum_ls);
                insertar_devolucion.Parameters.AddWithValue("@sincronizar_ls", cabecera.sincronizar_ls);
                insertar_devolucion.Parameters.AddWithValue("@tipo_creacion", cabecera.tipo_creacion);
                insertar_devolucion.Parameters.AddWithValue("@fecha_creacion", cabecera.fecha_creacion);

                if (String.IsNullOrEmpty(cabecera.tipo_actualizacion))
                {
                    insertar_devolucion.Parameters.AddWithValue("@tipo_actualizacion", DBNull.Value);
                }
                else
                {
                    insertar_devolucion.Parameters.AddWithValue("@tipo_actualizacion", cabecera.tipo_actualizacion);
                }

                if (String.IsNullOrEmpty(cabecera.usuario_actualizacion))
                {
                    insertar_devolucion.Parameters.AddWithValue("@usuario_actualizacion", DBNull.Value);
                }
                else
                {
                    insertar_devolucion.Parameters.AddWithValue("@usuario_actualizacion", cabecera.usuario_actualizacion);
                }

                if (String.IsNullOrEmpty(cabecera.fecha_actualizacion))
                {
                    insertar_devolucion.Parameters.AddWithValue("@fecha_actualizacion", DBNull.Value);
                }
                else
                {
                    insertar_devolucion.Parameters.AddWithValue("@fecha_actualizacion", cabecera.fecha_actualizacion);
                }

                insertar_devolucion.Parameters.AddWithValue("@keyorganizacion", cabecera.keyorganizacion);

                insertar_devolucion.ExecuteNonQuery();

                // Registrar el detalle

                foreach (DetalleDV detalle in detalles)
                {
                    SqlCommand insertar_detalle = new SqlCommand(" INSERT INTO dev_det ( iddevdet, iddevcab, docentry, linenum, itemcode, dscription, numeroparte, whscode, quantity, quantity_sap, price, pricebefdi, discprcnt, linesubtotal, vatprcnt, linevat, linetotal, tiene_img, nombre_img, tiene_doc, nombre_doc, fecha_creacion, keyorganizacion ) " +
                    " VALUES ( @iddevdet, @iddevcab, @docentry, @linenum, @itemcode, @dscription, @numeroparte, @whscode, @quantity, @quantity_sap, @price, @pricebefdi, @discprcnt, @linesubtotal, @vatprcnt, @linevat, @linetotal, @tiene_img, @nombre_img, @tiene_doc, @nombre_doc, @fecha_creacion, @keyorganizacion ) ", DBSqlServer.GetConnection());

                    insertar_detalle.Parameters.AddWithValue("@iddevdet", detalle.iddevdet);
                    insertar_detalle.Parameters.AddWithValue("@iddevcab", detalle.iddevcab);
                    insertar_detalle.Parameters.AddWithValue("@docentry", detalle.docentry);
                    insertar_detalle.Parameters.AddWithValue("@linenum", detalle.linenum);
                    insertar_detalle.Parameters.AddWithValue("@itemcode", detalle.itemcode);
                    insertar_detalle.Parameters.AddWithValue("@dscription", detalle.dscription);
                    insertar_detalle.Parameters.AddWithValue("@numeroparte", detalle.numeroparte);
                    insertar_detalle.Parameters.AddWithValue("@whscode", detalle.whscode);
                    insertar_detalle.Parameters.AddWithValue("@quantity", detalle.quantity);
                    insertar_detalle.Parameters.AddWithValue("@quantity_sap", detalle.quantity_sap);
                    insertar_detalle.Parameters.AddWithValue("@price", detalle.price);
                    insertar_detalle.Parameters.AddWithValue("@pricebefdi", detalle.pricebefdi);
                    insertar_detalle.Parameters.AddWithValue("@discprcnt", detalle.discprcnt);
                    insertar_detalle.Parameters.AddWithValue("@linesubtotal", detalle.linesubtotal);
                    insertar_detalle.Parameters.AddWithValue("@vatprcnt", detalle.vatprcnt);
                    insertar_detalle.Parameters.AddWithValue("@linevat", detalle.linevat);
                    insertar_detalle.Parameters.AddWithValue("@linetotal", detalle.linetotal);
                    insertar_detalle.Parameters.AddWithValue("@tiene_img", detalle.tiene_img);

                    if (String.IsNullOrEmpty(detalle.nombre_img))
                    {
                        insertar_detalle.Parameters.AddWithValue("@nombre_img", DBNull.Value);
                    }
                    else
                    {
                        insertar_detalle.Parameters.AddWithValue("@nombre_img", detalle.nombre_img);
                    }

                    insertar_detalle.Parameters.AddWithValue("@tiene_doc", detalle.tiene_doc);

                    if (String.IsNullOrEmpty(detalle.nombre_doc))
                    {
                        insertar_detalle.Parameters.AddWithValue("@nombre_doc", DBNull.Value);
                    }
                    else
                    {
                        insertar_detalle.Parameters.AddWithValue("@nombre_doc", detalle.nombre_doc);
                    }

                    insertar_detalle.Parameters.AddWithValue("@fecha_creacion", detalle.fecha_creacion);
                    insertar_detalle.Parameters.AddWithValue("@keyorganizacion", detalle.keyorganizacion);

                    insertar_detalle.ExecuteNonQuery();

                }

                // Registrar la autorizacion

                foreach (AutorizacionDV autorizacion in autorizaciones)
                {

                    SqlCommand insertar_autorizacion = new SqlCommand(" INSERT INTO dev_aut ( iddevaut, idmodelo, idetapa, iddevcab, tipo_aprobador, aprobador, tipo_solicitante, solicitante, estado, observacion, mensaje, mensaje_respuesta, archivo, documento, tipo_creacion, usuario_creacion, fecha_creacion, tipo_actualizacion, usuario_actualizacion, fecha_actualizacion, keyorganizacion ) " +
                    " VALUES ( @iddevaut, @idmodelo, @idetapa, @iddevcab, @tipo_aprobador, @aprobador, @tipo_solicitante, @solicitante, @estado, @observacion, @mensaje, @mensaje_respuesta, @archivo, @documento, @tipo_creacion, @usuario_creacion, @fecha_creacion, @tipo_actualizacion, @usuario_actualizacion, @fecha_actualizacion, @keyorganizacion ) ", DBSqlServer.GetConnection());

                    insertar_autorizacion.Parameters.AddWithValue("@iddevaut", autorizacion.iddevaut);
                    insertar_autorizacion.Parameters.AddWithValue("@idmodelo", autorizacion.idmodelo);
                    insertar_autorizacion.Parameters.AddWithValue("@idetapa", autorizacion.idetapa);
                    insertar_autorizacion.Parameters.AddWithValue("@iddevcab", autorizacion.iddevcab);
                    insertar_autorizacion.Parameters.AddWithValue("@tipo_aprobador", autorizacion.tipo_aprobador);

                    if (String.IsNullOrEmpty(autorizacion.aprobador))
                    {
                        insertar_autorizacion.Parameters.AddWithValue("@aprobador", DBNull.Value);
                    }
                    else
                    {
                        insertar_autorizacion.Parameters.AddWithValue("@aprobador", autorizacion.aprobador);
                    }

                    insertar_autorizacion.Parameters.AddWithValue("@tipo_solicitante", autorizacion.tipo_solicitante);
                    insertar_autorizacion.Parameters.AddWithValue("@solicitante", autorizacion.solicitante);
                    insertar_autorizacion.Parameters.AddWithValue("@estado", autorizacion.estado);

                    if (String.IsNullOrEmpty(autorizacion.observacion))
                    {
                        insertar_autorizacion.Parameters.AddWithValue("@observacion", DBNull.Value);
                    }
                    else
                    {
                        insertar_autorizacion.Parameters.AddWithValue("@observacion", autorizacion.observacion);
                    }

                    if (String.IsNullOrEmpty(autorizacion.mensaje))
                    {
                        insertar_autorizacion.Parameters.AddWithValue("@mensaje", DBNull.Value);
                    }
                    else
                    {
                        insertar_autorizacion.Parameters.AddWithValue("@mensaje", autorizacion.mensaje);
                    }

                    if (String.IsNullOrEmpty(autorizacion.mensaje_respuesta))
                    {
                        insertar_autorizacion.Parameters.AddWithValue("@mensaje_respuesta", DBNull.Value);
                    }
                    else
                    {
                        insertar_autorizacion.Parameters.AddWithValue("@mensaje_respuesta", autorizacion.mensaje_respuesta);
                    }

                    insertar_autorizacion.Parameters.AddWithValue("@archivo", autorizacion.archivo);

                    if (String.IsNullOrEmpty(autorizacion.documento))
                    {
                        insertar_autorizacion.Parameters.AddWithValue("@documento", DBNull.Value);
                    }
                    else
                    {
                        insertar_autorizacion.Parameters.AddWithValue("@documento", autorizacion.documento);
                    }

                    insertar_autorizacion.Parameters.AddWithValue("@tipo_creacion", autorizacion.tipo_creacion);
                    insertar_autorizacion.Parameters.AddWithValue("@usuario_creacion", autorizacion.usuario_creacion);
                    insertar_autorizacion.Parameters.AddWithValue("@fecha_creacion", autorizacion.fecha_creacion);

                    if (String.IsNullOrEmpty(autorizacion.tipo_actualizacion))
                    {
                        insertar_autorizacion.Parameters.AddWithValue("@tipo_actualizacion", DBNull.Value);
                    }
                    else
                    {
                        insertar_autorizacion.Parameters.AddWithValue("@tipo_actualizacion", autorizacion.tipo_actualizacion);
                    }

                    if (String.IsNullOrEmpty(autorizacion.usuario_actualizacion))
                    {
                        insertar_autorizacion.Parameters.AddWithValue("@usuario_actualizacion", DBNull.Value);
                    }
                    else
                    {
                        insertar_autorizacion.Parameters.AddWithValue("@usuario_actualizacion", autorizacion.usuario_actualizacion);
                    }

                    if (String.IsNullOrEmpty(autorizacion.fecha_actualizacion))
                    {
                        insertar_autorizacion.Parameters.AddWithValue("@fecha_actualizacion", DBNull.Value);
                    }
                    else
                    {
                        insertar_autorizacion.Parameters.AddWithValue("@fecha_actualizacion", autorizacion.fecha_actualizacion);
                    }

                    insertar_autorizacion.Parameters.AddWithValue("@keyorganizacion", autorizacion.keyorganizacion);

                    insertar_autorizacion.ExecuteNonQuery();
                }

                // Todo salio bien

                respuesta.Estado = 1;
                respuesta.Mensaje = "La devolucion ( " + cabecera.iddevcab + " ), ha sido registrada";

                logs.grabarLog("registrarDevolucion", "La devolucion ( " + cabecera.iddevcab + " ), ha sido registrada");

            }
            catch (Exception e)
            {
                respuesta.Estado = 0;
                respuesta.Mensaje = e.Message;

                logs.grabarLog("registrarDevolucion", e.Message);
                logs.grabarLog("registrarDevolucion_DEBUG", e.StackTrace);
            }
            finally
            {
                DBSqlServer.DesconectaDB();
            }

            return respuesta;
        }
    }
}