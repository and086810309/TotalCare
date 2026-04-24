using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Web.Services;
using SAPbobsCOM;

namespace WSMyDealerSAPv3
{
    /// <summary>
    /// Descripción breve de WSIntegracion
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio Web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class WSIntegracion : System.Web.Services.WebService
    {
        [WebMethod(Description = "DESA - Permite verificar funcionamiento de DI API")]
        public string verificarSAPDIAPI()
        {
            List<String> response = new List<String>();
            string respuesta = "";
            try
            {
                Company company = null;
                company = DataBase.conectar();
                if (!DataBase.Respuesta.Exito)
                {
                    DatosEnlace.release_variables();
                    return DataBase.Respuesta.CodigoError + " - " + DataBase.Respuesta.CodigoRespuesta + " - " + DataBase.Respuesta.DescripcionError;
                }
                else
                {
                    DatosEnlace.release_variables();
                    DataBase.DesconectaDB(company);
                    return "Correcto - Compañia conectada por DI API";
                }
            }
            catch (Exception e)
            {
                DatosEnlace.release_variables();
                logs.grabarLog("LOG_WSINTEGRACION", e.Message);
                logs.grabarLog("LOG_WSINTEGRACION", e.StackTrace);
                return DataBase.Respuesta.CodigoError + " - " + DataBase.Respuesta.CodigoRespuesta + " - " + DataBase.Respuesta.DescripcionError;
            }
        }

        [WebMethod(Description = "DESA - Permite obtener XML de datos de vistas")]
        public String obtenerDatosTablaVista(String nombreTablaVista, int inicio, int cantidad)
        {
            String respuesta = "";

            try
            {
                logs.grabarLog("LOG_GET", "TablaVista " + nombreTablaVista + " :: " + inicio + " :: " + cantidad);
                respuesta = General.obtenerDatosTablaVista(nombreTablaVista, inicio, cantidad);

                DatosEnlace.release_variables();
            }
            catch (Exception e)
            {
                logs.grabarLog("LOG_GET", e.Message);
                logs.grabarLog("LOG_GET", e.StackTrace);
            }
            /*  lq... no entiendo xq comprimir y descomprimir ????? lo comente
            clases.Compresion comp = new clases.Compresion();
            string val = comp.CompressString(respuesta);

            val = comp.DecompressString(val);
            
            return val;
            */
            return respuesta;
        }

        //[WebMethod(Description = "Permite actualizar el email del cliente en SAP BO")]
        //public string updateEmailCliente(string cliente, string email)
        //{
        //    string respuesta = "";
        //    try
        //    {
        //        respuesta = Clientes.actualizarEmail(cliente, email);
        //        DatosEnlace.release_variables();
        //    }
        //    catch (Exception e)
        //    {
        //        logs.grabarLog("LOG_WSINTEGRACION", e.Message);
        //        logs.grabarLog("LOG_WSINTEGRACION", e.StackTrace);
        //    }
        //    return respuesta;
        //}


        [WebMethod(Description = "DESA - Permite verificar si el pedido existe")]
        public RespuestaExistenciaPedido existePedido(int numPedido)
        {
            RespuestaExistenciaPedido respuesta = new RespuestaExistenciaPedido();

            try
            {
                logs.grabarLog("existePedido", numPedido + "");

                respuesta = Pedidos.existePedido(numPedido);

                logs.grabarLog("existePedido", respuesta.ExistePedido + "");

                DatosEnlace.release_variables();
            }
            catch (Exception e)
            {
                logs.grabarLog("LOG_WSINTEGRACION", e.Message);
                logs.grabarLog("LOG_WSINTEGRACION", e.StackTrace);
            }
            return respuesta;
        }

        /**
         * Proxy para el ingreso de ordenes
         * @param cabecera La cabecera de la orden a ingresar
         * @param detalles La lista de detalles a ingresar
         * @return El objeto de respuesta con los datos del ingreso exitoso o fallid
                respuesta = Estandarizador.procesarRespuesta(salida);o
         */
        [WebMethod(Description = "DESA - Permite ingresar una orden MyDealer en SAP BO")]
        public Respuesta ingresarOrden(CabeceraOrden cabecera, string detalles)
        {
            Respuesta respuesta = new Respuesta();

            try
            {
                //logs.grabarLog("PEDIDOS", "detalle:" + detalles);

                List<DetalleOrden> list_detalle = new List<DetalleOrden>(); ;

                list_detalle = Pedidos.StringToList(detalles);

                RespuestaExistenciaPedido existe = Pedidos.existePedido(cabecera.NumeroOrdenWeb);
                
                logs.grabarLog("PEDIDOS", "existe.ExistePedido: " + existe.ExistePedido.ToString());

                if (existe.ExistePedido)
                {
                    respuesta.CodigoError = "1";
                    respuesta.CodigoRespuesta = "ING001:-:" + existe.ExistePedido.ToString() + ":-:" + existe.NumeroPedidoSAP;
                    respuesta.DescripcionError = "Pedido existe";
                    respuesta.Exito = false;
                }
                else
                {
                    List<String> salida = Pedidos.ingresarOrden(cabecera, list_detalle);
                    respuesta = Estandarizador.procesarRespuesta(salida);
                }

                DatosEnlace.release_variables();
            }
            catch (Exception e)
            {
                respuesta.Exito = false;
                respuesta.DescripcionError = e.Message;

                logs.grabarLog("LOG_WSINTEGRACION", e.Message);
                logs.grabarLog("LOG_WSINTEGRACION", e.StackTrace);
            }
            return respuesta;
        }

        [WebMethod(Description = "DESARROLLO - Permite ingresar una Nota de debito en SAP BO")]
        public Respuesta ingresarNotaDebito(CabeceraNotaDeDebito cabecera, string detalles)
        {
            Respuesta respuesta = new Respuesta();

            try
            {

                List<DetalleNotaDeDebito> list_detalle = new List<DetalleNotaDeDebito>
                    {
                        new DetalleNotaDeDebito
                        {
                            Wsc = "BGG",
                            CodigoProducto = detalles,
                            CantidadProducto = 1
                        },
                    };

                bool existe = NotasDebito.existeNotaDebito(cabecera.NumeroNotaDeDebito);

                logs.grabarLog("PEDIDOS", "existe.ExistePedido: "+ cabecera.NumeroNotaDeDebito + " " + existe);

                if (existe)
                {
                    respuesta.Exito = true;
                    respuesta.CodigoRespuesta = cabecera.NumeroNotaDeDebito;
                    respuesta.EntradaRAW = "";
                }
                else
                {
                    List<String> salida = NotasDebito.IngresarArticulo(cabecera, list_detalle);
                    respuesta = Estandarizador.procesarRespuesta(salida);
                }

                DatosEnlace.release_variables();
            }
            catch (Exception e)
            {
                respuesta.Exito = false;
                respuesta.DescripcionError = e.Message;

                logs.grabarLog("LOG_WSINTEGRACION", e.Message);
                logs.grabarLog("LOG_WSINTEGRACION", e.StackTrace);
            }
            return respuesta;
        }


        [WebMethod(Description = "DESA - Permite guardar una cobranza")]
        public Respuesta guardarCobranza(SeccionPagosEfectivo cabeceraPagos, string xmlDocumentos, string xmlPagos)
        {
            Respuesta respuesta = new Respuesta();

            try
            {
                logs.grabarLog("COBRANZA_LOG", "INI:");
                //logs.grabarLog("COBRANZA_LOG", "DOCS:"+ xmlDocumentos);
                //logs.grabarLog("COBRANZA_LOG", "PAGOS:" + xmlPagos);
                List<SeccionDocumentos> documentos = Cobranza.StringToListSeccionDocumentos(xmlDocumentos);
                List<Pago> pagos = Cobranza.StringToListPago(xmlPagos);

                List<String> salida = Cobranza.ingresarCobranza(cabeceraPagos, documentos, pagos);
                respuesta = Estandarizador.procesarRespuesta(salida);

                DatosEnlace.release_variables();
            }
            catch (Exception e)
            {
                logs.grabarLog("LOG_WSINTEGRACION", e.Message);
                logs.grabarLog("LOG_WSINTEGRACION", e.StackTrace);
            }
            return respuesta;
        }


        //[WebMethod]
        //public Respuesta obtenerCabeceraOrden(int numorden, int numordenweb)
        //{
        //    Respuesta respuesta = null;
        //    try
        //    {
        //        respuesta = Pedidos.obtenerCabeceraOrden(numorden, numordenweb);

        //        DatosEnlace.release_variables();
        //    }
        //    catch (Exception e)
        //    {
        //        logs.grabarLog("LOG_WSINTEGRACION", e.Message);
        //        logs.grabarLog("LOG_WSINTEGRACION", e.StackTrace);
        //    }
        //    return respuesta;
        //}


        //[WebMethod]
        //public Respuesta obtenerDetalleOrden(int numorden, int numordenweb)
        //{
        //    Respuesta respuesta = null;
        //    try
        //    {
        //        respuesta = Pedidos.obtenerDetalleOrden(numorden, numordenweb);

        //        DatosEnlace.release_variables();
        //    }
        //    catch (Exception e)
        //    {
        //        logs.grabarLog("LOG_WSINTEGRACION", e.Message);
        //        logs.grabarLog("LOG_WSINTEGRACION", e.StackTrace);
        //    }
        //    return respuesta;
        //}


        //[WebMethod]
        //public String ingresarDevolucion(String devolucion)
        //{
        //    String retorno = "";
        //    try
        //    {

        //        XmlDocument xmlDevolucion = new XmlDocument();
        //        xmlDevolucion.LoadXml(devolucion);

        //        // XmlNode mainDevolucion = xmlDevolucion.FirstChild;
        //        XmlNodeList mainDevolucion = xmlDevolucion.GetElementsByTagName("Devoluciones");
        //        XmlElement datosDevolucion = (XmlElement)mainDevolucion[0];


        //        retorno = Devoluciones.existeDevolucion(datosDevolucion["Codevolucion"].InnerText);
        //        XmlDocument xmlVerificacion = new XmlDocument();
        //        xmlVerificacion.LoadXml(retorno);

        //        XmlNodeList mainVerificacion = xmlVerificacion.GetElementsByTagName("Respuesta");
        //        XmlElement datosVerificacion = (XmlElement)mainVerificacion[0];

        //        if (datosVerificacion["Exito"].InnerText == "true")
        //        {
        //            retorno = Devoluciones.ingresarDevolucion(devolucion);
        //        }

        //        DatosEnlace.release_variables();
        //    }
        //    catch (Exception e)
        //    {
        //        logs.grabarLog("LOG_WSINTEGRACION", e.Message);
        //        logs.grabarLog("LOG_WSINTEGRACION", e.StackTrace);
        //    }
        //    return retorno;
        //}


        //[WebMethod(Description = "Permite verificar si la devolucion existe")]
        //public String existeDevolucion(String numDevolucion)
        //{
        //    String respuesta = "";

        //    try
        //    {
        //        respuesta = Devoluciones.existeDevolucion(numDevolucion);

        //        DatosEnlace.release_variables();
        //    }
        //    catch (Exception e)
        //    {
        //        logs.grabarLog("LOG_WSINTEGRACION", e.Message);
        //        logs.grabarLog("LOG_WSINTEGRACION", e.StackTrace);
        //    }
        //    return respuesta;
        //}


        //[WebMethod(Description = "Permite verificar si la devolucion existe")]
        //public String registrarCllienteNuevo(String xmlDatos)
        //{
        //    return Clientes.crearCliente(xmlDatos);
        //}


        //[WebMethod(Description = "Permite verificar si la devolucion existe")]
        //public String existeSolicitudNotaCredito(String numSolicitud)
        //{
        //    String respuesta = "";

        //    try
        //    {
        //        respuesta = NotasCredito.existeNotaCredito(numSolicitud);

        //        DatosEnlace.release_variables();
        //    }
        //    catch (Exception e)
        //    {
        //        logs.grabarLog("LOG_WSINTEGRACION", e.Message);
        //        logs.grabarLog("LOG_WSINTEGRACION", e.StackTrace);
        //    }
        //    return respuesta;
        //}


        //[WebMethod]
        //public String ingresarSolicitudNotaCredito(String xml_solicitud)
        //{
        //    String retorno = "";
        //    try
        //    {
        //        XmlDocument xmlDevolucion = new XmlDocument();
        //        xmlDevolucion.LoadXml(xml_solicitud);

        //        // XmlNode mainDevolucion = xmlDevolucion.FirstChild;
        //        XmlNodeList mainDevolucion = xmlDevolucion.GetElementsByTagName("NotasCredito");
        //        XmlElement datosDevolucion = (XmlElement)mainDevolucion[0];


        //        retorno = NotasCredito.existeNotaCredito(datosDevolucion["NumeroSolicitud"].InnerText);
        //        XmlDocument xmlVerificacion = new XmlDocument();
        //        xmlVerificacion.LoadXml(retorno);

        //        XmlNodeList mainVerificacion = xmlVerificacion.GetElementsByTagName("Respuesta");
        //        XmlElement datosVerificacion = (XmlElement)mainVerificacion[0];

        //        if (datosVerificacion["Exito"].InnerText == "true")
        //        {
        //            retorno = NotasCredito.ingresarNotaCredito(xml_solicitud);
        //        }

        //        DatosEnlace.release_variables();
        //    }
        //    catch (Exception e)
        //    {
        //        logs.grabarLog("LOG_WSINTEGRACION", e.Message);
        //        logs.grabarLog("LOG_WSINTEGRACION", e.StackTrace);
        //    }
        //    return retorno;
        //}


        ///*[WebMethod]
        //public String demoIngresarDevolucion()
        //{
        //    return Devoluciones.ingresarDevolucion("<?xml version='1.0' encoding='ISO-8859-1'?> " 
        //        + "<Devoluciones><PedirAutorizacion>S</PedirAutorizacion><FechaRegistro>2016-06-17</FechaRegistro><Establecimiento>001</Establecimiento>"
        //        + "<PuntoEmision>002</PuntoEmision><NumeroFactura>000000003</NumeroFactura><Codevolucion>1</Codevolucion><Origen>MD</Origen>"
        //        + "<Codcliente>AC0100045004001</Codcliente><Coddireccioncliente>PRINCIPAL</Coddireccioncliente><Codvendedor>10</Codvendedor>"
        //        + "<Detalles><Detalle><Codproducto>JUGTE0075</Codproducto><Cantidad>10</Cantidad><Precio>19.4300</Precio><Unidad>UNIDAD</Unidad>"
        //        + "<Motivo>motivo 1</Motivo><Observacion>observacion motivo 1</Observacion><NumeroLote></NumeroLote></Detalle><Detalle>"
        //        + "<Codproducto>VAJIL0026NEW</Codproducto><Cantidad>20</Cantidad><Precio>9.9500</Precio><Unidad>SET</Unidad><Motivo>motivo 2</Motivo>"
        //        + "<Observacion>observacion motivo 2</Observacion><NumeroLote></NumeroLote></Detalle></Detalles></Devoluciones>");
        //}*/



        //[WebMethod]
        //public Respuesta demoCobranza()
        //{
        //    Respuesta respuesta = new Respuesta();

        //    SeccionPagosEfectivo cabeceraPagos = new SeccionPagosEfectivo();
        //    List<SeccionDocumentos> documentos = new List<SeccionDocumentos>();
        //    List<Pago> pagos = new List<Pago>();


        //    cabeceraPagos.CodigoCliente = "C1111111116";
        //    cabeceraPagos.CuentaCaja = "_SYS00000000175";
        //    cabeceraPagos.FechaDocumento = "2017-01-07";
        //    cabeceraPagos.FechaVencimiento = "2017-01-07";
        //    cabeceraPagos.MontoEfectivo = 3;
        //    cabeceraPagos.MontoLetra = 0;
        //    cabeceraPagos.NumeroFisico = "25";
        //    cabeceraPagos.NumeroMBW = "805";
        //    cabeceraPagos.Observaciones1 = "";
        //    cabeceraPagos.Observaciones2 = "";

        //    // cabeceraPagos.CuentaTransferencia
        //    cabeceraPagos.ReferenciaTransferencia = "";
        //    cabeceraPagos.CodigoCobrador = "6";


        //    SeccionDocumentos doc = new SeccionDocumentos();
        //    doc.MontoRetencion = 0;
        //    doc.NumeroCuota = 1;
        //    doc.NumeroCuotaSAP = 1;
        //    doc.NumeroDocumento = 34;
        //    doc.NumeroRetencion = "";
        //    doc.TipoDocumento = "FAC";
        //    doc.Valor = 1;

        //    documentos.Add(doc);

        //    doc = new SeccionDocumentos();
        //    doc.MontoRetencion = 0;
        //    doc.NumeroCuota = 2;
        //    doc.NumeroCuotaSAP = 2;
        //    doc.NumeroDocumento = 34;
        //    doc.NumeroRetencion = "";
        //    doc.TipoDocumento = "FAC";
        //    doc.Valor = 1;

        //    documentos.Add(doc);

        //    doc = new SeccionDocumentos();
        //    doc.MontoRetencion = 0;
        //    doc.NumeroCuota = 3;
        //    doc.NumeroCuotaSAP = 3;
        //    doc.NumeroDocumento = 34;
        //    doc.NumeroRetencion = "";
        //    doc.TipoDocumento = "FAC";
        //    doc.Valor = 1;

        //    documentos.Add(doc);


        //    //SeccionDocumentos doc = new SeccionDocumentos();
        //    //doc.MontoRetencion = 0;
        //    //doc.NumeroCuota = 5;
        //    //doc.NumeroCuotaSAP = 1;
        //    //doc.NumeroDocumento = 168;
        //    //doc.NumeroRetencion = "";
        //    //doc.TipoDocumento = "NDI1";
        //    //doc.Valor = 0.25;

        //    //documentos.Add(doc);

        //    //doc = new SeccionDocumentos();
        //    //doc.MontoRetencion = 0;
        //    //doc.NumeroCuota = 1;
        //    //doc.NumeroCuotaSAP = 1;
        //    //doc.NumeroDocumento = 33;
        //    //doc.NumeroRetencion = "";
        //    //doc.TipoDocumento = "ND";
        //    //doc.Valor = 0.75;

        //    //documentos.Add(doc);

        //    //SeccionDocumentos doc = new SeccionDocumentos();
        //    //doc.MontoRetencion = 0;
        //    //doc.NumeroCuota = 1;
        //    //doc.NumeroCuotaSAP = 1;
        //    //doc.NumeroDocumento = 186;
        //    //doc.NumeroRetencion = "";
        //    //doc.TipoDocumento = "CHQPRO";
        //    //doc.Valor = 0.5;

        //    //documentos.Add(doc);

        //    //doc = new SeccionDocumentos();
        //    //doc.MontoRetencion = 0;
        //    //doc.NumeroCuota = 1;
        //    //doc.NumeroCuotaSAP = 1;
        //    //doc.NumeroDocumento = 184;
        //    //doc.NumeroRetencion = "";
        //    //doc.TipoDocumento = "NDI2";
        //    //doc.Valor = 0.5;

        //    //documentos.Add(doc);


        //    Pago p = new Pago();
        //    p.TipoPago = "EFE";
        //    pagos.Add(p);


        //    List<string> salida = Cobranza.ingresarCobranza(cabeceraPagos, documentos, pagos);
        //    respuesta = Estandarizador.procesarRespuesta(salida);

        //    return respuesta;
        //}



        ///*[WebMethod]
        //public Respuesta demoOrden(int numOrdenWeb)
        //{
        //    Respuesta response = new Respuesta();

        //    CabeceraOrden cab = new CabeceraOrden();
        //    cab.CodigoCliente = "C0927140038";
        //    cab.FechaEntregaPedido = "2016/07/18";
        //    cab.FechaGeneracionPedido = "2016/07/26";
        //    cab.NumeroOrdenWeb = numOrdenWeb;
        //    cab.Observaciones = "PRUEBA DESDE EL WEB SERVICE";
        //    cab.CodigoFormaPago = 1;
        //    cab.CodigoVendedor = 1;
        //    cab.Pedidoaprobado = "N";
        //    cab.Ruta_logistica = "R001";
        //    cab.TotalPedido = 17.92;
        //    cab.TotalGasto = 1.92;
        //    cab.CodigoDireccionEnvio = "DOS"; // PRINCIPAL
        //    cab.Ruta_secuencia = 0;
        //    cab.Pedir_autorizacion = "S";
        //    cab.Tipo_pedido = "NOR";
        //    // cab.Iva = "IVA_14";



        //    DetalleOrden det = new DetalleOrden();
        //    det.Wsc = "01";
        //    det.CodigoProducto = "A00002";
        //    det.CantidadProducto = double.Parse("5.00");
        //    det.PrecioProducto = double.Parse("8.00");
        //    det.PorcentajeDescuento = double.Parse("0.00");
        //    det.UndidadCaja = -1;
        //    det.UndEntry = 0;


        //    DetalleOrden det2 = new DetalleOrden();
        //    det2.Wsc = "01";
        //    det2.CodigoProducto = "ART0001";
        //    det2.CantidadProducto = double.Parse("4.00");
        //    det2.PrecioProducto = double.Parse("72.00");
        //    det2.PorcentajeDescuento = double.Parse("0.00");
        //    det2.Codunidadmedida = "Paquetex6";
        //    det2.UndidadCaja = 36;
        //    det2.UndEntry = 3;


        //    DetalleOrden det3 = new DetalleOrden();
        //    det3.Wsc = "01";
        //    det3.CodigoProducto = "ART0001";
        //    det3.CantidadProducto = double.Parse("2.00");
        //    det3.PrecioProducto = double.Parse("12.00");
        //    det3.PorcentajeDescuento = double.Parse("0.00");
        //    det3.Codunidadmedida = "Caja6";
        //    det3.UndidadCaja = 6;
        //    det3.UndEntry = 2;


        //    DetalleOrden det4 = new DetalleOrden();
        //    det4.Wsc = "01";
        //    det4.CodigoProducto = "ART0001";
        //    det4.CantidadProducto = double.Parse("5.00");
        //    det4.PrecioProducto = double.Parse("2.00");
        //    det4.PorcentajeDescuento = double.Parse("0.00");
        //    det4.Codunidadmedida = "Unidad";
        //    det4.UndidadCaja = 1;
        //    det4.UndEntry = 1;

        //    List<DetalleOrden> listDet = new List<DetalleOrden>();
        //    //listDet.Add(det);
        //    listDet.Add(det3);
        //    listDet.Add(det2);
        //    listDet.Add(det4);

        //    //listDet.Clear();
        //    //listDet = Pedidos.StringToList("<?xml version='1.0' encoding='UTF-8'?>	<items><item>	<NumeroLineaDetalle><![CDATA[1]]></NumeroLineaDetalle>	<CodigoProducto><![CDATA[40303901]]></CodigoProducto>	<CantidadProducto><![CDATA[11.00]]></CantidadProducto>	<PorcentajeDescuento><![CDATA[0.0000000000]]></PorcentajeDescuento>	<PrecioProducto><![CDATA[4.0000]]></PrecioProducto>	<TotalLinea><![CDATA[44.00]]></TotalLinea>	<CodigoDocumentoRelacionado><![CDATA[]]></CodigoDocumentoRelacionado>	<Wsc><![CDATA[01]]></Wsc>	<Detalle_promocion><![CDATA[]]></Detalle_promocion>	<Detalle_descuento><![CDATA[0.00|145|2013-07-05| :) ]]></Detalle_descuento>	<Promocion_estado><![CDATA[B]]></Promocion_estado>	<Piezas><![CDATA[11]]></Piezas>	<Peso><![CDATA[1.00]]></Peso>	<Peso_total><![CDATA[11.00]]></Peso_total>	</item><item>	<NumeroLineaDetalle><![CDATA[2]]></NumeroLineaDetalle>	<CodigoProducto><![CDATA[41117901]]></CodigoProducto>	<CantidadProducto><![CDATA[1.00]]></CantidadProducto>	<PorcentajeDescuento><![CDATA[0.0000000000]]></PorcentajeDescuento>	<PrecioProducto><![CDATA[7.0000]]></PrecioProducto>	<TotalLinea><![CDATA[7.00]]></TotalLinea>	<CodigoDocumentoRelacionado><![CDATA[]]></CodigoDocumentoRelacionado>	<Wsc><![CDATA[01]]></Wsc>	<Detalle_promocion><![CDATA[]]></Detalle_promocion>	<Detalle_descuento><![CDATA[0.00|145|2013-07-05| :) ]]></Detalle_descuento>	<Promocion_estado><![CDATA[N]]></Promocion_estado>	<Piezas><![CDATA[1]]></Piezas>	<Peso><![CDATA[1.00]]></Peso>	<Peso_total><![CDATA[1.00]]></Peso_total></item><item>	<NumeroLineaDetalle><![CDATA[3]]></NumeroLineaDetalle>	<CodigoProducto><![CDATA[40615902]]></CodigoProducto>	<CantidadProducto><![CDATA[10.00]]></CantidadProducto>	<PorcentajeDescuento><![CDATA[0.0000000000]]></PorcentajeDescuento>	<PrecioProducto><![CDATA[0.0000]]></PrecioProducto>	<TotalLinea><![CDATA[0.00]]></TotalLinea>	<CodigoDocumentoRelacionado><![CDATA[]]></CodigoDocumentoRelacionado>	<Wsc><![CDATA[01]]></Wsc>	<Detalle_promocion><![CDATA[ promo=7 sec=21 prod=41117901 promo=7 sec=21 prod=41101901 promo=7 sec=21 prod=40305901 promo=7 sec=21 prod=40303901(Aplica:10.0000)]]></Detalle_promocion>	<Detalle_descuento><![CDATA[]]></Detalle_descuento>	<Promocion_estado><![CDATA[P]]></Promocion_estado>	<Piezas><![CDATA[10]]></Piezas>	<Peso><![CDATA[1.00]]></Peso>	<Peso_total><![CDATA[10.00]]></Peso_total></item></items>");


        //    List<String> salida = Pedidos.ingresarOrden(cab, listDet);
        //    response = Estandarizador.procesarRespuesta(salida);

        //    return response;
        //}*/



        ///*[WebMethod]
        //public List<DetalleOrden> temporalXMLDetalleOrden(string xml)
        //{
        //    return Pedidos.StringToList(xml);
        //}*/



        ///*[WebMethod]
        //public List<SeccionDocumentos> temporalXMLSeccionDocumentos(string xml) {
        //    return Cobranza.StringToListSeccionDocumentos(xml);
        //}*/



        ///*[WebMethod]
        //public List<Pago> temporalXMLPagos(string xml)
        //{
        //    return Cobranza.StringToListPago(xml);
        //}*/



        /*[WebMethod]
        public Respuesta demoExisteCobranza()
        {
            Respuesta respuesta = new Respuesta();
            RespuestaExistenciaPedido existe = Cobranza.existeCobranza("800");
            if (existe.ExistePedido)
            {
                respuesta.CodigoError = "1";
                respuesta.CodigoRespuesta = "COB001";
                respuesta.DescripcionError = "Cobranza existe";
                respuesta.Exito = false;
            }
            else
            {
                respuesta.CodigoError = "0";
                respuesta.CodigoRespuesta = "--";
                respuesta.DescripcionError = "Cobranza no existe";
                respuesta.Exito = true;
            }
            return respuesta;
        }*/






    }
}
