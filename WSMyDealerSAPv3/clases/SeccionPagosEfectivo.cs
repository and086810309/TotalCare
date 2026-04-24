using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class SeccionPagosEfectivo
    {
        private string fechaDocumento; // DocDate

        public string FechaDocumento
        {
            get { return fechaDocumento; }
            set { fechaDocumento = value; }
        }

        private string codigoCliente; // CarCode
        public string CodigoCliente
        {
            get { return codigoCliente; }
            set { codigoCliente = value; }
        }
        
        private string cuentaCaja; // CashAccount
        public string CuentaCaja
        {
            get { return cuentaCaja; }
            set { cuentaCaja = value; }
        }

        private double montoEfectivo; // CashSum
        public double MontoEfectivo
        {
            get { return montoEfectivo; }
            set { montoEfectivo = value; }
        }

        private string cuentaTransferencia; // TransferAccount
        public string CuentaTransferencia
        {
            get { return cuentaTransferencia; }
            set { cuentaTransferencia = value; }
        }

        private double sumaTransferencia; // TransferSum
        public double SumaTransferencia
        {
            get { return sumaTransferencia; }
            set { sumaTransferencia = value; }
        }

        private string fechaTransferencia; // TransferDate
        public string FechaTransferencia
        {
            get { return fechaTransferencia; }
            set { fechaTransferencia = value; }
        }

        private string referenciaTransferencia; // TransferReference
        public string ReferenciaTransferencia
        {
            get { return referenciaTransferencia; }
            set { referenciaTransferencia = value; }
        }

        private string observaciones1; // Reference1
        public string Observaciones1
        {
            get { return observaciones1; }
            set { observaciones1 = value; }
        }

        private string observaciones2; // Reference2
        public string Observaciones2
        {
            get { return observaciones2; }
            set { observaciones2 = value; }
        }

        private string fechaVencimiento; // DueDate
        public string FechaVencimiento
        {
            get { return fechaVencimiento; }
            set { fechaVencimiento = value; }
        }

        private double montoLetra; // BillOfExchangeAmount
        public double MontoLetra
        {
            get { return montoLetra; }
            set { montoLetra = value; }
        }

        private string numeroMBW; // codigo del recibo en MBW
        public string NumeroMBW
        {
            get { return numeroMBW; }
            set { numeroMBW = value; }
        }

        private string numeroFisico; // El numero fisico digitado por el cobrador
        public string NumeroFisico
        {
            get { return numeroFisico; }
            set { numeroFisico = value; }
        }

        string codigoCobrador;
        public string CodigoCobrador
        {
            get { return codigoCobrador; }
            set { codigoCobrador = value; }
        }

        private string serie;

        public string Serie
        {
            get { return serie; }
            set { serie = value; }
        }

        private string esanticipo;

        public string EsAnticipo
        {
            get { return esanticipo; }
            set { esanticipo = value; }
        }

        public string IdFP
        {
            get
            {
                return idFP;
            }

            set
            {
                idFP = value;
            }
        }

        private string idFP;

        private string serierete;

        public string SerieRete
        {
            get { return serierete; }
            set { serierete = value; }
        }

    }
}