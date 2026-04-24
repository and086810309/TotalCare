using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class SeccionPagosTarjeta
    {
        private int tarjetaCredito; // CreditCard - CODRETENCION
        public int TarjetaCredito
        {
            get { return tarjetaCredito; }
            set { tarjetaCredito = value; }
        }


        private string cuentaTarjetaCredito; // CreditAcct - CUENTA DE LA RETENCION
        public string CuentaTarjetaCredito
        {
            get { return cuentaTarjetaCredito; }
            set { cuentaTarjetaCredito = value; }
        }

        private string lote; // lote
        public string Lote
        {
            get { return lote; }
            set { lote = value; }
        }

        private string tipo_tarjeta; // Si es Debito  o Credito 
        public string Tipo_tarjeta
        {
            get { return tipo_tarjeta; }
            set { tipo_tarjeta = value; }
        }


        private string numeroTarjeta; // CreditCardNumber - folioNum - NUMERO DE LA TARJETA
        public string NumeroTarjeta
        {
            get { return numeroTarjeta; }
            set { numeroTarjeta = value; }
        }


        private string numeroReclamoFactura; // VoucherNum - numero aleatorio - NUMERO DE FACTURA RELACIONANADA
        public string NumeroReclamoFactura
        {
            get { return numeroReclamoFactura; }
            set { numeroReclamoFactura = value; }
        }


        private string numeroAutorizacion; // OwnerIdNum - numeroDocumento - NUMERO DE AUTORIZACION
        public string NumeroAutorizacion
        {
            get { return numeroAutorizacion; }
            set { numeroAutorizacion = value; }
        }


        private double sumaCredito; // CreditSum - IMPORTE VENCIDO
        public double SumaCredito
        {
            get { return sumaCredito; }
            set { sumaCredito = value; }
        }


        private String fechaVencAutorizacion; //    - FECHA DE VENCIMIENTO DE AUTORIZACION
        public String FechaVencAutorizacion
        {
            get { return fechaVencAutorizacion; }
            set { fechaVencAutorizacion = value; }
        }


        private String tipoRetencion; // CrTypeCode - TIPO DE RETENCION, VALOR
        public String TipoRetencion
        {
            get { return tipoRetencion; }
            set { tipoRetencion = value; }
        }


        private String setieRetencion; //  OwnerPhone -  SERIE DE RETENCION
        public String SetieRetencion
        {
            get { return setieRetencion; }
            set { setieRetencion = value; }
        }


        private int cantidadPagos; // umOfPmnts -  CANTIDAD DE PAGOS
        public int CantidadPagos
        {
            get { return cantidadPagos; }
            set { cantidadPagos = value; }
        }


        private double baseImponible; // u_monto_base - BASE IMPONIBLE
        public double BaseImponible
        {
            get { return baseImponible; }
            set { baseImponible = value; }
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
    }
}