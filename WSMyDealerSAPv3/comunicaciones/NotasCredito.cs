using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

using System.Xml;
using System.Data.Odbc;

namespace WSMyDealerSAPv3
{
    public class NotasCredito
    {

        public static String existeNotaCredito(string numSolicitud)
        {
            String retorno = "<?xml version='1.0' encoding='ISO-8859-1'?>" +
                          "<Respuesta>" +
                              "<Exito>{0}</Exito>" +
                              "<ExistePedido>{1}</ExistePedido>" +
                              "<CodigoError>{2}</CodigoError>" +
                              "<CodigoRespuesta>{3}</CodigoRespuesta>" +
                              "<DescripcionError>{4}</DescripcionError>" +
                              "<NumeroPedidoSAP>{5}</NumeroPedidoSAP>" +
                              "<ErrorConexion>{6}</ErrorConexion>" +
                              "<Registro />" +
                          "</Respuesta>";

            DBSqlServer.ConectaDB();
            if (!DBSqlServer.Respuesta.Exito)
            {
                retorno = retorno
                            .Replace("{0}", "false")
                            .Replace("{1}", "false")
                            .Replace("{2}", DBSqlServer.Respuesta.CodigoError)
                            .Replace("{3}", DBSqlServer.Respuesta.CodigoRespuesta)
                            .Replace("{4}", DBSqlServer.Respuesta.DescripcionError)
                            .Replace("{5}", "0")
                            .Replace("{6}", "true");
                return retorno;
            }

            // CONSULTAR A LA VISTA POR MEDIO DEL NUMERO DE PEDIDO DE MYDEALER.
            try
            {
                String sql = " select numorden, numordenweb, DocNum, tipo from md_notas_credito  with(nolock) where numordenweb=" + numSolicitud;

                OdbcCommand com = new OdbcCommand(sql, DBSqlServer.Conexion);
                com.CommandType = CommandType.Text;

                OdbcDataReader record = com.ExecuteReader();
                if (record.HasRows)
                {
                    // record.MoveFirst();
                    if (record.Read())
                    {
                        retorno = retorno
                                    .Replace("{0}", "false")
                                    .Replace("{1}", "true")
                                    .Replace("{2}", "")
                                    .Replace("{3}", "")
                                    .Replace("{4}", "La solicitud ya fue registrada")
                                    .Replace("{5}", record.GetValue(0).ToString())
                                    .Replace("{6}", "false");
                    }
                    else
                    {
                        throw new Exception("Error al al leer los datos. " + numSolicitud);
                    }
                }
                else
                {
                    retorno = retorno
                                .Replace("{0}", "true")
                                .Replace("{1}", "false")
                                .Replace("{2}", "")
                                .Replace("{3}", "")
                                .Replace("{4}", "")
                                .Replace("{5}", "0")
                                .Replace("{6}", "false");
                }

                record.Close(); record.Dispose(); record = null;
                com.Dispose(); com = null;

            }
            catch (Exception e)
            {
                retorno = retorno
                            .Replace("{0}", "false")
                            .Replace("{1}", "false")
                            .Replace("{2}", "")
                            .Replace("{3}", "")
                            .Replace("{4}", e.Message)
                            .Replace("{5}", "0")
                            .Replace("{6}", "true");

                logs.grabarLog("NCREDITO", e.Message);
                logs.grabarLog("NCREDITO_DEBUG", e.StackTrace);
            }
            finally
            {
                DBSqlServer.DesconectaDB();
            }

            return retorno;
        }

        // 'in (12.0, 14.0)
        private static String obtenerCodigoImpuesto(double iva_rate)
        {
            String retorno = "";

            DBSqlServer.ConectaDB();
            if (!DBSqlServer.Respuesta.Exito) throw new Exception(DBSqlServer.Respuesta.CodigoError + ": " + DBSqlServer.Respuesta.DescripcionError);

            string sql = " select code from OSTC where rate  = " + iva_rate;

            OdbcCommand com = new OdbcCommand(sql, DBSqlServer.Conexion);
            com.CommandType = CommandType.Text;

            OdbcDataReader record = com.ExecuteReader();
            if (record.HasRows)
            {

                if (record.Read())
                {
                    retorno = record.GetValue(0).ToString();
                    logs.grabarLog("sql_NCREDITO", "CODE IMPUESTO.: " + retorno);
                }
                else throw new Exception("Error al al leer los datos. ");
            }
            else
            {
                throw new Exception("Error al al leer los datos. ");
            }

            if (record != null) { record.Close(); record.Dispose(); record = null; }
            if (com != null) { com.Dispose(); com = null; }

            DBSqlServer.DesconectaDB();

            return retorno;
        }

        private static Double obtenerImpuestoFactura(string num_factura) {
            double retorno = 0;

            DBSqlServer.ConectaDB();
            if (!DBSqlServer.Respuesta.Exito) throw new Exception(DBSqlServer.Respuesta.CodigoError + ": " + DBSqlServer.Respuesta.DescripcionError);


           /* string sql = " select max(fd.VatPrcnt) porcentaje " +
                         " from OINV f with(nolock) " +
                         "   inner join inv1 fd with(nolock) on f.DocEntry = fd.DocEntry " +
                         " where f.DocSubType = '--' " +
                         "         and f.DocEntry = " + num_factura;*/

            
            string sql = " SELECT I.U_IVA_APLI " +
                         " FROM OINV F with(nolock) " +
                         "   INNER JOIN [@EXX_IVA_VAL] I with(nolock) ON F.TAXDATE BETWEEN I.U_Fecha_ini AND I.U_Fecha_fin " +
                         " where F.DocSubType = '--' " +
                         "         and F.DOCENTRY = " + num_factura;

            OdbcCommand com = new OdbcCommand(sql, DBSqlServer.Conexion);
            com.CommandType = CommandType.Text;

            OdbcDataReader record = com.ExecuteReader();
            if (record.HasRows)
            {
                
                if (record.Read()) {
                    retorno = Double.Parse(record.GetValue(0).ToString());
                    logs.grabarLog("sql_NCREDITO", "PORC.: " + retorno + " ::: \n" + sql);
                }
                else throw new Exception("Error al al leer los datos. ");
            }
            else
            {
                throw new Exception("Error al al leer los datos. ");
            }

            if (record != null) { record.Close(); record.Dispose(); record = null; }
            if (com != null) { com.Dispose(); com = null; }

            DBSqlServer.DesconectaDB();

            return retorno;
        }


        public static string ingresarNotaCredito(string xmlDatos)
        {

            String retorno = "<?xml version='1.0' encoding='ISO-8859-1'?>" +
                          "<Respuesta>" +
                              "<Exito>{0}</Exito>" +
                              "<ExistePedido>{1}</ExistePedido>" +
                              "<CodigoError>{2}</CodigoError>" +
                              "<CodigoRespuesta>{3}</CodigoRespuesta>" +
                              "<DescripcionError>{4}</DescripcionError>" +
                              "<NumeroPedidoSAP>{5}</NumeroPedidoSAP>" +
                              "<ErrorConexion>{6}</ErrorConexion>" +
                              "<Registros>{7}</Registros>" +
                              "<MaxRegistros>{8}</MaxRegistros>" +
                          "</Respuesta>";

            int error = 0; string mensaje = "", prefix_error = "", codsolicitud = "";
            SAPbobsCOM.Company company = null;


            SAPbobsCOM.Documents oDoc = null;
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(xmlDatos);

                XmlNodeList mainCliente = xml.GetElementsByTagName("NotasCredito");
                XmlElement datosCliente = (XmlElement)mainCliente[0];

                double total = XmlConvert.ToDouble(datosCliente["Monto"].InnerText);
                double impuesto = obtenerImpuestoFactura(datosCliente["NumeroFactura"].InnerText);
                double subtotal = total;
                String code_impuesto = "";

                if (impuesto > 0) {
                    subtotal = (total / (1 + (impuesto / 100)));
                    subtotal = Math.Round(subtotal, 2);

                    code_impuesto = obtenerCodigoImpuesto(impuesto);
                }


                //datosCliente["NumeroFactura"].InnerText;




                company = DataBase.conectar();
                if (!DataBase.Respuesta.Exito) throw new Exception("Error al conectarse a la Compañia");


                codsolicitud = datosCliente["NumeroSolicitud"].InnerText;

                oDoc = (SAPbobsCOM.Documents)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts); //oDrafts
                oDoc.DocObjectCode = SAPbobsCOM.BoObjectTypes.oCreditNotes;
                oDoc.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Service;

                oDoc.CardCode = datosCliente["CodigoCliente"].InnerText;
                oDoc.DocDate = DateTime.Parse(datosCliente["Fecha"].InnerText);
                oDoc.SalesPersonCode = Int32.Parse(datosCliente["CodigoVendedor"].InnerText);
                oDoc.Comments = datosCliente["Observaciones"].InnerText;

                //oDoc.UserFields.Fields.Item("U_MD_NC_FACTURA").Value = datosCliente["NumeroFactura"].InnerText;
                //oDoc.UserFields.Fields.Item("U_MD_NC_CODTIPO").Value = datosCliente["CodigoTipoNotaCredito"].InnerText;
                //oDoc.UserFields.Fields.Item("U_MD_NC_PORCENT").Value = datosCliente["Porcentaje"].InnerText;
                //oDoc.UserFields.Fields.Item("U_MD_NC_FECHA_PAGO").Value = DateTime.Parse(datosCliente["FechaPago"].InnerText);
                //oDoc.UserFields.Fields.Item("U_tipo_comprob").Value = "04";
                //oDoc.UserFields.Fields.Item("U_COD_ST").Value = "00";

                //oDoc.UserFields.Fields.Item("U_MD_NC_CODPORCENT").Value = datosCliente["CodigoPorcentaje"].InnerText;

                oDoc.JournalMemo = "Origen: MD - Solicitud: " + codsolicitud + " - Porcentaje: " + datosCliente["Porcentaje"].InnerText;

                

                //oDoc.UserFields.Fields.Item("U_MD_ORDEN").Value = codsolicitud;
                //oDoc.UserFields.Fields.Item("U_MD_ORIGEN").Value = "MD";



                oDoc.Lines.SetCurrentLine(0);
                oDoc.Lines.ItemDescription = DatosEnlace.glosaNotaCredito;
                oDoc.Lines.AccountCode = DatosEnlace.cuetaNotaCredito;
                oDoc.Lines.LineTotal = subtotal;

                if (!String.Empty.Equals(code_impuesto)) oDoc.Lines.TaxCode = code_impuesto;

                // oDoc.DocTotal = Double.Parse(datosCliente["Monto"].InnerText);


                error = oDoc.Add();
                if (error != 0)
                {
                    company.GetLastError(out error, out mensaje);
                    throw new Exception(" ** " + error + ": " + mensaje + " :: " + codsolicitud);
                }
                // oDoc = null;


                retorno = retorno
                            .Replace("{0}", "true")
                            .Replace("{1}", "")
                            .Replace("{2}", "")
                            .Replace("{3}", "")
                            .Replace("{4}", "")
                            .Replace("{5}", company.GetNewObjectKey())
                            .Replace("{6}", "false")
                            .Replace("{7}", "")
                            .Replace("{8}", "");

            }
            catch (Exception e)
            {
                retorno = retorno
                            .Replace("{0}", "false")
                            .Replace("{1}", "false")
                            .Replace("{2}", "")
                            .Replace("{3}", "")
                            .Replace("{4}", e.Message)
                            .Replace("{5}", codsolicitud)
                            .Replace("{6}", "true")
                            .Replace("{7}", e.StackTrace)
                            .Replace("{8}", "0");

                logs.grabarLog("NCREDITO", prefix_error + e.Message); //  + company.GetLastErrorDescription()
                logs.grabarLog("NCREDITO_DEBUG", e.ToString());

            }
            finally
            {
                DataBase.DesconectaDB(company);
            }

            if (oDoc != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(oDoc);
            oDoc = null;


            return retorno;
        }

    }
}