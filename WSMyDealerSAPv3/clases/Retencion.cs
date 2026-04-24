using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class Retencion
    {
        private string docEntry;

        public string DocEntry
        {
            get { return docEntry; }
            set { docEntry = value; }
        }
        private string folio;

        public string Folio
        {
            get { return folio; }
            set { folio = value; }
        }
        private string numeroDocumento;

        public string NumeroDocumento
        {
            get { return numeroDocumento; }
            set { numeroDocumento = value; }
        }
        private string fechaRetencion;

        public string FechaRetencion
        {
            get { return fechaRetencion; }
            set { fechaRetencion = value; }
        }
        private string codigoRetencion; // aqui va el numero de la retencion (de la cabecera)

        public string CodigoRetencion
        {
            get { return codigoRetencion; }
            set { codigoRetencion = value; }
        }
        private string numeroRetencion; // aqui va el codigo del sri

        public string NumeroRetencion
        {
            get { return numeroRetencion; }
            set { numeroRetencion = value; }
        }
        private double baseImponible;

        public double BaseImponible
        {
            get { return baseImponible; }
            set { baseImponible = value; }
        }
        private double porcentajeRetencion;

        public double PorcentajeRetencion
        {
            get { return porcentajeRetencion; }
            set { porcentajeRetencion = value; }
        }
        private string tipoRetencion;

        public string TipoRetencion
        {
            get { return tipoRetencion; }
            set { tipoRetencion = value; }
        }
        private double valorRetencion;

        public double ValorRetencion
        {
            get { return valorRetencion; }
            set { valorRetencion = value; }
        }
        private string bienServicio;

        public string BienServicio
        {
            get { return bienServicio; }
            set { bienServicio = value; }
        }
        private string concepto;

        public string Concepto
        {
            get { return concepto; }
            set { concepto = value; }
        }
    }
}