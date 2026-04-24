using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class CabeceraFactura
    {
        private string facturaPtoEmi;
        public string FacturaPtoEmi
        {
            get { return facturaPtoEmi; }
            set { facturaPtoEmi = value; }
        }


        private string facturaEstab;
        public string FacturaEstab
        {
            get { return facturaEstab; }
            set { facturaEstab = value; }
        }


        private string facturaNumAutoriza;
        public string FacturaNumAutoriza
        {
            get { return facturaNumAutoriza; }
            set { facturaNumAutoriza = value; }
        }


        private string facturaNumero;
        public string FacturaNumero
        {
            get { return facturaNumero; }
            set { facturaNumero = value; }
        }


        private string facturaTipoComprob;
        public string FacturaTipoComprob
        {
            get { return facturaTipoComprob; }
            set { facturaTipoComprob = value; }
        }


        private DateTime facturaFecha;
        public DateTime FacturaFecha
        {
            get { return facturaFecha; }
            set { facturaFecha = value; }
        }



    }
}