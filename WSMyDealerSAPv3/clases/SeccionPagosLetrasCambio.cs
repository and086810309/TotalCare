using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class SeccionPagosLetrasCambio
    {
        private string numeroLetra; // ReferenceNo
        public string NumeroLetra
        {
            get { return numeroLetra; }
            set { numeroLetra = value; }
        }

        private string idFP; // ReferenceNo
        public string IdFP
        {
            get { return idFP; }
            set { idFP = value; }
        }

        private string codigoBancoAsociado; // BPBankCode
        public string CodigoBancoAsociado
        {
            get { return codigoBancoAsociado; }
            set { codigoBancoAsociado = value; }
        }

        private string cuentaBancariaBancoAsociado; // BPBankAct
        public string CuentaBancariaBancoAsociado
        {
            get { return cuentaBancariaBancoAsociado; }
            set { cuentaBancariaBancoAsociado = value; }
        }

        private string numeroFolio; // FolioNumber
        public string NumeroFolio
        {
            get { return numeroFolio; }
            set { numeroFolio = value; }
        }

        private string prefijoFolio; // FolioPrefixString - Se quema LT
        public string PrefijoFolio
        {
            get { return prefijoFolio; }
            set { prefijoFolio = value; }
        }

        private string fechaVencimientoLetra; // BillOfExchangeDueDate
        public string FechaVencimientoLetra
        {
            get { return fechaVencimientoLetra; }
            set { fechaVencimientoLetra = value; }
        }

        private string codigoPais; // 
        public string CodigoPais
        {
            get { return codigoPais; }
            set { codigoPais = value; }
        }

        private double sumaValorLetra; //
        public double SumaValorLetra
        {
            get { return sumaValorLetra; }
            set { sumaValorLetra = value; }
        }

        private string cuentaLetra; // CheckAccount
        public string CuentaLetra
        {
            get { return cuentaLetra; }
            set { cuentaLetra = value; }
        }

        private string metodoPago; // CheckAccount
        public string MetodoPago
        {
            get { return metodoPago; }
            set { metodoPago = value; }
        }

    }
}