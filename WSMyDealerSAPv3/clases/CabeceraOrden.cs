using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class CabeceraOrden
    {

        private int numeroOrdenWeb; // numero_pedido
        public int NumeroOrdenWeb
        {
            get { return numeroOrdenWeb; }
            set { numeroOrdenWeb = value; }
        }

        private int numeroOrden;
        public int NumeroOrden
        {
            get { return numeroOrden; }
            set { numeroOrden = value; }
        }

        private string codigoCliente; // codigo_cliente
        public string CodigoCliente
        {
            get { return codigoCliente; }
            set { codigoCliente = value; }
        }

        private string codigoCobrador; // Agent_Code
        public string CodigoCobrador
        {
            get { return codigoCobrador; }
            set { codigoCobrador = value; }
        }

        private string codigoDireccionEnvio; // codigo_direccion_envio
        public string CodigoDireccionEnvio
        {
            get { return codigoDireccionEnvio; }
            set { codigoDireccionEnvio = value; }
        }

        private string fechaGeneracionPedido; // fecha_registro_pedido
        public string FechaGeneracionPedido
        {
            get { return fechaGeneracionPedido; }
            set { fechaGeneracionPedido = value; }
        }

        private string fechaEntregaPedido;   //fecha de despacho del pedido
        public string FechaEntregaPedido
        {
            get { return fechaEntregaPedido; }
            set { fechaEntregaPedido = value; }
        }

        private int codigoVendedor; // codigo_vendedor
        public int CodigoVendedor
        {
            get { return codigoVendedor; }
            set { codigoVendedor = value; }
        }

        private string observaciones; // observaciones
        public string Observaciones
        {
            get { return observaciones; }
            set { observaciones = value; }
        }

        private string numeroOrdenPedidoFisico; // numero_fisico+
        public string NumeroOrdenPedidoFisico
        {
            get { return numeroOrdenPedidoFisico; }
            set { numeroOrdenPedidoFisico = value; }
        }

        private string aprobadoventa;    //Indicador de aprobacion por venta
        public string Aprobadoventa
        {
            get { return aprobadoventa; }
            set { aprobadoventa = value; }
        }

        private string codigoTransportista; // codigo_transportista
        public string CodigoTransportista
        {
            get { return codigoTransportista; }
            set { codigoTransportista = value; }
        }

        private string pedidoaprobado;   //indicador de aprobacion general de pedido
        public string Pedidoaprobado
        {
            get { return pedidoaprobado; }
            set { pedidoaprobado = value; }
        }

        private int codigoFormaPago; // codigo_formapago
        public int CodigoFormaPago
        {
            get { return codigoFormaPago; }
            set { codigoFormaPago = value; }
        }

        private string codigoServicioCliente; // codigo_servicio_cliente
        public string CodigoServicioCliente
        {
            get { return codigoServicioCliente; }
            set { codigoServicioCliente = value; }
        }

        private string codmotivonoaprobacion;    //Codigo del motivo por el cual el pedido no es aprobado
        public string Codmotivonoaprobacion
        {
            get { return codmotivonoaprobacion; }
            set { codmotivonoaprobacion = value; }
        }

        private string detallemotivoaprobacion; // Detalle del motivo de aprobacion
        public string Detallemotivoaprobacion
        {
            get { return detallemotivoaprobacion; }
            set { detallemotivoaprobacion = value; }
        }

        private double totalPedido; // DocTotal
        public double TotalPedido
        {
            get { return totalPedido; }
            set { totalPedido = value; }
        }

        private string descprioridad;    //Decripcion de la prioridad del pedido
        public string Descprioridad
        {
            get { return descprioridad; }
            set { descprioridad = value; }
        }

        // campos adicionales por flete - solo aplica en Ecuador
        private double totalGasto;
        public double TotalGasto
        {
            get { return totalGasto; }
            set { totalGasto = value; }
        }

        // Campos usados cuando cabecera de orden se usa en reclamos - jvillavi
        private string numeroReclamo;
        public string NumeroReclamo
        {
            get { return numeroReclamo; }
            set { numeroReclamo = value; }
        }

        private string codigoMotivoReclamo;
        public string CodigoMotivoReclamo
        {
            get { return codigoMotivoReclamo; }
            set { codigoMotivoReclamo = value; }
        }

        string ruta_logistica;
        public string Ruta_logistica
        {
            get { return ruta_logistica; }
            set { ruta_logistica = value; }
        }

        int ruta_secuencia;
        public int Ruta_secuencia
        {
            get { return ruta_secuencia; }
            set { ruta_secuencia = value; }
        }

        string pedir_autorizacion;
        public string Pedir_autorizacion
        {
            get { return pedir_autorizacion; }
            set { pedir_autorizacion = value; }
        }

        string tipo_pedido;
        public string Tipo_pedido
        {
            get { return tipo_pedido; }
            set { tipo_pedido = value; }
        }

        double subotal;
        public double Subotal
        {
            get { return subotal; }
            set { subotal = value; }
        }

        double valor_descto;
        public double Valor_descto
        {
            get { return valor_descto; }
            set { valor_descto = value; }
        }

        int series;
        public int  Series
        {
            get { return series; }
            set { series = value; }
        }

        /*String iva;
        public String Iva
        {
            get { return iva; }
            set { iva = value; }
        }*/

        string undEntry;
        public string UndEntry
        {
            get { return undEntry; }
            set { undEntry = value; }
        }

        string undCode;
        public string UndCode
        {
            get { return undCode; }
            set { undCode = value; }
        }



        
        string tipopago;
        public string Tipopago
        {
            get { return tipopago; }
            set { tipopago = value; }
        }

        
        string acumGalonaje;
        public string AcumGalonaje
        {
            get { return acumGalonaje; }
            set { acumGalonaje = value; }
        }

        
        string porcIncremento;
        public string PorcIncremento
        {
            get { return porcIncremento; }
            set { porcIncremento = value; }
        }

        
        string galonaje;
        public string Galonaje
        {
            get { return galonaje; }
            set { galonaje = value; }
        }



        string preguntas;
        public string Preguntas
        {
            get { return preguntas; }
            set { preguntas = value; }
        }

        string proyecto;
        public string Proyecto
        {
            get { return proyecto; }
            set { proyecto = value; }
        }

        string ciudprov;
        public string CiudProv
        {
            get { return ciudprov; }
            set { ciudprov = value; }
        }

        string codruta;
        public string CodRuta
        {
            get { return codruta; }
            set { codruta = value; }
        }

        string zona;
        public string Zona
        {
            get { return zona; }
            set { zona = value; }
        }

        string subzona;
        public string Subzona
        {
            get { return subzona; }
            set { subzona = value; }
        }

        string origen;
        public string Origen
        {
            get { return origen; }
            set { origen = value; }
        }


    }
}