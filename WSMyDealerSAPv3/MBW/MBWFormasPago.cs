using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class MBWFormasPago
    {
        // select GroupNum, PymntGroup, ExtraMonth, ExtraDays from OCTG
        int codformapago;

        public int Codformapago
        {
            get { return codformapago; }
            set { codformapago = value; }
        }
        string descripcion;

        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }
        int meses;

        public int Meses
        {
            get { return meses; }
            set { meses = value; }
        }
        int dias;

        public int Dias
        {
            get { return dias; }
            set { dias = value; }
        }
        double descuento;

        public double Descuento
        {
            get { return descuento; }
            set { descuento = value; }
        }
        string estado;

        public string Estado
        {
            get { return estado; }
            set { estado = value; }
        }
    }
}