using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class DetalleOrden
    {
        private int numeroLineaDetalle; // secuencia

        public int NumeroLineaDetalle
        {
            get { return numeroLineaDetalle; }
            set { numeroLineaDetalle = value; }
        }
        private string codigoProducto; // codigo_producto

        public string CodigoProducto
        {
            get { return codigoProducto; }
            set { codigoProducto = value; }
        }
        private double cantidadProducto; // cantidad_producto

        public double CantidadProducto
        {
            get { return cantidadProducto; }
            set { cantidadProducto = value; }
        }
        private Double porcentajeDescuento; // descuento_producto

        public Double PorcentajeDescuento
        {
            get { return porcentajeDescuento; }
            set { porcentajeDescuento = value; }
        }
        private double precioProducto; // precio_lista

        public double PrecioProducto
        {
            get { return precioProducto; }
            set { precioProducto = value; }
        }
        private string wsc; // codigo_bodega

        public string Wsc
        {
            get { return wsc; }
            set { wsc = value; }
        }
        private double totalLinea; // pxq (incluye desc)

        public double TotalLinea
        {
            get { return totalLinea; }
            set { totalLinea = value; }
        }

        // campos exclusivos para el caso reclamos - jvillavi
        private string codigoDocumentoRelacionado;

        public string CodigoDocumentoRelacionado
        {
            get { return codigoDocumentoRelacionado; }
            set { codigoDocumentoRelacionado = value; }
        }

        string detalle_descuento;

        public string Detalle_descuento
        {
            get { return detalle_descuento; }
            set { detalle_descuento = value; }
        }
        string detalle_promocion;

        public string Detalle_promocion
        {
            get { return detalle_promocion; }
            set { detalle_promocion = value; }
        }

        string promocion_estado;

        public string Promocion_estado
        {
            get { return promocion_estado; }
            set { promocion_estado = value; }
        }

        int piezas;

        public int Piezas
        {
            get { return piezas; }
            set { piezas = value; }
        }
        double peso;

        public double Peso
        {
            get { return peso; }
            set { peso = value; }
        }
        double peso_total;

        public double Peso_total
        {
            get { return peso_total; }
            set { peso_total = value; }
        }


        double valor_dscto;
        public double Valor_dscto
        {
            get { return valor_dscto; }
            set { valor_dscto = value; }
        }

        double subtotal;
        public double Subtotal
        {
            get { return subtotal; }
            set { subtotal = value; }
        }

        string codunidadmedida;
        public string Codunidadmedida
        {
            get { return codunidadmedida; }
            set { codunidadmedida = value; }
        }

        double porcIva;
        public double PorcIva
        {
            get { return porcIva; }
            set { porcIva = value; }
        }



        int undEntry;
        public int UndEntry
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


        double undidadCaja;
        public double UndidadCaja
        {
            get { return undidadCaja; }
            set { undidadCaja = value; }
        }


        double galones;
        public double Galones
        {
            get { return galones; }
            set { galones = value; }
        }

       

    }
}