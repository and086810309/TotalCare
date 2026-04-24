using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Data.Odbc;

namespace WSMyDealerSAPv3
{
    public static class NotasDebito
    {
        public static bool existeNotaDebito(string numDebito)
        {

            DBSqlServer.ConectaDB();


            try
            {
                String sql = "SELECT * FROM \"OINV\" WHERE \"DocNum\" = "+numDebito+";";

                OdbcCommand com = new OdbcCommand(sql, DBSqlServer.Conexion);
                com.CommandType = CommandType.Text;

                OdbcDataReader record = com.ExecuteReader();
                if (record.HasRows)
                {
                    if (record.Read())
                    {
                        //existe
                        return true;
                    }
                }

                record.Close(); record.Dispose(); record = null;
                com.Dispose(); com = null;

            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                DBSqlServer.DesconectaDB();
            }

            return false;
        }

        public static List<String> IngresarNotaDeDebitoCliente(CabeceraNotaDeDebito cabecera, List<DetalleNotaDeDebito> detalles )
        {
            List<String> response = new List<String>();
            int error = 0;
            string mensaje = "", prefix_error = "";
            string lqdebug = "";
            SAPbobsCOM.Company company = null;

            company = DataBase.conectar();

            if (!DataBase.Respuesta.Exito)
            {
                response.Add("Error");
                response.Add(DataBase.Respuesta.CodigoError);
                response.Add(DataBase.Respuesta.CodigoRespuesta);
                response.Add(DataBase.Respuesta.DescripcionError);
                return response;
            }

            SAPbobsCOM.Documents oDoc = null;

            try
            {
                //oDoc = (SAPbobsCOM.Documents)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes);
                //oDoc = (SAPbobsCOM.Documents)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
                oDoc = (SAPbobsCOM.Documents)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);

                prefix_error = "Nota de débito # " + cabecera.NumeroNotaDeDebito + " :: ";

                oDoc.Series = cabecera.Series;
                //oDoc.Series = "NDE01";
                oDoc.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Service;
                //oDoc.DocumentSubType = SAPbobsCOM.BoDocumentSubType.bod_ExportInvoice;
                //oDoc.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_CredItnote;
                oDoc.DocDate = DateTime.Parse(cabecera.FechaGeneracionNotaDeDebito);
                oDoc.DocDueDate = DateTime.Parse(cabecera.FechaVencimientoNotaDeDebito);
                //oDoc.TaxDate = DateTime.Parse(cabecera.FechaGeneracionNotaDeDebito);
                oDoc.CardCode = cabecera.CodigoCliente;
                oDoc.Comments = cabecera.Observaciones;
                oDoc.NumAtCard = cabecera.CodigoRecibo;
                //oDoc.JournalMemo = cabecera.NumeroNotaDeDebito;
                //oDoc.UserFields.Fields.Item("U_CodigoRecibo").Value = cabecera.CodigoRecibo; // Asigna el valor al campo de usuario U_CodigoRecibo



                // Configuración del valor total de la nota de débito
                oDoc.DocTotal = cabecera.ValorTotal;  
                //oDoc.DocTotalFc = cabecera.ValorTotal;
                if (detalles != null)
                {
                    int linea = 0;
                    foreach (DetalleNotaDeDebito detalle in detalles)
                    {
                        //oDoc.Lines.SetCurrentLine(linea);
                        //oDoc.Lines.BaseType = (int)SAPbobsCOM.BoAPARDocumentTypes.bodt_PurchaseCreditNote;
                        //oDoc.Lines.BaseEntry = int.Parse(cabecera.CodigoRecibo);
                        //oDoc.Lines.BaseLine = linea; 

                        //oDoc.Lines.WarehouseCode = detalle.Wsc;
                        oDoc.Lines.ItemCode = "";//detalle.CodigoProducto;
                        //oDoc.Lines.Quantity = detalle.CantidadProducto;
                        oDoc.Lines.ItemDescription = "0225-N/D POR DESCUENTOS GENERALES";
                        oDoc.Lines.Price = cabecera.ValorTotal;
                        oDoc.Lines.UnitPrice = cabecera.ValorTotal;
                        oDoc.Lines.TaxCode = "IVA";
                        //oDoc.Lines.Add();
                        linea++;
                    }
                }

                error = oDoc.Add();

                if (error != 0)
                {
                    mensaje += " ::: " + lqdebug;
                    company.GetLastError(out error, out mensaje);
                    logs.grabarLog("NOTAS_DEBITO", "NotaDeDebito:" + cabecera.NumeroNotaDeDebito.ToString() + " ** " + error + ": " + mensaje);
                    throw new Exception(" ** " + error + ": " + mensaje + " :: " + cabecera.NumeroNotaDeDebito);
                }
                else
                {
                    string key = company.GetNewObjectKey();
                    oDoc.GetByKey(int.Parse(key));
                    string xDocNum = oDoc.DocNum.ToString();

                    logs.grabarLog("NOTAS_DEBITO", "NotaDeDebito:" + cabecera.NumeroNotaDeDebito.ToString() + " NotaSap:" + xDocNum);
                    response.Add("Success");
                    response.Add(xDocNum);
                    response.Add("");
                }
            }
            catch (Exception e)
            {
                response.Clear();
                response.Add("Error");
                response.Add(error.ToString());
                response.Add("NOTA001");
                response.Add(e.Message);

                logs.grabarLog("NOTAS_DEBITO", prefix_error + e.Message + company.GetLastErrorDescription() + " ::: " + lqdebug);
                logs.grabarLog("NOTAS_DEBITO_DEBUG", e.ToString());
            }
            finally
            {
                DataBase.DesconectaDB(company);
            }

            try
            {
                if (oDoc != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(oDoc);
                oDoc = null;
            }
            catch (Exception)
            {
            }

            return response;
        }
        public static List<String> IngresarNotaDeDebitoClienteTest(CabeceraNotaDeDebito cabecera, List<DetalleNotaDeDebito> detalles)
        {
            List<String> response = new List<String>();
            int error = 0;
            string mensaje = "", prefix_error = "";
            string lqdebug = "";
            SAPbobsCOM.Company company = null;

            company = DataBase.conectar();

            if (!DataBase.Respuesta.Exito)
            {
                response.Add("Error");
                response.Add(DataBase.Respuesta.CodigoError);
                response.Add(DataBase.Respuesta.CodigoRespuesta);
                response.Add(DataBase.Respuesta.DescripcionError);
                return response;
            }

            SAPbobsCOM.Documents oDoc = null;

            try
            {
                oDoc = (SAPbobsCOM.Documents)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                //oDoc = (SAPbobsCOM.Documents)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes);
                
                prefix_error = "Nota de débito # " + cabecera.NumeroNotaDeDebito + " :: ";

                oDoc.Series = 80;
                oDoc.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Service;
                oDoc.PaymentMethod = "CHQGYE";
                oDoc.DocumentSubType = SAPbobsCOM.BoDocumentSubType.bod_DebitMemo;
                //
                oDoc.CardCode = "0909083974";
                oDoc.DocDate = DateTime.Now;
                oDoc.NumAtCard = "42";
                oDoc.DocTotal = 500;
                //oDoc.SalesPersonCode = 2;

                //oDoc.Lines.ItemCode = "A-924865";
                oDoc.Lines.ItemCode = "";
                oDoc.Lines.LineStatus = SAPbobsCOM.BoStatus.bost_Open;
                oDoc.Lines.BaseType = -1;
                oDoc.Lines.ItemDescription = "0219 - N / D AJUSTES DE ANTIGUEDAD DE CARTERA";


                oDoc.Lines.Quantity = 10;
                oDoc.Lines.WarehouseCode = "BGG";
                oDoc.Lines.Price = 500;
                oDoc.Lines.UnitPrice = 500;
                oDoc.Lines.LineTotal = 500;
                //oDoc.Lines.splcode = 362; ojo es la lista de precio
                oDoc.Lines.GrossTotal = 500;
                oDoc.Lines.PriceAfterVAT = 500;
                oDoc.Lines.TaxCode = "IVA";
                oDoc.Lines.LineType = SAPbobsCOM.BoDocLineType.dlt_Regular;
                //oDoc.Lines.ItemType = SAPbobsCOM.BoDocItemType.dit_Resource;
                oDoc.Lines.GrossProfitTotalBasePrice = 500;
                oDoc.Lines.TaxTotal = (500/1.12);
                oDoc.Lines.COGSCostingCode2 = "FERRET";
                oDoc.Lines.COGSCostingCode3 = "FINANC";
                oDoc.DiscountPercent = 0;

                error = oDoc.Add();

                if (error != 0)
                {
                    mensaje += " ::: " + lqdebug;
                    company.GetLastError(out error, out mensaje);
                    logs.grabarLog("NOTAS_DEBITO", "NotaDeDebito:" + cabecera.NumeroNotaDeDebito.ToString() + " ** " + error + ": " + mensaje);
                    throw new Exception(" ** " + error + ": " + mensaje + " :: " + cabecera.NumeroNotaDeDebito);
                }
                else
                {
                    string key = company.GetNewObjectKey();
                    oDoc.GetByKey(int.Parse(key));
                    string xDocNum = oDoc.DocNum.ToString();

                    logs.grabarLog("NOTAS_DEBITO", "NotaDeDebito:" + cabecera.NumeroNotaDeDebito.ToString() + " NotaSap:" + xDocNum);
                    response.Add("Success");
                    response.Add(xDocNum);
                    response.Add("");
                }
            }
            catch (Exception e)
            {
                response.Clear();
                response.Add("Error");
                response.Add(error.ToString());
                response.Add("NOTA001");
                response.Add(e.Message);

                logs.grabarLog("NOTAS_DEBITO", prefix_error + e.Message + company.GetLastErrorDescription() + " ::: " + lqdebug);
                logs.grabarLog("NOTAS_DEBITO_DEBUG", e.ToString());
            }
            finally
            {
                DataBase.DesconectaDB(company);
            }

            try
            {
                if (oDoc != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(oDoc);
                oDoc = null;
            }
            catch (Exception)
            {
            }

            return response;
        }
        public static List<String> IngresarNotaDeDebitoClienteSimple(CabeceraNotaDeDebito cabecera, List<DetalleNotaDeDebito> detalles)
        {
            List<String> response = new List<String>();
            int error = 0;
            string mensaje = "", prefix_error = "";
            string lqdebug = "";
            SAPbobsCOM.Company company = null;

            company = DataBase.conectar();

            if (!DataBase.Respuesta.Exito)
            {
                response.Add("Error");
                response.Add(DataBase.Respuesta.CodigoError);
                response.Add(DataBase.Respuesta.CodigoRespuesta);
                response.Add(DataBase.Respuesta.DescripcionError);
                return response;
            }

            SAPbobsCOM.Documents oDoc = null;

            try
            {
                oDoc = (SAPbobsCOM.Documents)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                //oDoc = (SAPbobsCOM.Documents)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes);

                prefix_error = "Nota de débito # " + cabecera.NumeroNotaDeDebito + " :: ";

                oDoc.Series = 80;
                oDoc.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Service;
                //
                oDoc.CardCode = "C00008";
                oDoc.DocDate = DateTime.Now;
                oDoc.NumAtCard = "42";
                oDoc.DocTotal = 500;
                //oDoc.SalesPersonCode = 2;

                //oDoc.Lines.ItemCode = "A-924865";
                oDoc.Lines.ItemCode = "";
                oDoc.Lines.AccountCode = "1010102006";
                //oDoc.Lines.BaseType = -1;
                oDoc.Lines.ItemDescription = "AJUSTES DE ANTIGUEDAD DE CARTERA";


                oDoc.Lines.Quantity = 10;
                //oDoc.Lines.WarehouseCode = "BGG";
                oDoc.Lines.Price = 500;
                oDoc.Lines.UnitPrice = 500;
                oDoc.Lines.LineTotal = 500;
                //oDoc.Lines.splcode = 362; ojo es la lista de precio
                oDoc.Lines.TaxCode = "IVA";
               

                error = oDoc.Add();

                if (error != 0)
                {
                    mensaje += " ::: " + lqdebug;
                    company.GetLastError(out error, out mensaje);
                    logs.grabarLog("NOTAS_DEBITO", "NotaDeDebito:" + cabecera.NumeroNotaDeDebito.ToString() + " ** " + error + ": " + mensaje);
                    throw new Exception(" ** " + error + ": " + mensaje + " :: " + cabecera.NumeroNotaDeDebito);
                }
                else
                {
                    string key = company.GetNewObjectKey();
                    oDoc.GetByKey(int.Parse(key));
                    string xDocNum = oDoc.DocNum.ToString();

                    logs.grabarLog("NOTAS_DEBITO", "NotaDeDebito:" + cabecera.NumeroNotaDeDebito.ToString() + " NotaSap:" + xDocNum);
                    response.Add("Success");
                    response.Add(xDocNum);
                    response.Add("");
                }
            }
            catch (Exception e)
            {
                response.Clear();
                response.Add("Error");
                response.Add(error.ToString());
                response.Add("NOTA001");
                response.Add(e.Message);

                logs.grabarLog("NOTAS_DEBITO", prefix_error + e.Message + company.GetLastErrorDescription() + " ::: " + lqdebug);
                logs.grabarLog("NOTAS_DEBITO_DEBUG", e.ToString());
            }
            finally
            {
                DataBase.DesconectaDB(company);
            }

            try
            {
                if (oDoc != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(oDoc);
                oDoc = null;
            }
            catch (Exception)
            {
            }

            return response;
        }
        public static List<String> IngresarArticulo(CabeceraNotaDeDebito cabecera, List<DetalleNotaDeDebito> detalles)
        {
            List<String> response = new List<String>();
            int error = 0;
            string mensaje = "", prefix_error = "";
            string lqdebug = "";
            SAPbobsCOM.Company company = null;

            company = DataBase.conectar();

            if (!DataBase.Respuesta.Exito)
            {
                response.Add("Error");
                response.Add(DataBase.Respuesta.CodigoError);
                response.Add(DataBase.Respuesta.CodigoRespuesta);
                response.Add(DataBase.Respuesta.DescripcionError);
                return response;
            }

            //SAPbobsCOM.Documents oDoc = null;

            try
            {
                SAPbobsCOM.Items oItem = (SAPbobsCOM.Items)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems);

                // Configurar las propiedades del artículo
                oItem.ItemCode = "704774963";  // Código único del artículo
                oItem.ItemName = "Prueba AQ";
                oItem.BarCode = "12345cb";
                oItem.InventoryItem = SAPbobsCOM.BoYesNoEnum.tYES;  // Es un artículo de inventario
                oItem.PurchaseItem = SAPbobsCOM.BoYesNoEnum.tYES;  // Se puede comprar
                oItem.SalesItem = SAPbobsCOM.BoYesNoEnum.tYES;  // Se puede vender
                oItem.ManageSerialNumbers = SAPbobsCOM.BoYesNoEnum.tNO;  // No se manejan números de serie
                oItem.ManageBatchNumbers = SAPbobsCOM.BoYesNoEnum.tNO;  // No se manejan números de lote
                //oItem.UserFields.Fields.Item("U_CCMarca").Value = "ABRA";  // Campo personalizado
                //oItem.UserFields.Fields.Item("U_CCLinea").Value = "ACABA";
                //oItem.UserFields.Fields.Item("U_MARCA_ICE").Value = "A3";
                //oItem.UserFields.Fields.Item("U_CANTIDAD_MINIMA").Value = 1;
                //oItem.UserFields.Fields.Item("U_PORCMAXDSCTO").Value = 0;
                // Agregar el artículo
                error = oItem.Add();

                if (error != 0)
                {
                    string descri = company.GetLastErrorDescription();
                    Console.WriteLine($"Error al agregar el artículo: {company.GetLastErrorDescription()}");
                }
                else
                {
                    Console.WriteLine("Artículo agregado exitosamente.");
                }
            }
            catch (Exception e)
            {
                response.Clear();
                response.Add("Error");
                response.Add(error.ToString());
                response.Add("NOTA001");
                response.Add(e.Message);

                logs.grabarLog("NOTAS_DEBITO", prefix_error + e.Message + company.GetLastErrorDescription() + " ::: " + lqdebug);
                logs.grabarLog("NOTAS_DEBITO_DEBUG", e.ToString());
            }
            finally
            {
                DataBase.DesconectaDB(company);
            }

            return response;
        }

    }
}