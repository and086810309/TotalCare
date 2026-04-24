using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class SeccionPagosCheques
    {
        private string fechaVencimiento; // DueDate
        public string FechaVencimiento
        {
            get { return fechaVencimiento; }
            set { fechaVencimiento = value; }
        }

        private int numeroCheques; // CheckNumber
        public int NumeroCheques
        {
            get { return numeroCheques; }
            set { numeroCheques = value; }
        }

        private string codigoBanco; // BankCode
        public string CodigoBanco
        {
            get { return codigoBanco; }
            set { codigoBanco = value; }
        }

        private string numeroCuentaBancaria; // AccountNum
        public string NumeroCuentaBancaria
        {
            get { return numeroCuentaBancaria; }
            set { numeroCuentaBancaria = value; }
        }

        private double sumaValorCheques; //CheckSum
        public double SumaValorCheques
        {
            get { return sumaValorCheques; }
            set { sumaValorCheques = value; }
        }

        private string codigoPais; // CountryCode
        public string CodigoPais
        {
            get { return codigoPais; }
            set { codigoPais = value; }
        }

        private string cuentaCheque; // CheckAccount
        public string CuentaCheque
        {
            get { return cuentaCheque; }
            set { cuentaCheque = value; }
        }

        // nueva coleccion para el caso retenciones
        private string numeroAutorizacion; // Numero de autorizacion del SRI
        public string NumeroAutorizacion
        {
            get { return numeroAutorizacion; }
            set { numeroAutorizacion = value; }
        }

        private List<Retencion> retencionesAsociadas; // Lista de retenciones asociadas al documento
        public List<Retencion> RetencionesAsociadas
        {
            get { return retencionesAsociadas; }
            set { retencionesAsociadas = value; }
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