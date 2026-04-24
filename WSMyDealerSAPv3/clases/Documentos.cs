using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class Documentos
    {

        private string codcliente; // cardcode

        public string Codcliente
        {
            get { return codcliente; }
            set { codcliente = value; }
        }
        private string cliente; // nombre (cliente)

        public string Cliente
        {
            get { return cliente; }
            set { cliente = value; }
        }
        //public string tipoDocumento; // tipodocumento - InvoiceType
        private string codDocumento; // docentry

        public string CodDocumento
        {
            get { return codDocumento; }
            set { codDocumento = value; }
        }
        private string numDocumento; // documento - DocEntry

        public string NumDocumento
        {
            get { return numDocumento; }
            set { numDocumento = value; }
        }
        private string folioDocumento;

        public string FolioDocumento
        {
            get { return folioDocumento; }
            set { folioDocumento = value; }
        }
        //public string codigoCuotaSAP; // codigo real de la cuota sap - Line_Id
        //public int cuotaDocumento; //cuota_operacion
        private double total; // valor - SumApplied y PaidSum

        public double Total
        {
            get { return total; }
            set { total = value; }
        }
        private double pagado;

        public double Pagado
        {
            get { return pagado; }
            set { pagado = value; }
        }
        private double saldo; // saldo

        public double Saldo
        {
            get { return saldo; }
            set { saldo = value; }
        }
        private string fecha;

        public string Fecha
        {
            get { return fecha; }
            set { fecha = value; }
        }
        private string vfecha; // fechavencimiento

        public string Vfecha
        {
            get { return vfecha; }
            set { vfecha = value; }
        }

        string numpedido;

        public string Numpedido
        {
            get { return numpedido; }
            set { numpedido = value; }
        }

        string tipoDocumento;

        public string TipoDocumento
        {
            get { return tipoDocumento; }
            set { tipoDocumento = value; }
        }

        double impuesto, subtotal;

        public double Subtotal
        {
            get { return subtotal; }
            set { subtotal = value; }
        }

        public double Impuesto
        {
            get { return impuesto; }
            set { impuesto = value; }
        }
    }
}