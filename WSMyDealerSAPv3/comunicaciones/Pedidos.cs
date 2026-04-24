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
    public static class Pedidos
    {
        public static RespuestaExistenciaPedido existePedido(int numPedido)
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
                return response;
            }

            // CONSULTAR A LA VISTA POR MEDIO DEL NUMERO DE PEDIDO DE MYDEALER.
            try
            {
                String sql = " select \"numpedidoerp\", \"srorden\", \"estado\"  from \"MD_PEDIDOS_ESTADO\" where \"srorden\"=" + numPedido;

                OdbcCommand com = new OdbcCommand(sql, DBSqlServer.Conexion);
                com.CommandType = CommandType.Text;

                OdbcDataReader record = com.ExecuteReader();
                if (record.HasRows)
                {
                    if (record.Read())
                    {
                        response.ExistePedido = true;
                        response.NumeroPedidoSAP = record.GetValue(0).ToString().Trim();
                        response.NumeroPedidoMyDealer = record.GetValue(1).ToString().Trim();
                        //response.Tipo = record.GetValue(3).ToString();
                        response.Tipo = "";
                        response.Estado = record.GetValue(2).ToString().Trim();
                    }
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

                logs.grabarLog("PEDIDOS", e.Message);
                logs.grabarLog("PEDIDOS_DEBUG", e.StackTrace);
            }
            finally
            {
                DBSqlServer.DesconectaDB();
            }

            //GC.WaitForPendingFinalizers();
            //GC.Collect();

            return response;
        }

        

        public static string obtenerPrimerValor(string query)
        {
            DBSqlServer.ConectaDB();
            if (!DBSqlServer.Respuesta.Exito)
            {
                return null;
            }

            string retorno = null;

            try
            {
                String sql = query;
                logs.grabarLog("PEDIDOS_sql", sql);
                OdbcCommand com = new OdbcCommand(sql, DBSqlServer.Conexion);
                com.CommandType = CommandType.Text;

                OdbcDataReader record = com.ExecuteReader();
                if (record.HasRows)
                {
                    if (record.Read())
                    {
                        retorno = record.GetValue(0).ToString().Trim();
                    }
                }

                record.Close(); record.Dispose(); record = null;
                com.Dispose(); com = null;

            }
            catch (Exception e)
            {
                retorno = null;
                logs.grabarLog("PEDIDOS", e.Message);
                logs.grabarLog("PEDIDOS_DEBUG", e.StackTrace);
            }
            finally
            {
                DBSqlServer.DesconectaDB();
            }

            return retorno;
        }
        

        

        /**
         * Ingresar una orden en SAP BO
         * @param cabecera La cabecera de la orden
         * @param detalles Los detalles de la orden
         * @return El XML con la representacion de la respuesta
         */
        public static List<String> ingresarOrden(CabeceraOrden cabecera, List<DetalleOrden> detalles)
        {
            //CreateSalesOrder();
            List<String> response = new List<String>();
            int error = 0; string mensaje = "", prefix_error = "";
            string propietario = "";
            string lqdebug = "", str="";
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
                oDoc = (SAPbobsCOM.Documents)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);

                prefix_error = " orden # " + cabecera.NumeroOrdenWeb + " :: ";

                oDoc.Series = cabecera.Series;

                oDoc.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items;
                oDoc.DocDate = DateTime.Parse(cabecera.FechaGeneracionPedido);
                oDoc.DocDueDate = DateTime.Parse(cabecera.FechaEntregaPedido);
                oDoc.CardCode = cabecera.CodigoCliente;
                oDoc.Comments = cabecera.Observaciones;
                oDoc.PaymentGroupCode = cabecera.CodigoFormaPago;
                oDoc.SalesPersonCode = cabecera.CodigoVendedor;
                oDoc.ShipToCode = cabecera.CodigoDireccionEnvio;
                oDoc.JournalMemo = "Origen: MD - Recibo: " + cabecera.NumeroOrdenWeb;
                //lqdebug += "->U_MD_ORDEN";
                oDoc.UserFields.Fields.Item("U_MD_ORDEN").Value = cabecera.NumeroOrdenWeb.ToString();
                //lqdebug += "->U_AprobEspecial";
                //oDoc.UserFields.Fields.Item("U_AprobEspecial").Value = "S";  //LQ 2021-06-24 agregar para que no de error de cambio de descuento respecto al descuento configurado x defecto
                //lqdebug += "->U_MEGA_PROV_CIUD:"+cabecera.CiudProv;
                //str = cabecera.CiudProv;
                //if (str != "C" && str != "P" && str != "E") str = "E";  // LQ 2021-09-12 exento por defecto
                //oDoc.UserFields.Fields.Item("U_MEGA_PROV_CIUD").Value = str;
                //lqdebug += "->U_RutaSN:"+ cabecera.CodRuta;
                /* LQ 23/6/22 si es ET3 carga a codigo transporte es entrega a tercero */
                //if (cabecera.CodRuta == "ET3") {
                //    oDoc.UserFields.Fields.Item("U_TRANSPORTE").Value = cabecera.CodRuta;
                //}
                //else
                //{
                //    oDoc.UserFields.Fields.Item("U_RutaSN").Value = cabecera.CodRuta;
                //}
                /**/
                //lqdebug += "->U_MEGA_ZONASN:" + cabecera.Zona;
                //oDoc.UserFields.Fields.Item("U_MEGA_ZONASN").Value = cabecera.Zona;
                //lqdebug += "->U_MEGA_SUBZONASN:" + cabecera.Subzona; 
                //oDoc.UserFields.Fields.Item("U_MEGA_SUBZONASN").Value = cabecera.Subzona;
                //propietario = obtenerPrimerValor("select \"codpropietario\" from MD_VENDEDORES where \"codvendedor\"='" + cabecera.CodigoVendedor + "'");
                //lqdebug += "->propietario:" + propietario;
                //if (propietario != null) oDoc.DocumentsOwner = int.Parse(propietario);   //test owner en pedidos

                /*2023-12-14 agregar campo de origen de pedido U_MD_APP_TIPO */
                lqdebug += "->origen:" + cabecera.Origen;
                //oDoc.UserFields.Fields.Item("U_MD_APP_TIPO").Value = cabecera.Origen;

                oDoc.DiscountPercent = 0;

                oDoc.BPL_IDAssignedToInvoice = 2; // Default sucursal 2 de imperial
                oDoc.UserFields.Fields.Item("U_PI_ESTADOS").Value = "C";//default C de Creado
                //oDoc.UserFields.Fields.Item("U_PI_ESTFEHO").Value = DateTime.Now.ToShortDateString();
                //oDoc.UserFields.Fields.Item("U_PI_HORA").Value = "15:41";
                //oDoc.ContactPersonCode = 2;
                // DETALLE LINEAS
                int linea = 0;
                foreach (DetalleOrden detalle in detalles)
                {
                    if (linea > 0) oDoc.Lines.Add();
                    oDoc.Lines.SetCurrentLine(linea);
                    oDoc.Lines.WarehouseCode = detalle.Wsc;
                    oDoc.Lines.ItemCode = detalle.CodigoProducto;
                    oDoc.Lines.Quantity = detalle.CantidadProducto;
                    //oDoc.Lines.Price = detalle.TotalLinea / detalle.CantidadProducto;
                    oDoc.Lines.DiscountPercent = detalle.PorcentajeDescuento;
                    oDoc.Lines.UnitPrice = detalle.PrecioProducto;
                    oDoc.Lines.SalesPersonCode = cabecera.CodigoVendedor;

                    //oDoc.Lines.COGSCostingCode = "OCF";
                    
                    // 22/06/21 jdiaz
                    //oDoc.Lines.COGSCostingCode = obtenerPrimerValor("select \"codsucursal\" from MD_BODEGAS where \"codbodega\"='" + detalle.Wsc + "'");
                    //oDoc.Lines.COGSCostingCode2 = obtenerPrimerValor("select \"CCLinea\" from md_productos where \"codproducto\"='" + detalle.CodigoProducto + "'");
                    //oDoc.Lines.COGSCostingCode4 = obtenerPrimerValor("select \"CCMarca\" from md_productos where \"codproducto\"='" + detalle.CodigoProducto + "'");

                    logs.grabarLog("PEDIDOS_ocr", detalle.Wsc + " - " + oDoc.Lines.COGSCostingCode);
                    //logs.grabarLog("PEDIDOS_ocr", detalle.CodigoProducto + " - " + oDoc.Lines.COGSCostingCode2);
                    //logs.grabarLog("PEDIDOS_ocr", detalle.CodigoProducto + " - " + oDoc.Lines.COGSCostingCode4);

                    linea++;
                }
                lqdebug += "->ADD ";
                error = oDoc.Add();

                if (error != 0)
                {
                    mensaje += " ::: " + lqdebug;
                    company.GetLastError(out error, out mensaje);
                    logs.grabarLog("PEDIDOS", "OrdenWeb:" + cabecera.NumeroOrdenWeb.ToString() + " ** " + error + ": " + mensaje);
                    throw new Exception(" ** " + error + ": " + mensaje + " :: " + cabecera.CodigoDireccionEnvio);
                }
                else
                {
                    string key = company.GetNewObjectKey();
                    oDoc.GetByKey(int.Parse(key));
                    string xDocNum = oDoc.DocNum.ToString();

                    logs.grabarLog("PEDIDOS", "OrdenWeb:" + cabecera.NumeroOrdenWeb.ToString() + " OrdenSap:" + xDocNum);
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
                response.Add("PED001");
                response.Add(e.Message);

                logs.grabarLog("PEDIDOS", prefix_error + e.Message + company.GetLastErrorDescription() + " ::: " + lqdebug);
                logs.grabarLog("PEDIDOS_DEBUG", e.ToString());

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

        public static void CreateSalesOrder()
        {
            SAPbobsCOM.Company company = null;

            company = DataBase.conectar();
            SAPbobsCOM.Documents salesOrder = null;
            salesOrder = (SAPbobsCOM.Documents)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);

            //Documents salesOrder = company.GetBusinessObject(BoObjectTypes.oOrders);

            // Establecer campos de la orden de venta
            salesOrder.Series = 81;
            salesOrder.CardCode = "C00010"; // Código del cliente
            salesOrder.DocDueDate = DateTime.Now.Date; // Fecha de vencimiento del documento

            // Agregar líneas de detalle (productos)
            salesOrder.Lines.ItemCode = "IND0001"; // Código del producto
            salesOrder.Lines.Quantity = 1; // Cantidad del producto
            salesOrder.Lines.UnitPrice = 100.00; // Precio unitario del producto
            salesOrder.Lines.Add();

            // Agregar más líneas de detalle si es necesario
            salesOrder.BPL_IDAssignedToInvoice = 2;
            salesOrder.UserFields.Fields.Item("U_PI_ESTADOS").Value = "C";
            // Adicionar la orden de venta
            int result = salesOrder.Add();

            // Verificar si la orden de venta se creó correctamente
            if (result == 0)
            {
                Console.WriteLine("Orden de venta creada exitosamente.");
            }
            else
            {
                string errorMsg = company.GetLastErrorDescription();
                Console.WriteLine("Error al crear la orden de venta: " + errorMsg);
            }
        }


        public static List<DetalleOrden> StringToList(string XML)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(XML);

            XmlNodeList items = xml.GetElementsByTagName("items");
            XmlNodeList item = ((XmlElement)items[0]).GetElementsByTagName("item");
            List<DetalleOrden> list = new List<DetalleOrden>();

            foreach (XmlElement nodo in item)
            {
                DetalleOrden detalle = new DetalleOrden();
                detalle.CantidadProducto = XmlConvert.ToDouble(nodo.GetElementsByTagName("CantidadProducto")[0].InnerText);
                detalle.CodigoDocumentoRelacionado = nodo.GetElementsByTagName("CodigoDocumentoRelacionado")[0].InnerText;
                detalle.CodigoProducto = nodo.GetElementsByTagName("CodigoProducto")[0].InnerText;
                detalle.NumeroLineaDetalle = XmlConvert.ToInt32(nodo.GetElementsByTagName("NumeroLineaDetalle")[0].InnerText);
                detalle.PorcentajeDescuento = XmlConvert.ToDouble(nodo.GetElementsByTagName("PorcentajeDescuento")[0].InnerText);
                detalle.PrecioProducto = XmlConvert.ToDouble(nodo.GetElementsByTagName("PrecioProducto")[0].InnerText);
                detalle.TotalLinea = XmlConvert.ToDouble(nodo.GetElementsByTagName("TotalLinea")[0].InnerText);
                detalle.Wsc = nodo.GetElementsByTagName("Wsc")[0].InnerText;
                detalle.Detalle_descuento = nodo.GetElementsByTagName("Detalle_descuento")[0].InnerText;
                detalle.Detalle_promocion = nodo.GetElementsByTagName("Detalle_promocion")[0].InnerText;
                detalle.Promocion_estado = nodo.GetElementsByTagName("Promocion_estado")[0].InnerText;
                detalle.Piezas = XmlConvert.ToInt32(nodo.GetElementsByTagName("Piezas")[0].InnerText);
                detalle.Peso = XmlConvert.ToDouble(nodo.GetElementsByTagName("Peso")[0].InnerText);
                detalle.Peso_total = XmlConvert.ToDouble(nodo.GetElementsByTagName("Peso_total")[0].InnerText);
                try { detalle.PorcIva = XmlConvert.ToDouble(nodo.GetElementsByTagName("Peso_total")[0].InnerText); }
                catch (Exception) { detalle.PorcIva = 0; }

                detalle.UndEntry = XmlConvert.ToInt32(nodo.GetElementsByTagName("UnidadEntry")[0].InnerText);
                detalle.UndCode = nodo.GetElementsByTagName("UnidadCode")[0].InnerText;
                detalle.UndidadCaja = XmlConvert.ToDouble(nodo.GetElementsByTagName("UnidadCaja")[0].InnerText);

                detalle.Galones = XmlConvert.ToDouble(nodo.GetElementsByTagName("Galones")[0].InnerText);


                list.Add(detalle);
            }


            return list;
        }

        public static Respuesta obtenerCabeceraOrden(int numorden, int numordenweb)
        {
            Respuesta response = new Respuesta();

            RespuestaExistenciaPedido ped = existePedido(numordenweb);
            SAPbobsCOM.Company company = null;

            company = DataBase.conectar();

            logs.grabarLog("PEDIDOS", "DataBase.Respuesta.Exito: " + DataBase.Respuesta.Exito + "");

            if (!DataBase.Respuesta.Exito)
            {
                response.Exito = false;
                response.CodigoError = DataBase.Respuesta.CodigoError;
                response.CodigoRespuesta = DataBase.Respuesta.CodigoRespuesta;
                response.DescripcionError = DataBase.Respuesta.DescripcionError;
                return response;
            }

            SAPbobsCOM.Documents oDoc = (SAPbobsCOM.Documents)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);

            bool retornar = false;
            try
            {
                if (!ped.ExistePedido)
                {
                    throw new Exception("Pedido no encontrado");
                }

                if (ped.Tipo.Equals("O"))
                {
                    oDoc = (SAPbobsCOM.Documents)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);

                    if (oDoc.GetByKey(int.Parse(ped.NumeroPedidoSAP)) != true)
                        throw new Exception("Error al leer el pedido (oOrders)");
                }
                else
                {
                    oDoc = (SAPbobsCOM.Documents)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts); //oDrafts
                    oDoc.DocObjectCode = SAPbobsCOM.BoObjectTypes.oOrders;

                    if (oDoc.GetByKey(int.Parse(ped.NumeroPedidoSAP)) != true)
                        throw new Exception("Error al leer el pedido (oDrafts)");
                }
                retornar = false;
            }
            catch (Exception e)
            {
                response.Exito = false;
                response.CodigoError = "CAB002";
                response.CodigoRespuesta = "CAB002";
                response.DescripcionError = "PEDIDO NO ENCONTRADO.";
                // response;
                retornar = true;
                logs.grabarLog("CABECERA_ORDEN", e.Message);
                logs.grabarLog("CABECERA_ORDEN_DEBUG", e.StackTrace);
            }

            if (retornar)
            {
                if (oDoc != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(oDoc);
                oDoc = null;
                DataBase.DesconectaDB(company);

                //GC.WaitForPendingFinalizers();
                //GC.Collect();

                return response;
            }

            try
            {
                CabeceraOrden cab = new CabeceraOrden();
                cab.Aprobadoventa = "";
                cab.CodigoCliente = oDoc.CardCode;
                cab.CodigoCobrador = oDoc.AgentCode;
                cab.CodigoDireccionEnvio = oDoc.ShipToCode;
                cab.CodigoFormaPago = oDoc.PaymentGroupCode;
                cab.CodigoMotivoReclamo = "";
                cab.CodigoServicioCliente = "";
                cab.CodigoTransportista = "";
                cab.CodigoVendedor = oDoc.SalesPersonCode;
                cab.Codmotivonoaprobacion = "";
                cab.Descprioridad = "";
                cab.Detallemotivoaprobacion = "";
                cab.FechaEntregaPedido = oDoc.DocDueDate.ToString();
                cab.FechaGeneracionPedido = oDoc.DocDate.ToString();
                cab.NumeroOrdenPedidoFisico = "";
                cab.NumeroOrden = oDoc.DocEntry;
                //r      cab.NumeroOrdenWeb = (int) oDoc.UserFields.Fields.Item("U_ita_ped_num").Value;
                cab.NumeroOrdenWeb = 1;

                cab.NumeroReclamo = "";
                cab.Observaciones = oDoc.Comments;
                cab.Pedidoaprobado = (oDoc.Confirmed.Equals(SAPbobsCOM.BoYesNoEnum.tYES) ? "Y" : "N");
                //r      cab.Pedir_autorizacion = oDoc.UserFields.Fields.Item("U_ita_ped_aut").Value.ToString();
                //r      cab.Ruta_logistica = oDoc.UserFields.Fields.Item("U_ita_rut_log").Value.ToString();
                //r     cab.Ruta_secuencia = (int) oDoc.UserFields.Fields.Item("U_ita_rut_sec").Value;
                cab.Pedir_autorizacion = "s";
                cab.Ruta_logistica = "s";
                cab.Ruta_secuencia = 1;

                cab.Subotal = 0;
                //r     cab.Tipo_pedido = oDoc.UserFields.Fields.Item("U_ita_ped_tipo").Value.ToString();
                cab.Tipo_pedido = "t";

                cab.TotalGasto = oDoc.VatSum;
                cab.TotalPedido = oDoc.DocTotal;
                cab.Valor_descto = 0;

                response.Exito = true;
                response.CodigoError = "";
                response.DescripcionError = "";
                response.CodigoRespuesta = "OK";
                // response.EntradaRAW = cab;

                XmlSerializer xmlSerializer = new XmlSerializer(cab.GetType());
                StringWriter textWriter = new StringWriter();

                xmlSerializer.Serialize(textWriter, cab);
                response.EntradaRAW = textWriter.ToString();

                cab = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                response.Exito = false;
                response.DescripcionError = e.Message;
                response.CodigoError = "CAB001";
                response.CodigoRespuesta = "CAB001";
                response.EntradaRAW = "";
                logs.grabarLog("CABECERA_ORDEN", e.Message);
                logs.grabarLog("CABECERA_ORDEN_DEBUG", e.StackTrace);
            }
            finally
            {
                DataBase.DesconectaDB(company);
            }

            if (oDoc != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(oDoc);
            oDoc = null;

            //GC.WaitForPendingFinalizers();
            //GC.Collect();

            return response;
        }

        public static Respuesta obtenerDetalleOrden(int numorden, int numordenweb)
        {
            Respuesta response = new Respuesta();

            RespuestaExistenciaPedido ped = existePedido(numordenweb);
            SAPbobsCOM.Company company = null;


            company = DataBase.conectar();
            if (!DataBase.Respuesta.Exito)
            {
                response.Exito = false;
                response.CodigoError = DataBase.Respuesta.CodigoError;
                response.CodigoRespuesta = DataBase.Respuesta.CodigoRespuesta;
                response.DescripcionError = DataBase.Respuesta.DescripcionError;
                return response;
            }

            SAPbobsCOM.Documents oDoc = (SAPbobsCOM.Documents)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);


            bool retornar = false;
            try
            {
                if (!ped.ExistePedido)
                {
                    throw new Exception("Pedido no encontrado");
                }

                if (ped.Tipo.Equals("O"))
                {
                    oDoc = (SAPbobsCOM.Documents)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);

                    if (oDoc.GetByKey(int.Parse(ped.NumeroPedidoSAP)) != true)
                        throw new Exception("Error al leer el pedido (oOrders)");
                }
                else
                {
                    oDoc = (SAPbobsCOM.Documents)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts); //oDrafts
                    oDoc.DocObjectCode = SAPbobsCOM.BoObjectTypes.oOrders;

                    if (oDoc.GetByKey(int.Parse(ped.NumeroPedidoSAP)) != true)
                        throw new Exception("Error al leer el pedido (oDrafts)");
                }
                retornar = false;
            }
            catch (Exception e)
            {
                response.Exito = false;
                response.CodigoError = "CAB002";
                response.CodigoRespuesta = "CAB002";
                response.DescripcionError = "PEDIDO NO ENCONTRADO. - " + e.Message;
                retornar = true;

                logs.grabarLog("DETALLE_ORDEN", e.Message);
                logs.grabarLog("DETALLE_ORDEN_DEBUG", e.StackTrace);
            }

            if (retornar)
            {
                if (oDoc != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(oDoc);
                oDoc = null;
                DataBase.DesconectaDB(company);

                //GC.WaitForPendingFinalizers();
                //GC.Collect();

                return response;
            }

            try
            {
                List<DetalleOrden> det = new List<DetalleOrden>();

                int numItems = oDoc.Lines.Count;


                DetalleOrden detalle;

                for (int i = 0; i < numItems; i++)
                {
                    detalle = new DetalleOrden();
                    oDoc.Lines.SetCurrentLine(i);

                    detalle.CodigoProducto = oDoc.Lines.ItemCode;
                    detalle.CantidadProducto = oDoc.Lines.Quantity;
                    detalle.CodigoDocumentoRelacionado = "";
                    detalle.Codunidadmedida = "";
                    //r detalle.Detalle_descuento = oDoc.Lines.UserFields.Fields.Item("U_ita_des_num").Value.ToString();
                    //r detalle.Detalle_promocion = oDoc.Lines.UserFields.Fields.Item("U_ita_pro_num").Value.ToString();
                    detalle.Detalle_descuento = "d";
                    detalle.Detalle_promocion = "p";

                    detalle.NumeroLineaDetalle = i;
                    //r detalle.Peso = (double)oDoc.Lines.UserFields.Fields.Item("U_ita_pes_std").Value;
                    detalle.Peso = 1;

                    detalle.Peso_total = (double)oDoc.Lines.Weight1;
                    //r detalle.Piezas = (int) oDoc.Lines.UserFields.Fields.Item("U_ita_ped_pza").Value;
                    detalle.Piezas = 1;

                    detalle.PorcentajeDescuento = oDoc.Lines.DiscountPercent;
                    if (oDoc.Lines.DeferredTax == SAPbobsCOM.BoYesNoEnum.tYES)
                        detalle.PorcIva = 12;
                    else
                        detalle.PorcIva = 0;
                    detalle.PrecioProducto = oDoc.Lines.UnitPrice;
                    //r     detalle.Promocion_estado = oDoc.Lines.UserFields.Fields.Item("U_ita_ped_est").Value.ToString();
                    detalle.Promocion_estado = "e";

                    detalle.Subtotal = detalle.PrecioProducto * detalle.CantidadProducto;
                    detalle.TotalLinea = oDoc.Lines.LineTotal;
                    detalle.Valor_dscto = Math.Round(detalle.Subtotal * detalle.PorcentajeDescuento / 100, 2);
                    detalle.Wsc = oDoc.Lines.WarehouseCode;

                    det.Add(detalle);
                }
                /*
                 vatsum es el valor del iva por cada linea
    [15:00:30] Silvia Vivar: linetotal es el total de cada línea ya con descuento e iva
    [15:00:51] Silvia Vivar: podria calcularlo de la siguiente manera
    [15:06:13] Silvia Vivar: el campo pricebefdi es el precio del item antes del descuento aplicado
    [15:06:39] Silvia Vivar: el campo price, ya contiene el precio menos el descuento
    [15:07:23] Silvia Vivar: por lo tanto quantity *  price es el subtotal por línea
                 */

                response.Exito = true;
                response.CodigoError = "";
                response.DescripcionError = "";
                response.CodigoRespuesta = "OK";
                // response.EntradaRAW = cab;

                XmlSerializer xmlSerializer = new XmlSerializer(det.GetType());
                StringWriter textWriter = new StringWriter();

                xmlSerializer.Serialize(textWriter, det);
                response.EntradaRAW = textWriter.ToString();
                det = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                response.Exito = false;
                response.DescripcionError = e.Message;
                response.CodigoError = "CAB001";
                response.CodigoRespuesta = "CAB001";
                response.EntradaRAW = "";

                logs.grabarLog("DETALLE_ORDEN", e.Message);
                logs.grabarLog("DETALLE_ORDEN_DEBUG", e.StackTrace);
            }
            finally
            {
                DataBase.DesconectaDB(company);
            }

            if (oDoc != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(oDoc);
            oDoc = null;

            //GC.WaitForPendingFinalizers();
            //GC.Collect();

            return response;
        }

    }
}