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
    public class Cobranza
    {
        public static RespuestaExistenciaPedido existeCobranza(string numCobranza)
        {
            RespuestaExistenciaPedido response = new RespuestaExistenciaPedido();
            response.ExistePedido = false;
            response.NumeroPedidoSAP = "0";
            response.NumeroPedidoMyDealer = "0";
            response.DetalleError = "";

            DBSqlServer.ConectaDB();
            if (!DBSqlServer.Respuesta.Exito)
            {
                response.ExistePedido = true;
                response.DetalleError = DBSqlServer.Respuesta.DescripcionError;
                response.NumeroPedidoSAP = "0";
                response.NumeroPedidoMyDealer = "0";
                response.Estado = "";
                return response;
            }

            // CONSULTAR A LA VISTA POR MEDIO DEL NUMERO DE PEDIDO DE MYDEALER.
            try
            {
                String sql = " select \"DocNum\", \"CounterRef\", \"DocEntry\" from \"ORCT\" where \"U_MD_RECIBO\"='" + numCobranza + "'";

                OdbcCommand com = new OdbcCommand(sql, DBSqlServer.Conexion);
                com.CommandType = CommandType.Text;

                OdbcDataReader record = com.ExecuteReader();

                // int y = record.RecordCount;
                //if (y > 0)
                if (record.HasRows)
                {
                    if (record.Read())
                    {
                        response.ExistePedido = true;
                        response.NumeroPedidoSAP = record.GetValue(0).ToString();
                    }
                    //record.MoveFirst();
                    //response.NumeroPedidoSAP = record.Fields.Item("DocEntry").Value.ToString();
                    //r  response.NumeroPedidoMyDealer = record.Fields.Item("U_ita_cob_num").Value.ToString();
                }

                record.Close(); record.Dispose(); record = null;

                com.Dispose(); com = null;

            }
            catch (Exception e)
            {
                response.ExistePedido = true;
                response.DetalleError = e.Message;
                response.NumeroPedidoSAP = "0";
                response.NumeroPedidoMyDealer = "0";

                logs.grabarLog("COBRANZA_ERROR", e.Message);
                logs.grabarLog("COBRANZA_ERROR", e.StackTrace);
            }
            finally
            {
                DBSqlServer.DesconectaDB();
            }

            //GC.WaitForPendingFinalizers();
            //GC.Collect();

            return response;
        }

        public static string existeCobranzaPago(string numCobranza)
        {
            string retorno = "";
            RespuestaExistenciaPedido response = new RespuestaExistenciaPedido();
            response.ExistePedido = false;
            response.NumeroPedidoSAP = "0";
            response.NumeroPedidoMyDealer = "0";
            response.DetalleError = "";

            DBSqlServer.ConectaDB();
            if (!DBSqlServer.Respuesta.Exito)
            {
                retorno = "";
            }

            // CONSULTAR A LA VISTA POR MEDIO DEL NUMERO DE PEDIDO DE MYDEALER.
            try
            {
                String sql = " select \"DocNum\", \"CounterRef\", \"DocEntry\" from \"ORCT\" where \"U_MD_RECIBO\"='" + numCobranza + "'";

                OdbcCommand com = new OdbcCommand(sql, DBSqlServer.Conexion);
                com.CommandType = CommandType.Text;

                OdbcDataReader record = com.ExecuteReader();

                if (record.HasRows)
                {
                    if (record.Read())
                    {
                        retorno = record.GetValue(0).ToString();
                    }
                }

                record.Close(); record.Dispose(); record = null;

                com.Dispose(); com = null;

            }
            catch (Exception e)
            {
                retorno = "";

                logs.grabarLog("COBRANZA_ERROR", e.Message);
                logs.grabarLog("COBRANZA_ERROR", e.StackTrace);
            }
            finally
            {
                DBSqlServer.DesconectaDB();
            }

            //GC.WaitForPendingFinalizers();
            //GC.Collect();

            return retorno;
        }

        public void CortarTexto(ref string texto, int inicio, int fin)
        {
            if (texto.Length > fin)
                texto = texto.Substring(inicio, fin);
        }

        /**
         * Permite ingresar una cobranza en SBO
         * @param cabeceraPagos la cabecera comun de todos los pagos generados
         * @param documentos La lista de documentos a ingresar
         * @param pagos La lista de pagos a ingresar
         * @return El XML con la representacion de la respuesta
         */
        public static List<String> ingresarCobranza(SeccionPagosEfectivo cabeceraPagos, List<SeccionDocumentos> documentos, List<Pago> pagos)
        {
            //TestCobranza();
            List<String> response = new List<String>();
            SAPbobsCOM.Company company = null;

            logs.grabarLog("COBRANZA_LOG", "conexion:");
            company = DataBase.conectar();
            if (!DataBase.Respuesta.Exito)
            {
                response.Add("Error");
                response.Add(DataBase.Respuesta.CodigoError);
                response.Add(DataBase.Respuesta.CodigoRespuesta);
                response.Add(DataBase.Respuesta.DescripcionError);
                return response;
            }
            logs.grabarLog("COBRANZA_LOG", "inicio:" );

            string sCmd = "", cadena = "", pk = "";
            int error = 0; string mensaje = ""; string pista = "Pista00"; /*LQ 2021-06-25 para buscar pista en log de linea del error*/
            int cont = 0;
            SAPbobsCOM.Payments oDoc = null;
            pista = "Pista00a";

            try
            {
                company.StartTransaction();

                int linea_tar = 0; // rETENCIONES 

                double valortotalaplicar = 0;
                string sNumFac = "";
                string sNumFacAnt = "";
                string sTipoPAgo = "VariosTiposFpago";
                pista = "Pista00b";
                foreach (Pago pago in pagos)
                {
                    bool existe_pago = false; //Variable cuando exista duplicidad dejar continuar el grabado devolviendo la serie existente
                    logs.grabarLog("COBRANZA_LOG", "tipopago:" + pago.TipoPago);
                    //LQ Controlo que si llegan dos o mas bloques de retencion de la misma factura se interpreta que es el mismo recibo lo que se suma es un nuevo tipo de retencion en el mismo recibo
                    if (pago.TipoPago == "RET" || pago.TipoPago == "TAR")
                    {
                        sTipoPAgo = "RETENCIONES/TAR_CRED";
                        

                        string id = "";
                        id = pago.PagosConTarjeta.IdFP;

                        string DocNumDuplicidad = existeCobranzaPago(id);

                        if (!DocNumDuplicidad.Equals(""))
                        {
                            existe_pago = true;
                            cadena += DocNumDuplicidad + ",";
                        }
                        else
                        {
                            sNumFac = pago.PagosConTarjeta.NumeroReclamoFactura; //Numero de factura relacionada es la clave para ver si se crear
                            if (sNumFac != sNumFacAnt)
                            {

                                /*SI PASA A OTRA RETENCION GRABA-------> */
                                if (sNumFacAnt != "") {
                                    /******* INICIO GRABA RETENCION --> */
                                    pista = "retPista10";
                                    logs.grabarLog("COBRANZA_XML", oDoc.GetAsXML());
                                    pista = "retPista11";
                                    // Añadir a SAP **************************************************************
                                    error = oDoc.Add();
                                    if (error != 0)
                                    {
                                        pista += " -> retPista11a";
                                        company.GetLastError(out error, out mensaje);
                                        logs.grabarLog("COBRANZA_LOG", "ReciboWeb:" + cabeceraPagos.NumeroMBW + "- PK:" + id + " ** " + error + ": " + mensaje + pista);
                                        pista += " -> " + "ERRORSAP:" + mensaje;
                                        //throw new Exception(mensaje + " :: " + cabeceraPagos.NumeroMBW + " :: " + sCmd + " :: " + oDoc.DueDate);
                                        cadena = "";
                                        if (company.InTransaction)
                                            company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                                    }
                                    else
                                    {
                                        pista = "retPista11b";
                                        cont++;
                                        string key = company.GetNewObjectKey();
                                        oDoc.GetByKey(int.Parse(key));
                                        string xDocEntry = oDoc.DocEntry.ToString();
                                        string xDocNum = oDoc.DocNum.ToString();
                                        logs.grabarLog("COBRANZA_LOG", "ReciboWeb OK:" + cabeceraPagos.NumeroMBW + "- PK:" + id + " - DocEntry:" + xDocEntry + " - DocNum:" + xDocNum);
                                        cadena += xDocNum + ",";
                                    }
                                    /******* FIN GRABA RETENCION ------ **/
                                }
                                /* <-------SI PASA A OTRA RETENCION GRABA*/


                                sNumFacAnt = sNumFac;

                                /* CREO PAGO RET*/
                                linea_tar = 0;
                                pista = "RetPista00a";
                                oDoc = null;
                                oDoc = (SAPbobsCOM.Payments)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments);
                                pista += "-> U_MD_RECIBO :";

                                oDoc.UserFields.Fields.Item("U_MD_RECIBO").Value = pago.PagosConTarjeta.IdFP;
                                
                                oDoc.Series = int.Parse(cabeceraPagos.SerieRete);

                                /**** aplicar = pago.PagosConTarjeta.SumaCredito; ****/
                                 
                                pista = "retPista02";
                                oDoc.DocDate = DateTime.Now;
                                oDoc.TaxDate = DateTime.Parse(cabeceraPagos.FechaDocumento);
                                oDoc.CardCode = cabeceraPagos.CodigoCliente;
                                oDoc.CounterReference = cabeceraPagos.NumeroFisico;
                                oDoc.Reference2 = cabeceraPagos.NumeroMBW;
                                oDoc.Remarks = cabeceraPagos.Observaciones2;
                                oDoc.Reference1 = cabeceraPagos.Observaciones1;
                                oDoc.DueDate = DateTime.Now;
                                oDoc.JournalRemarks = "Origen: MD - Recibo:" + cabeceraPagos.NumeroMBW + " - Tipo:" + pago.PagosConTarjeta.Tipo_tarjeta;
                                oDoc.CashAccount = cabeceraPagos.CuentaCaja;
                                
                                //oDoc.CheckAccount = cabeceraPagos.CuentaCaja;
                                oDoc.BPLID = 2;
                                oDoc.DocType = SAPbobsCOM.BoRcptTypes.rCustomer;
                                pista = "retPista03";

                                /*APLICO PRIMER DOC DISPONIBLE*/
                                int linea = 0;
                                foreach (SeccionDocumentos documento in documentos)
                                {
                                    documento.SaldoValor = Math.Round(documento.SaldoValor, 4);  //LQ redondeo para eliminar decimales no deseados

                                    bool existedocumento = true;

                                    pista = "retPista05";
                                    if (documento.SePuedeUsar)
                                    {
                                        // Conocer saldo y total desde la base
                                        DBSqlServer.ConectaDB();
                                        //obtengo saldo de la cuota actual en sap
                                        //string sql = " select \"saldo\", \"total\" from \"MD_CTASXCOBRAR\" where \"DocEntry\"='" + documento.NumeroDocumento + "' and \"codcuota\"='" + documento.NumeroCuota + "'";
                                        string sql = " select \"saldo\", \"total\" from \"MD_CTASXCOBRAR\" where \"docEntry\"='" + documento.NumeroDocumento + "' and \"codcuota\"='" + documento.NumeroCuota + "' and \"tipo_doc\"='" + documento.TipoDocumento + "'";
                                        logs.grabarLog("COBRANZA_LOG", "SQLsaldocuota:" + sql);
                                        OdbcCommand com = new OdbcCommand(sql, DBSqlServer.Conexion);
                                        com.CommandType = CommandType.Text;
                                        OdbcDataReader record = com.ExecuteReader();
                                        string saldodoc = "", totaldoc = "";
                                        if (record.HasRows)
                                        {
                                            if (record.Read())
                                            {
                                                saldodoc = record.GetValue(0).ToString().Trim();
                                                totaldoc = record.GetValue(1).ToString().Trim();
                                                logs.grabarLog("COBRANZA_LOG", "existedocumento = true");
                                            }
                                            else
                                            {
                                                logs.grabarLog("COBRANZA_LOG", "existedocumento = false");
                                                existedocumento = false;
                                            }

                                        }
                                        else
                                        {
                                            logs.grabarLog("COBRANZA_LOG", "ELSe existedocumento = false");
                                            existedocumento = false;
                                        }

                                        /*LQ cierro el record sin importar si existe o no documento*/
                                        record.Close();
                                        record.Dispose();
                                        record = null;
                                        com.Dispose();
                                        com = null;

                                        pista = "retPista06";

                                        if (existedocumento)
                                        {
                                            if (documento.SaldoValor > 0)
                                            {
                                                documento.SaldoValor = Math.Round(documento.SaldoValor, 4); //elimino decimales no deseados

                                                /*controlo que valor aplicar al documento... si saldo del documento es menor a la retencion se aplica el saldo del documento la diferencia SAP debe dejar saldo a cuenta */
                                                if (double.Parse(saldodoc) < documento.SaldoValor)
                                                {
                                                    documento.SaldoValor = double.Parse(saldodoc);  /**/
                                                }

                                                oDoc.Invoices.SetCurrentLine(linea);
                                                oDoc.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_Invoice;
                                                oDoc.Invoices.DocEntry = documento.NumeroDocumento;
                                                oDoc.Invoices.DocLine = documento.NumeroCuota;
                                                oDoc.Invoices.InstallmentId = documento.NumeroCuotaSAP;
                                                oDoc.Invoices.SumApplied = documento.SaldoValor;

                                                documento.SaldoValor = 0;
                                                documento.SePuedeUsar = false;

                                                logs.grabarLog("COBRANZA_LOG", "documento:" + documento.NumeroDocumento + "/" + documento.NumeroCuota + " SaldoValor:" + documento.SaldoValor + " Aplicado:" + oDoc.Invoices.SumApplied);
                                                pista = "retPista08";


                                                linea++;
                                                break;   /* saldo del loop de documentos... me basta 1 doc de la retencion */

                                            }
                                        }
                                    }
                                } /* FIN APLICAR DOC*/

                            } /* FIN RET CAB*/

                            if(sNumFac == "")
                            {
                                oDoc = null;
                            }
                            if(oDoc == null)
                            {
                                linea_tar = 0;
                                //Cuando no hay factura relacionada
                                oDoc = (SAPbobsCOM.Payments)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments);
                                oDoc.UserFields.Fields.Item("U_MD_RECIBO").Value = pago.PagosConTarjeta.IdFP;

                                oDoc.Series = int.Parse(cabeceraPagos.SerieRete);

                                pista = "PistaFactNoRel";
                                oDoc.DocDate = DateTime.Now;
                                oDoc.TaxDate = DateTime.Parse(cabeceraPagos.FechaDocumento);
                                oDoc.CardCode = cabeceraPagos.CodigoCliente;
                                oDoc.CounterReference = cabeceraPagos.NumeroFisico;
                                oDoc.Reference2 = cabeceraPagos.NumeroMBW;
                                oDoc.Remarks = cabeceraPagos.Observaciones2 ;
                                oDoc.Reference1 = cabeceraPagos.Observaciones1;
                                oDoc.DueDate = DateTime.Now;
                                oDoc.JournalRemarks = "Origen: MD - Recibo:" + cabeceraPagos.NumeroMBW + " - Tipo:" + pago.PagosConTarjeta.Tipo_tarjeta;
                                oDoc.CashAccount = cabeceraPagos.CuentaCaja;

                                oDoc.BPLID = 2;
                                oDoc.DocType = SAPbobsCOM.BoRcptTypes.rCustomer;
                            }
                            /* DETALLE DE RETENCIONES */
                            pista = "retPista09";
                            if (linea_tar > 0) oDoc.CreditCards.Add();

                            if (pago.TipoPago == "TAR")
                            {
                                oDoc.CreditCards.SetCurrentLine(linea_tar);
                                oDoc.CreditCards.CreditCard = pago.PagosConTarjeta.TarjetaCredito;
                                //oDoc.CreditCards.CreditAcct = pago.PagosConTarjeta.CuentaTarjetaCredito;
                                oDoc.CreditCards.CreditCardNumber = pago.PagosConTarjeta.NumeroTarjeta;
                                oDoc.CreditCards.CardValidUntil = DateTime.Parse(pago.PagosConTarjeta.FechaVencAutorizacion);
                                //oDoc.CreditCards.OwnerIdNum = pago.PagosConTarjeta.NumeroAutorizacion;
                                oDoc.CreditCards.OwnerIdNum = pago.PagosConTarjeta.NumeroReclamoFactura;
                                if (pago.PagosConTarjeta.NumeroReclamoFactura != "")
                                {
                                    oDoc.CreditCards.VoucherNum = pago.PagosConTarjeta.NumeroReclamoFactura;
                                }
                                else
                                {
                                    oDoc.CreditCards.VoucherNum = pago.PagosConTarjeta.NumeroAutorizacion;
                                }
                                
                                oDoc.CreditCards.UserFields.Fields.Item("U_NUM_VOUCHER").Value = pago.PagosConTarjeta.NumeroAutorizacion;
                                oDoc.CreditCards.UserFields.Fields.Item("U_CXS_NUM_LOTE").Value = pago.PagosConTarjeta.Lote;
                                oDoc.CreditCards.FirstPaymentDue = DateTime.Today; //va la fecha en q se genera el pago de la retención en SAP
                                oDoc.CreditCards.OwnerPhone = pago.PagosConTarjeta.SetieRetencion;
                                oDoc.CreditCards.PaymentMethodCode = 1; //siempre va 1
                                oDoc.CreditCards.NumOfPayments = 1; //siempre va 1
                                oDoc.CreditCards.NumOfCreditPayments = 1; //siempre va 1
                                oDoc.CreditCards.CreditSum = pago.PagosConTarjeta.SumaCredito;
                                //oDoc.CreditCards.lote
                                //oDoc.CreditCards.UserFields.Fields.Item("U_MONTO_BASE").Value = pago.PagosConTarjeta.BaseImponible;
                            }
                            else
                            {
                                oDoc.CreditCards.SetCurrentLine(linea_tar);
                                oDoc.CreditCards.CreditCard = pago.PagosConTarjeta.TarjetaCredito;
                                oDoc.CreditCards.CreditAcct = pago.PagosConTarjeta.CuentaTarjetaCredito;
                                oDoc.CreditCards.CreditCardNumber = pago.PagosConTarjeta.NumeroTarjeta;
                                oDoc.CreditCards.CardValidUntil = DateTime.Parse(pago.PagosConTarjeta.FechaVencAutorizacion);
                                oDoc.CreditCards.OwnerIdNum = pago.PagosConTarjeta.NumeroAutorizacion;
                                oDoc.CreditCards.VoucherNum = pago.PagosConTarjeta.NumeroReclamoFactura;
                                oDoc.CreditCards.FirstPaymentDue = DateTime.Today; //va la fecha en q se genera el pago de la retención en SAP
                                oDoc.CreditCards.OwnerPhone = pago.PagosConTarjeta.SetieRetencion;
                                oDoc.CreditCards.PaymentMethodCode = 1; //siempre va 1
                                oDoc.CreditCards.NumOfPayments = 1; //siempre va 1
                                oDoc.CreditCards.NumOfCreditPayments = 1; //siempre va 1
                                oDoc.CreditCards.CreditSum = pago.PagosConTarjeta.SumaCredito;
                                //oDoc.CreditCards.lote
                                //oDoc.CreditCards.UserFields.Fields.Item("U_MONTO_BASE").Value = pago.PagosConTarjeta.BaseImponible;
                            }

                            pista += "-> U_REPL_NUM_RETE :";

                            //oDoc.CreditCards.UserFields.Fields.Item("U_REPL_NUM_RETE").Value = pago.PagosConTarjeta.NumeroTarjeta; /*LQ 210801*/
                            linea_tar++;

                        }



                        /*fin del bloque retenciones*/



                    }
                    else
                    {
                        //Si no es la froma de pago retencion siempre se crea un recibo por cada forma de pago
                        oDoc = null;
                        oDoc = (SAPbobsCOM.Payments)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments);


                    int linea = 0;

                    pista = "Pista00c";
                    //Obtener cantidad a aplicar
                    double aplicar = 0, aplicardoc = 0, anticipo = 0; /*lq 210718 anticipo*/
                    string id = "";
                    switch (pago.TipoPago)
                    {
                        case "EFE":
                            pista = "Pista00defe -> "+ cabeceraPagos.MontoEfectivo;
                            aplicar = cabeceraPagos.MontoEfectivo;
                            pista += " -> U_MD_RECIBO  " + cabeceraPagos.IdFP;
                            oDoc.UserFields.Fields.Item("U_MD_RECIBO").Value = cabeceraPagos.IdFP;
                            pista += " -> serie :" + cabeceraPagos.Serie;
                            oDoc.Series = int.Parse(cabeceraPagos.Serie);
                            id = cabeceraPagos.IdFP;
                            break;

                        case "DEP":
                            pista = "Pista00ddep";
                            //aplicar = cabeceraPagos.SumaTransferencia;
                            //NO DESCOMENTAR
                            //oDoc.UserFields.Fields.Item("U_MD_RECIBO").Value = cabeceraPagos.IdFP;
                            //id = cabeceraPagos.IdFP;

                            aplicar = pago.PagosConCheques.SumaValorCheques;
                            pista = "Pista00ddep set U_MD_RECIBO " + pago.PagosConCheques.IdFP;
                            oDoc.UserFields.Fields.Item("U_MD_RECIBO").Value = pago.PagosConCheques.IdFP;
                            id = pago.PagosConCheques.IdFP;
                            oDoc.Series = int.Parse(cabeceraPagos.Serie);
                            break;

                         case "CHE":
                            pista = "Pista00f";
                            aplicar = pago.PagosConCheques.SumaValorCheques;
                            pista = "Pista00f1 set U_MD_RECIBO " + pago.PagosConCheques.IdFP;
                            oDoc.UserFields.Fields.Item("U_MD_RECIBO").Value = pago.PagosConCheques.IdFP;
                            pista = "Pista00f2";
                            id = pago.PagosConCheques.IdFP;
                            oDoc.Series = int.Parse(cabeceraPagos.Serie);
                            break;
                        case "LET":
                            aplicar = pago.PagosConLetras.SumaValorLetra;
                            pista = "Pista letra set U_MD_RECIBO " + pago.PagosConLetras.IdFP;
                            oDoc.UserFields.Fields.Item("U_MD_RECIBO").Value = pago.PagosConLetras.IdFP;
                            id = pago.PagosConLetras.IdFP;
                            oDoc.Series = int.Parse(cabeceraPagos.Serie);
                            break;
                    }
                    pista = "Pista01";
                    // Verificar duplicidad
                    string DocNumDuplicidad = existeCobranzaPago(id);

                    if (!DocNumDuplicidad.Equals(""))
                    {
                        cadena += DocNumDuplicidad + ",";
                    }
                    else
                    {
                        pista = "Pista02";
                        oDoc.DocDate = DateTime.Now;
                        oDoc.TaxDate = DateTime.Parse(cabeceraPagos.FechaDocumento);
                        oDoc.CardCode = cabeceraPagos.CodigoCliente;
                        oDoc.CounterReference = cabeceraPagos.NumeroFisico;
                        oDoc.Reference2 = cabeceraPagos.NumeroMBW;
                        oDoc.Remarks = cabeceraPagos.Observaciones2;
                        oDoc.Reference1 = cabeceraPagos.Observaciones1;
                        oDoc.DueDate = DateTime.Now;
                        oDoc.JournalRemarks = "Origen: MD - Recibo:" + cabeceraPagos.NumeroMBW;
                        oDoc.BPLID = 2;
                        oDoc.CashAccount = cabeceraPagos.CuentaCaja;
                        oDoc.DocType = SAPbobsCOM.BoRcptTypes.rCustomer;


                        if (documentos.Count == 0)
                            aplicardoc = aplicar;

                        pista = "Pista03";
                        // Cantidad total
                        foreach (SeccionDocumentos documento in documentos)
                            valortotalaplicar += documento.Valor;

                        pista = "Pista04";
                        foreach (SeccionDocumentos documento in documentos)
                        {
                            documento.SaldoValor = Math.Round(documento.SaldoValor, 4);  //LQ redondeo para eliminar decimales no deseados

                            bool existedocumento = true;

                            pista = "Pista05";
                            if (documento.SePuedeUsar)
                            {
                                // Conocer saldo y total desde la base
                                DBSqlServer.ConectaDB();
                                //obtengo saldo de la cuota actual en sap
                                //string sql = " select \"saldo\", \"total\" from \"MD_CTASXCOBRAR\" where \"DocEntry\"='" + documento.NumeroDocumento + "' and \"codcuota\"='" + documento.NumeroCuota + "'";
                                string sql = " select \"saldo\", \"total\" from \"MD_CTASXCOBRAR\" where \"docEntry\"='" + documento.NumeroDocumento + "' and \"codcuota\"='" + documento.NumeroCuota + "' and \"tipo_doc\"='" + documento.TipoDocumento + "'";
                                OdbcCommand com = new OdbcCommand(sql, DBSqlServer.Conexion);
                                com.CommandType = CommandType.Text;
                                OdbcDataReader record = com.ExecuteReader();
                                string saldodoc = "", totaldoc = "";
                                if (record.HasRows)
                                {
                                    if (record.Read())
                                    {
                                        saldodoc = record.GetValue(0).ToString().Trim();
                                        totaldoc = record.GetValue(1).ToString().Trim();
                                        logs.grabarLog("COBRANZA_ERROR_SAP", "existedocumento = true");
                                    }
                                    else
                                    {
                                        logs.grabarLog("COBRANZA_ERROR_SAP", "existedocumento = false");
                                        existedocumento = false;
                                    }

                                }
                                else
                                {
                                    logs.grabarLog("COBRANZA_ERROR_SAP", "ELSe existedocumento = false");
                                    existedocumento = false;
                                }

                                pista = "Pista06";

                                if (existedocumento)
                                {
                                    switch (documento.TipoDocumento)
                                    {
                                        case "FAC":
                                            oDoc.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_Invoice;
                                            break;

                                        case "ND":
                                            oDoc.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_Invoice;
                                            break;

                                        case "CHQPRO":
                                            oDoc.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_PaymentAdvice;
                                            oDoc.Invoices.InstallmentId = 1;
                                            break;

                                        case "NDI2":
                                            oDoc.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_PaymentAdvice;
                                            oDoc.Invoices.InstallmentId = 1;
                                            break;

                                        case "NDI1":
                                            oDoc.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_Receipt;
                                            oDoc.Invoices.InstallmentId = 1;
                                            break;
                                    }

                                    pista = "Pista07";
                                    double saldocuota = 0;
                                    if (saldodoc != "")
                                        saldocuota = double.Parse(saldodoc);

                                    double total = 0;
                                    if (totaldoc != "")
                                        total = double.Parse(totaldoc);

                                    record.Close();
                                    record.Dispose();
                                    record = null;
                                    com.Dispose();
                                    com = null;
                                    //LQ - si el documento tiene algun saldo que aplicar analizo y aplico valor
                                    logs.grabarLog("COBRANZA_ERROR_SAP", "documento:" + documento.NumeroDocumento + "/" + documento.NumeroCuota + " SaldoValor:" + documento.SaldoValor);
                                    if (documento.SaldoValor > 0)
                                    {
                                        if (linea > 0) oDoc.Invoices.Add();
                                        oDoc.Invoices.SetCurrentLine(linea);
                                        oDoc.Invoices.DocEntry = documento.NumeroDocumento;
                                        oDoc.Invoices.DocLine = documento.NumeroCuota;
                                        oDoc.Invoices.InstallmentId = documento.NumeroCuotaSAP;
                                        //documento.SePuedeUsar = false;  // en el metodo aplicar lo que el vendedor escogio no se permite reutilizar cuotas
                                        pista = "Pista08";
                                        // Si el saldo de la cuota es mayor al valor que se va a aplicar a la cuota se aplica el valor tal como lo escogio el vendedor

                                        if (aplicar >= documento.SaldoValor)
                                        {
                                            //Verifico saldo real de la cuota para determinar si le puedo bajar 
                                            // aplicamos la cuota
                                            if (saldocuota >= documento.SaldoValor)
                                            {
                                                oDoc.Invoices.SumApplied = documento.SaldoValor;
                                            }
                                            else // se da de baja toda la cuota y queda un remanente que se usara mas adelante
                                            {
                                                oDoc.Invoices.SumApplied = saldocuota;
                                                anticipo += documento.SaldoValor - saldocuota;
                                                documento.SePuedeUsar = false;
                                            }
                                        }
                                        else // se da de baja toda la cuota y queda un remanente que se usara mas adelante
                                        {
                                            //Verificamos el saldo de la cuota
                                            if (saldocuota >= aplicar)
                                            {
                                                oDoc.Invoices.SumApplied = aplicar;
                                            }
                                            else // se da de baja toda la cuota y queda un remanente que se usara mas adelante
                                            {
                                                oDoc.Invoices.SumApplied = saldocuota;
                                                anticipo += aplicar - saldocuota;  /*LQ 210718 */
                                                documento.SePuedeUsar = false;
                                            }
                                        }
                                        documento.SaldoValor -= oDoc.Invoices.SumApplied;
                                        aplicardoc += oDoc.Invoices.SumApplied;
                                        aplicar -= oDoc.Invoices.SumApplied;
                                        valortotalaplicar -= oDoc.Invoices.SumApplied;

                                        /*eliminando decimales no deseados*/
                                        documento.SaldoValor = Math.Round(documento.SaldoValor, 4);
                                        aplicardoc           = Math.Round(aplicardoc, 4);
                                        aplicar              = Math.Round(aplicar, 4);
                                        valortotalaplicar    = Math.Round(valortotalaplicar, 4);


                                        logs.grabarLog("COBRANZA_LOG", "documento:" + documento.NumeroDocumento + "/" + documento.NumeroCuota + " aplicado " + oDoc.Invoices.SumApplied + " SaldoValor:" + documento.SaldoValor + " Anticipo" + anticipo);
                                        if (aplicar <= 0)
                                        {
                                            break;
                                        }

                                        linea++;
                                    }
                                }


                            }
                        }

                        if (documentos.Count > 0)
                        {
                            // anticipo
                            if (aplicar > 0)
                            {
                                logs.grabarLog("COBRANZA_LOG", "anticipo:" + aplicardoc + " " + aplicar);
                                aplicardoc += aplicar;
                            }
                        }


                        pista = "Pista09";
                        switch (pago.TipoPago)
                        {
                            case "EFE":
                                oDoc.CashSum = aplicardoc;
                                break;

                            case "DEP":

                                oDoc.TransferAccount = pago.PagosConCheques.NumeroCuentaBancaria;
                                oDoc.TransferSum = aplicardoc;
                                oDoc.TransferDate = DateTime.Parse(pago.PagosConCheques.FechaVencimiento);
                                oDoc.TransferReference = pago.PagosConCheques.NumeroAutorizacion;

                                /*oDoc.TransferAccount = cabeceraPagos.CuentaTransferencia;
                                oDoc.TransferSum = aplicardoc;
                                oDoc.TransferDate = DateTime.Parse(cabeceraPagos.FechaTransferencia);
                                oDoc.TransferReference = cabeceraPagos.ReferenciaTransferencia;*/

                                if (DateTime.Compare(oDoc.TransferDate, oDoc.DueDate) > 0) oDoc.DueDate = oDoc.TransferDate;
                                break;

  
                            case "CHE":
                                    //oDoc.BillOfExchange.BPBankCode = pago.PagosConCheques.CodigoBanco;
                                    //oDoc.BillOfExchange.BPBankCountry = pago.PagosConCheques.CodigoPais;
                                    //oDoc.BillOfExchange.BPBankAct = pago.PagosConCheques.NumeroCuentaBancaria;
                                    //oDoc.BillOfExchange.ReferenceNo = pago.PagosConCheques.NumeroCheques + "";
                                    //oDoc.BillOfExchange.Remarks = "CHQ POS # " + pago.PagosConCheques.NumeroCheques;
                                    ////LQ 20210727 pongo feha de fecimiento//  oDoc.BillOfExchange.BillOfExchangeDueDate = oDoc.DueDate;
                                    //oDoc.BillOfExchange.BillOfExchangeDueDate = DateTime.Parse(pago.PagosConCheques.FechaVencimiento);

                                    //oDoc.BillOfExchange.PaymentMethodCode = pago.PagosConLetras.MetodoPago;
                                    //oDoc.BillOfExchangeAmount = aplicardoc;
                                    oDoc.Checks.CheckSum = aplicardoc; // Monto del cheque
                                    oDoc.Checks.CountryCode = pago.PagosConCheques.CodigoPais;
                                    oDoc.Checks.BankCode = pago.PagosConCheques.CodigoBanco; // Código del banco
                                    oDoc.Checks.Branch = "BranchCode"; // Código de la sucursal
                                    oDoc.Checks.AccounttNum = pago.PagosConCheques.NumeroCuentaBancaria; // Número de cuenta
                                    oDoc.Checks.CheckNumber = pago.PagosConCheques.NumeroCheques; // Número del cheque
                                    oDoc.Checks.DueDate = DateTime.Parse(pago.PagosConCheques.FechaVencimiento); // Fecha de vencimiento del cheque
                                    oDoc.CheckAccount = pago.PagosConCheques.CuentaCheque;
                                    //LQ 20210727 comentado..// if (DateTime.Compare(oDoc.BillOfExchange.BillOfExchangeDueDate, oDoc.DueDate) > 0) oDoc.DueDate = oDoc.BillOfExchange.BillOfExchangeDueDate;
                                    break;
                            case "LET":
                                    oDoc.BillOfExchange.BillOfExchangeNo = pago.PagosConLetras.NumeroLetra;
                                    oDoc.BillOfExchange.BPBankCountry = pago.PagosConLetras.CodigoPais;
                                    oDoc.BillOfExchange.BPBankCode = pago.PagosConLetras.CodigoBancoAsociado;
                                    oDoc.BillOfExchange.BPBankAct = pago.PagosConLetras.CuentaBancariaBancoAsociado;
                                    oDoc.BillOfExchange.ReferenceNo = pago.PagosConLetras.NumeroLetra + "";
                                    oDoc.BillOfExchange.Remarks = "CHQ POS # " + pago.PagosConLetras.NumeroLetra;
                                    oDoc.BillOfExchange.BillOfExchangeDueDate = DateTime.Parse(pago.PagosConLetras.FechaVencimientoLetra);
                                    oDoc.BillOfExchange.PaymentMethodCode = pago.PagosConLetras.MetodoPago;
                                    oDoc.BillOfExchangeAmount = aplicardoc;
                                    oDoc.BillofExchangeStatus = SAPbobsCOM.BoBoeStatus.boes_Created;
                                    oDoc.BoeAccount = pago.PagosConLetras.CuentaLetra;
                                    
                                    break;
                        }

                        pista = "Pista10";
                        logs.grabarLog("COBRANZA_XML", oDoc.GetAsXML());

                        // Si existe sobrande al final añadirlo a favor de la cuenta, permite sin cuenta? 
                        //oDoc.AccountPayments.SumPaid = 200;
                        //oDoc.AccountPayments.Add();

                        pista = "Pista11";

                        // Añadir a SAP
                        error = oDoc.Add(); 

                        if (error != 0)
                        {
                            pista += " -> Pista11a";
                            company.GetLastError(out error, out mensaje);
                            logs.grabarLog("COBRANZA_ERROR_SAP", "ReciboWeb:" + cabeceraPagos.NumeroMBW + "- PK:" + id + " ** " + error + ": " + mensaje + pista);
                            logs.grabarLog("COBRANZA_LOG",       "ReciboWeb:" + cabeceraPagos.NumeroMBW + "- PK:" + id + " ** " + error + ": " + mensaje + pista);
                            pista += " -> " + "ERRORSAP:" + mensaje;
                            //throw new Exception(mensaje + " :: " + cabeceraPagos.NumeroMBW + " :: " + sCmd + " :: " + oDoc.DueDate);

                            cadena = "";

                            if (company.InTransaction)
                                company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

                            /*LQ 2022-02-27  cierro el ciclo para que aborte el programa y no continue buscando siguiente forma de pago (caso cobranza 14732 mega) */
                            throw new Exception(id + " :: " + mensaje + " :: Pista: " + id + " :: " + pista);

                        }
                        else
                        {
                            pista = "Pista11b";
                            cont++;
                            string key = company.GetNewObjectKey();
                            oDoc.GetByKey(int.Parse(key));
                            string xDocEntry = oDoc.DocEntry.ToString();
                            string xDocNum = oDoc.DocNum.ToString();
                            logs.grabarLog("COBRANZA_OK", "ReciboWeb:" + cabeceraPagos.NumeroMBW + "- PK:" + id + " - DocEntry:" + xDocEntry + " - DocNum:" + xDocNum);

                            cadena += xDocNum + ",";
                        }
                    }
                  }  /* end otras formas de pago diferentes de retencion */
                    if (sTipoPAgo == "RETENCIONES/TAR_CRED") /* al salir del ciclo de pagos guarda la ultima retencion */
                    {
                        if(existe_pago == false)
                        {
                            /******* INICIO GRABA RETENCION o tar --> */
                            pista = "retPista20";
                            logs.grabarLog("COBRANZA_XML", oDoc.GetAsXML());
                            pista = "retPista21";
                            // Añadir a SAP **************************************************************
                            error = oDoc.Add();
                            if (error != 0)
                            {
                                pista += " -> retPista21a";
                                company.GetLastError(out error, out mensaje);
                                logs.grabarLog("COBRANZA_ERROR_SAP", "ReciboWeb:" + cabeceraPagos.NumeroMBW + " ** " + error + ": " + mensaje + pista);
                                pista += " -> " + "ERRORSAP:" + mensaje;
                                //throw new Exception(mensaje + " :: " + cabeceraPagos.NumeroMBW + " :: " + sCmd + " :: " + oDoc.DueDate);
                                cadena = "";
                                if (company.InTransaction)
                                    company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                            }
                            else
                            {
                                pista = "retPista21b";
                                cont++;
                                string key = company.GetNewObjectKey();
                                oDoc.GetByKey(int.Parse(key));
                                string xDocEntry = oDoc.DocEntry.ToString();
                                string xDocNum = oDoc.DocNum.ToString();
                                logs.grabarLog("COBRANZA_OK", "ReciboWeb:" + cabeceraPagos.NumeroMBW + " - DocEntry:" + xDocEntry + " - DocNum:" + xDocNum);
                                cadena += xDocNum + ",";
                            }
                            /******* FIN GRABA RETENCION o  TAR ------ **/
                        }

                    }
                }  /* end foreach Pagos */



                /* al final del bloque de pagos.... */



                

                pista = pista + " -> " + "Pista12";

                if (cadena != "")
                {
                    if (company.InTransaction)
                        company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);

                    cadena = cadena.Remove(cadena.Length - 1);

                    response.Add("Success");
                    response.Add(cadena);
                    response.Add("");
                }
                else
                {
                    response.Clear();
                    response.Add("Error");
                    response.Add(error + ": " + pista + " " + mensaje);
                    response.Add("COB001");
                    response.Add(mensaje);

                    throw new Exception("No se realizó ninguna cobranza ");
                }

            }
            catch (Exception e)
            {
                cadena = "";

                if (company.InTransaction)
                    company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

                response.Clear();
                response.Add("Error");
                response.Add(pista + " " + error.ToString());
                response.Add("COB001");
                response.Add(e.Message);

                logs.grabarLog("COBRANZA", "Pista:" + pista);
                logs.grabarLog("COBRANZA", "Message" + e.Message);
                logs.grabarLog("COBRANZA", "StackTrace" + e.StackTrace);
                logs.grabarLog("COBRANZA", "Error:" + e.ToString());
                /*
                logs.grabarLog("COBRANZA_DEBUG", e.StackTrace);
                logs.grabarLog("COBRANZA_STRING", e.ToString());
                */
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

            //GC.WaitForPendingFinalizers();
            //GC.Collect();

            return response;
        }


        public static List<SeccionDocumentos> StringToListSeccionDocumentos(string XML)
        {
            List<SeccionDocumentos> list = new List<SeccionDocumentos>();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(XML);

            XmlNodeList items = xml.GetElementsByTagName("ListSeccionDocumentos");
            XmlNodeList item = ((XmlElement)items[0]).GetElementsByTagName("SeccionDocumentos");

            foreach (XmlElement nodo in item)
            {
                SeccionDocumentos documento = new SeccionDocumentos();
                documento.MontoRetencion = XmlConvert.ToDouble(nodo.GetElementsByTagName("MontoRetencion")[0].InnerText);
                documento.NumeroCuota = XmlConvert.ToInt32(nodo.GetElementsByTagName("NumeroCuota")[0].InnerText);
                documento.NumeroCuotaSAP = XmlConvert.ToInt32(nodo.GetElementsByTagName("NumeroCuotaSAP")[0].InnerText);
                documento.NumeroDocumento = XmlConvert.ToInt32(nodo.GetElementsByTagName("NumeroDocumento")[0].InnerText);
                documento.NumeroRetencion = nodo.GetElementsByTagName("NumeroRetencion")[0].InnerText;
                documento.TipoDocumento = nodo.GetElementsByTagName("TipoDocumento")[0].InnerText;
                documento.Valor = XmlConvert.ToDouble(nodo.GetElementsByTagName("Valor")[0].InnerText);
                documento.SePuedeUsar = true;
                documento.SaldoValor = documento.Valor; // parte con el valor total que el vendedor le aplico

                list.Add(documento);
            }

            return list;
        }


        public static List<Pago> StringToListPago(string XML)
        {
            List<Pago> list = new List<Pago>();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(XML);

            XmlNodeList items = xml.GetElementsByTagName("ListPago");
            XmlNodeList item = ((XmlElement)items[0]).GetElementsByTagName("Pago");

            foreach (XmlElement nodo in item)
            {
                Pago pago = new Pago();
                pago.TipoPago = nodo.GetElementsByTagName("TipoPago")[0].InnerText;
                pago.PagosConTarjeta = null;
                pago.PagosConLetras = null;
                pago.PagosConCheques = null;

                switch (pago.TipoPago)
                {
                    case "DEP":
                        pago.PagosConCheques = new SeccionPagosCheques();
                        pago.PagosConCheques.NumeroCuentaBancaria = nodo.GetElementsByTagName("NumeroCuentaBancaria")[0].InnerText;
                        pago.PagosConCheques.SumaValorCheques = XmlConvert.ToDouble(nodo.GetElementsByTagName("SumaValorCheques")[0].InnerText);
                        pago.PagosConCheques.FechaVencimiento = nodo.GetElementsByTagName("FechaVencimiento")[0].InnerText;
                        pago.PagosConCheques.NumeroAutorizacion = nodo.GetElementsByTagName("NumeroAutorizacion")[0].InnerText;

                        pago.PagosConCheques.IdFP = nodo.GetElementsByTagName("IdFP")[0].InnerText;
                        break;
                    case "CHE":
                        DateTime date1 = DateTime.Parse(nodo.GetElementsByTagName("FechaVencimiento")[0].InnerText);
                        DateTime date2 = DateTime.Now;

                        int result = DateTime.Compare(date1, date2);
                        //if (result <= 0)
                        if (1 == 1)
                        {
                            pago.PagosConCheques = new SeccionPagosCheques();
                            pago.PagosConCheques.CodigoBanco = nodo.GetElementsByTagName("CodigoBanco")[0].InnerText;
                            pago.PagosConCheques.NumeroCuentaBancaria = nodo.GetElementsByTagName("NumeroCuentaBancaria")[0].InnerText;
                            pago.PagosConCheques.FechaVencimiento = nodo.GetElementsByTagName("FechaVencimiento")[0].InnerText;
                            pago.PagosConCheques.CodigoPais = nodo.GetElementsByTagName("CodigoPais")[0].InnerText;
                            pago.PagosConCheques.SumaValorCheques = XmlConvert.ToDouble(nodo.GetElementsByTagName("SumaValorCheques")[0].InnerText);
                            pago.PagosConCheques.CuentaCheque = nodo.GetElementsByTagName("CuentaCheque")[0].InnerText;

                            pago.PagosConLetras = new SeccionPagosLetrasCambio();
                            pago.PagosConLetras.MetodoPago = nodo.GetElementsByTagName("MetodoPago")[0].InnerText;

                            pago.PagosConCheques.NumeroAutorizacion = nodo.GetElementsByTagName("NumeroAutorizacion")[0].InnerText;
                            pago.PagosConCheques.NumeroCheques = XmlConvert.ToInt32(nodo.GetElementsByTagName("NumeroCheques")[0].InnerText);
                            pago.PagosConCheques.RetencionesAsociadas = null;

                            pago.PagosConCheques.IdFP = nodo.GetElementsByTagName("IdFP")[0].InnerText;
                        }
                        else
                        {
                            //NO APLICA CODIGO ANTERIOR
                        }

                        break;
                    case "CHEPOST":

                        pago.TipoPago = "LET";
                        pago.PagosConLetras = new SeccionPagosLetrasCambio();
                        pago.PagosConLetras.CodigoBancoAsociado = nodo.GetElementsByTagName("CodigoBanco")[0].InnerText;
                        pago.PagosConLetras.CuentaBancariaBancoAsociado = nodo.GetElementsByTagName("NumeroCuentaBancaria")[0].InnerText;
                        pago.PagosConLetras.FechaVencimientoLetra = nodo.GetElementsByTagName("FechaVencimiento")[0].InnerText;
                        pago.PagosConLetras.CodigoPais = nodo.GetElementsByTagName("CodigoPais")[0].InnerText;
                        pago.PagosConLetras.SumaValorLetra = XmlConvert.ToDouble(nodo.GetElementsByTagName("SumaValorCheques")[0].InnerText);
                        pago.PagosConLetras.CuentaLetra = nodo.GetElementsByTagName("CuentaCheque")[0].InnerText;
                        pago.PagosConLetras.MetodoPago = nodo.GetElementsByTagName("MetodoPago")[0].InnerText;

                        pago.PagosConLetras.NumeroLetra = nodo.GetElementsByTagName("NumeroCheques")[0].InnerText;
                        pago.PagosConLetras.IdFP = nodo.GetElementsByTagName("IdFP")[0].InnerText;


                        break;

                    case "RET":
                        pago.PagosConTarjeta = new SeccionPagosTarjeta();
                        pago.PagosConTarjeta.TarjetaCredito = XmlConvert.ToInt32(nodo.GetElementsByTagName("CodRetencion")[0].InnerText);
                        pago.PagosConTarjeta.CuentaTarjetaCredito = nodo.GetElementsByTagName("CuentaContable")[0].InnerText;
                        pago.PagosConTarjeta.NumeroTarjeta = nodo.GetElementsByTagName("NumeroTarjeta")[0].InnerText;
                        pago.PagosConTarjeta.NumeroReclamoFactura = nodo.GetElementsByTagName("FacturaRelacionada")[0].InnerText;
                        pago.PagosConTarjeta.NumeroAutorizacion = nodo.GetElementsByTagName("NumeroAutorizacion")[0].InnerText;
                        pago.PagosConTarjeta.SumaCredito = XmlConvert.ToDouble(nodo.GetElementsByTagName("ImporteVencido")[0].InnerText);
                        pago.PagosConTarjeta.FechaVencAutorizacion = nodo.GetElementsByTagName("FechaVencAut")[0].InnerText;
                        pago.PagosConTarjeta.TipoRetencion = nodo.GetElementsByTagName("TipoRetencion")[0].InnerText;
                        pago.PagosConTarjeta.SetieRetencion = nodo.GetElementsByTagName("SerieRetencion")[0].InnerText;
                        pago.PagosConTarjeta.CantidadPagos = XmlConvert.ToInt32(nodo.GetElementsByTagName("CatnidadPagos")[0].InnerText);
                        pago.PagosConTarjeta.BaseImponible = XmlConvert.ToDouble(nodo.GetElementsByTagName("BaseImponible")[0].InnerText);
                        pago.PagosConTarjeta.IdFP = nodo.GetElementsByTagName("IdFP")[0].InnerText;

                        break;

                    case "TAR":
                        pago.PagosConTarjeta = new SeccionPagosTarjeta();
                        pago.PagosConTarjeta.TarjetaCredito = XmlConvert.ToInt32(nodo.GetElementsByTagName("CodRetencion")[0].InnerText);
                        pago.PagosConTarjeta.CuentaTarjetaCredito = nodo.GetElementsByTagName("CuentaContable")[0].InnerText;
                        pago.PagosConTarjeta.NumeroTarjeta = nodo.GetElementsByTagName("NumeroTarjeta")[0].InnerText;
                        pago.PagosConTarjeta.NumeroReclamoFactura = nodo.GetElementsByTagName("FacturaRelacionada")[0].InnerText;
                        pago.PagosConTarjeta.NumeroAutorizacion = nodo.GetElementsByTagName("NumeroAutorizacion")[0].InnerText;
                        pago.PagosConTarjeta.SumaCredito = XmlConvert.ToDouble(nodo.GetElementsByTagName("ImporteVencido")[0].InnerText);
                        pago.PagosConTarjeta.FechaVencAutorizacion = nodo.GetElementsByTagName("FechaVencAut")[0].InnerText;
                        pago.PagosConTarjeta.TipoRetencion = nodo.GetElementsByTagName("TipoRetencion")[0].InnerText;
                        pago.PagosConTarjeta.SetieRetencion = nodo.GetElementsByTagName("SerieRetencion")[0].InnerText;
                        pago.PagosConTarjeta.CantidadPagos = XmlConvert.ToInt32(nodo.GetElementsByTagName("CatnidadPagos")[0].InnerText);
                        pago.PagosConTarjeta.BaseImponible = XmlConvert.ToDouble(nodo.GetElementsByTagName("BaseImponible")[0].InnerText);
                        pago.PagosConTarjeta.IdFP = nodo.GetElementsByTagName("IdFP")[0].InnerText;
                        pago.PagosConTarjeta.Lote = nodo.GetElementsByTagName("NumLote")[0].InnerText;
                        pago.PagosConTarjeta.Tipo_tarjeta = nodo.GetElementsByTagName("TipoTarjeta")[0].InnerText;
                        break;
                }

                /*
                    pago.MontoRetencion = XmlConvert.ToDouble(nodo.GetElementsByTagName("MontoRetencion")[0].InnerText);
                    pago.NumeroCuota = XmlConvert.ToInt32(nodo.GetElementsByTagName("NumeroCuota")[0].InnerText);
                    pago.NumeroCuotaSAP = XmlConvert.ToInt32(nodo.GetElementsByTagName("NumeroCuotaSAP")[0].InnerText);
                    pago.NumeroDocumento = XmlConvert.ToInt32(nodo.GetElementsByTagName("NumeroDocumento")[0].InnerText);
                    pago.NumeroRetencion = nodo.GetElementsByTagName("NumeroRetencion")[0].InnerText;
                    pago.TipoDocumento = nodo.GetElementsByTagName("TipoDocumento")[0].InnerText;
                    pago.Valor = XmlConvert.ToDouble(nodo.GetElementsByTagName("Valor")[0].InnerText);*/

                list.Add(pago);
            }

            return list;
        }
        private static void TestCobranza()
        {
            try
            {
                SAPbobsCOM.Company company = null;

                logs.grabarLog("COBRANZA_LOG", "conexion:");
                company = DataBase.conectar();
                // Crear el objeto de pago
                // Crear el objeto de pago
                SAPbobsCOM.Payments oPayment = (SAPbobsCOM.Payments)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments);

                // Configurar los detalles del pago
                oPayment.DocType = SAPbobsCOM.BoRcptTypes.rCustomer;
                oPayment.CardCode = "C00011"; // Código del cliente (modificar según sea necesario)
                oPayment.DocDate = DateTime.Now;
                oPayment.BPLID = 2;
                // Configurar detalles de la retención
                //oPayment.WithholdingTaxDataWTX.Add();
                //oPayment.WithholdingTaxDataWTX.SetCurrentLine(0);
                //oPayment.WithholdingTaxDataWTX.WTCode = "312"; // Código de la retención
                //oPayment.WithholdingTaxDataWTX.WTAmount = 33.64; // Monto de la retención
                //oPayment.WithholdingTaxDataWTX.WTAccount = "1010501003"; // Cuenta contable de la retención

                // Configurar detalles de la tarjeta de crédito
                oPayment.CreditCards.CreditCard = 8; // Código de la tarjeta de crédito (asegúrate de que este código exista)
                oPayment.CreditCards.CreditAcct = "1010101002"; // Cuenta contable asociada a la tarjeta de crédito
                oPayment.CreditCards.CreditSum = 19.22; // Monto del pago con tarjeta de crédito
                oPayment.CreditCards.CreditCardNumber = "9999"; // Número de la tarjeta de crédito
                oPayment.CreditCards.CardValidUntil = DateTime.Parse("2024-05-21"); // Fecha de vencimiento de la tarjeta
                oPayment.CreditCards.VoucherNum = "53"; // Número de autorización

                // Asociar el pago a una factura específica (opcional)
                SAPbobsCOM.Documents invoice = (SAPbobsCOM.Documents)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                if (invoice.GetByKey(54)) // DocEntry de la factura (modificar según sea necesario)
                {
                    oPayment.Invoices.DocEntry = invoice.DocEntry;
                    oPayment.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_Invoice;
                    oPayment.Invoices.SumApplied = 33.64;
                }
                else
                {
                    Console.WriteLine("Factura no encontrada.");
                    company.Disconnect();
                    return;
                }

                // Agregar el pago
                int addResult = oPayment.Add();
                if (addResult != 0)
                {
                    Console.WriteLine($"Error al guardar el pago: {company.GetLastErrorDescription()}");
                }
                else
                {
                    Console.WriteLine("Pago guardado correctamente.");
                }

                // Desconectar de SAP Business One
                company.Disconnect();
            }
            catch (Exception ex)
            {
            }
        }
    }
}