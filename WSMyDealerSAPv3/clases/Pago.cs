using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class Pago
    {
        //public SeccionPagosEfectivo pagosConEfectivo;
        private string tipoPago;
        public string TipoPago
        {
            get { return tipoPago; }
            set { tipoPago = value; }
        }
        
        
        private SeccionPagosCheques pagosConCheques;
        public SeccionPagosCheques PagosConCheques
        {
            get { return pagosConCheques; }
            set { pagosConCheques = value; }
        }
        
        
        private SeccionPagosLetrasCambio pagosConLetras;
        public SeccionPagosLetrasCambio PagosConLetras
        {
            get { return pagosConLetras; }
            set { pagosConLetras = value; }
        }


        private SeccionPagosTarjeta pagosConTarjeta;
        public SeccionPagosTarjeta PagosConTarjeta
        {
            get { return pagosConTarjeta; }
            set { pagosConTarjeta = value; }
        }
    }
}