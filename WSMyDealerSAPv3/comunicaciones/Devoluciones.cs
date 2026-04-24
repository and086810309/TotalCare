using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Data.SqlClient;
using System.Data;
using System.Data.Odbc;

namespace WSMyDealerSAPv3
{
    public class Devoluciones
    {

        public static String existeDevolucion(string numDevolucion)
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

            //DataBase.ConectaDB();
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
                // SAPbobsCOM.Recordset record;
                // record = (SAPbobsCOM.Recordset)DataBase.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                // record.DoQuery("select numorden, DocNum, tipo, numdevolucion from md_devoluciones WITH (NOLOCK) where numdevolucion='" + numDevolucion + "' ");
                // // record.DoQuery("select DocEntry, 'U_ita_cob_num', DocNum from ORCT  WITH (NOLOCK)");

                String sql = "select numorden, DocNum, tipo, numdevolucion from md_devoluciones WITH (NOLOCK) where numdevolucion='" + numDevolucion + "' ";

                OdbcCommand com = new OdbcCommand(sql, DBSqlServer.Conexion);
                com.CommandType = CommandType.Text;
                OdbcDataReader record = com.ExecuteReader();

                // int y = record.RecordCount;
                //if (y > 0)
                if (record.HasRows) {
                    // record.MoveFirst();
                    retorno = retorno
                                .Replace("{0}", "false")
                                .Replace("{1}", "true")
                                .Replace("{2}", "")
                                .Replace("{3}", "")
                                .Replace("{4}", "La devolucion ya fue registrada")
                                // .Replace("{5}", record.Fields.Item("numorden").Value.ToString())
                                .Replace("{5}", record.GetValue(0).ToString())
                                .Replace("{6}", "false");
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
                // record = null;
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

                logs.grabarLog("DEVOLUCION", e.Message);
                logs.grabarLog("DEVOLUCION_DEBUG", e.StackTrace);
            }
            finally
            {
                DBSqlServer.DesconectaDB();
            }

            //GC.WaitForPendingFinalizers();
            //GC.Collect();

            return retorno;
        }



        public static String ingresarDevolucion(string devolucion)
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
                              "{7}" +
                          "</Respuesta>";
            SAPbobsCOM.Company company = null;

                company = DataBase.conectar();
            if (!DataBase.Respuesta.Exito)
            {
                retorno = retorno
                            .Replace("{0}", "false") // exito
                            .Replace("{1}", "false") // existe pedido
                            .Replace("{2}", DataBase.Respuesta.CodigoError) // codigo de error
                            .Replace("{3}", DataBase.Respuesta.CodigoRespuesta) // codigo de respuesta
                            .Replace("{4}", DataBase.Respuesta.DescripcionError) // descripcion respuesta
                            .Replace("{5}", "0") // numero pedido sap
                            .Replace("{6}", "true")
                            .Replace("{7}", "<Registro />"); // registro
                return retorno;
            }

            string sCmd = "", prefix_error = "";
            int error = 0; string mensaje = "";
            SAPbobsCOM.Documents oDoc = null;

            try
            {

                XmlDocument xmlDevolucion = new XmlDocument();
                xmlDevolucion.LoadXml(devolucion);

                // XmlNode mainDevolucion = xmlDevolucion.FirstChild;
                XmlNodeList mainDevolucion = xmlDevolucion.GetElementsByTagName("Devoluciones");
                XmlElement datosDevolucion = (XmlElement) mainDevolucion[0];


                if (datosDevolucion.HasChildNodes)
                {
                    if (datosDevolucion["PedirAutorizacion"].InnerText == "S")
                    {
                        // si se necesita aprobacion, se grabara como draft.
                        oDoc = (SAPbobsCOM.Documents)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts); //oDrafts
                        oDoc.DocObjectCode = SAPbobsCOM.BoObjectTypes.oReturns;
                    }
                    else
                    {
                        // si el pedido del cliente no necesita ser aprobado se va a la tabla de pedidos
                        oDoc = (SAPbobsCOM.Documents)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oReturns);
                    }

                    oDoc.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items;

                    prefix_error = " dev # " + datosDevolucion["Codevolucion"].InnerText + " :: ";

                    oDoc.DocDate = DateTime.Parse(datosDevolucion["FechaRegistro"].InnerText);


                    //oDoc.UserFields.Fields.Item("U_SER_EST_FR").Value = datosDevolucion["Establecimiento"].InnerText;
                    //oDoc.UserFields.Fields.Item("U_SER_PEFR").Value = datosDevolucion["PuntoEmision"].InnerText;
                    //oDoc.UserFields.Fields.Item("U_NUM_FAC_REL").Value = datosDevolucion["NumeroFactura"].InnerText;

                    //oDoc.UserFields.Fields.Item("U_MD_ORDEN").Value = datosDevolucion["Codevolucion"].InnerText;
                    //oDoc.UserFields.Fields.Item("U_MD_ORIGEN").Value = datosDevolucion["Origen"].InnerText;

                    oDoc.CardCode = datosDevolucion["Codcliente"].InnerText;

                    if ("" != datosDevolucion["Codvendedor"].InnerText)
                        oDoc.SalesPersonCode = Int32.Parse(datosDevolucion["Codvendedor"].InnerText);
                    oDoc.ShipToCode = datosDevolucion["Coddireccioncliente"].InnerText;

                    XmlNodeList items = datosDevolucion.GetElementsByTagName("Detalles");
                    XmlNodeList item = ((XmlElement)items[0]).GetElementsByTagName("Detalle");

                    int linea = 0; // SAPbobsCOM.Items prod = new SAPbobsCOM.Items();
                    foreach (XmlElement nodo in item) {
                        if (linea > 0) oDoc.Lines.Add();
                        oDoc.Lines.SetCurrentLine(linea);

                        oDoc.Lines.ItemCode = nodo.GetElementsByTagName("Codproducto")[0].InnerText;
                        oDoc.Lines.Quantity = XmlConvert.ToDouble(nodo.GetElementsByTagName("Cantidad")[0].InnerText);
                        oDoc.Lines.UnitPrice = XmlConvert.ToDouble(nodo.GetElementsByTagName("Precio")[0].InnerText);

                        //oDoc.Lines.UserFields.Fields.Item("U_JYO_MOT_DEVOL").Value = nodo.GetElementsByTagName("Motivo")[0].InnerText;
                        //oDoc.Lines.UserFields.Fields.Item("U_JYO_OBS_DEVOL").Value = nodo.GetElementsByTagName("Observacion")[0].InnerText;

                        sCmd = nodo.GetElementsByTagName("Codproducto")[0].InnerText + " - " + nodo.GetElementsByTagName("UnidadEntry")[0].InnerText;

                        if (XmlConvert.ToInt32(nodo.GetElementsByTagName("UnidadEntry")[0].InnerText) == 1) {
                            oDoc.Lines.UoMEntry = XmlConvert.ToInt32(nodo.GetElementsByTagName("UnidadEntry")[0].InnerText);
                            oDoc.Lines.InventoryQuantity = oDoc.Lines.Quantity;
                            // oDoc.Lines.InventoryQuantity = oDoc.Lines.Quantity * XmlConvert.ToDouble(nodo.GetElementsByTagName("UnidadCaja")[0].InnerText);
                        }


                        /*if (!prod.GetByKey(oDoc.Lines.ItemCode)) {
                            throw new Exception("Error al consultar el articulo.");
                        }*/

                        if (nodo.GetElementsByTagName("NumeroLote")[0].InnerText != "")
                        {
                            oDoc.Lines.BatchNumbers.SetCurrentLine(0);
                            oDoc.Lines.BatchNumbers.BatchNumber = nodo.GetElementsByTagName("NumeroLote")[0].InnerText;

                            if (XmlConvert.ToInt32(nodo.GetElementsByTagName("UnidadEntry")[0].InnerText) == 1)
                            {
                                oDoc.Lines.BatchNumbers.Quantity = oDoc.Lines.Quantity;
                            }
                            else
                            {
                                oDoc.Lines.BatchNumbers.Quantity = oDoc.Lines.Quantity * XmlConvert.ToDouble(nodo.GetElementsByTagName("UnidadCaja")[0].InnerText);
                            }
                        }

                        linea++;
                    }

                    /*
                     * 
                        Tabla Detalle: devolucionesdet
                            numlote

                     * [14:37:46] Danny Velastegui: ejemplo: cj24 es uomcode
                       [14:37:51] Danny Velastegui: pero internamente es el código 9
                     * 
                     * T0.[UomEntry], T0.[UomCode]
                     
                     */


                    error = oDoc.Add();
                    if (error != 0)
                    {
                        company.GetLastError(out error, out mensaje);
                        throw new Exception(error + " - " + mensaje + " :: " + sCmd);
                    }
                    else
                    {
                        retorno = retorno
                                    .Replace("{0}", "true") // exito
                                    .Replace("{1}", "false") // existe pedido
                                    .Replace("{2}", "") // codigo de error
                                    .Replace("{3}", "") // codigo de respuesta
                                    .Replace("{4}", "Registro grabado satisfactoriamente") // descripcion respuesta
                                    .Replace("{5}", company.GetNewObjectKey()) // numero pedido sap
                                    .Replace("{6}", "false")
                                    .Replace("{7}", "<Registro />"); // registro
                    }

                }
                else
                {
                    throw new Exception("Error al consultar la devolcuion");
                }

            }
            catch (Exception e)
            {
                retorno = retorno
                            .Replace("{0}", "false") // exito
                            .Replace("{1}", "false") // existe pedido
                            .Replace("{2}", error.ToString()) // codigo de error
                            .Replace("{3}", "DEV001 - " + mensaje) // codigo de respuesta
                            .Replace("{4}", e.Message) // descripcion respuesta
                            .Replace("{5}", "0") // numero pedido sap
                            .Replace("{6}", "true")
                            .Replace("{7}", "<Registro />"); // registro

                logs.grabarLog("DEVOLUCION", prefix_error + e.Message);
                logs.grabarLog("DEVOLUCION_DEBUG", e.StackTrace);
            }
            finally
            {
                DataBase.DesconectaDB(company);
            }

            if (oDoc != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(oDoc);
            oDoc = null;

            //GC.WaitForPendingFinalizers();
            //GC.Collect();

            return retorno;

        }
        
    }
}