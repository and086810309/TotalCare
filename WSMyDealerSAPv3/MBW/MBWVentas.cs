using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class MBWVentas
    {
        int anio;

        public int Anio
        {
            get { return anio; }
            set { anio = value; }
        }
        int mes;

        public int Mes
        {
            get { return mes; }
            set { mes = value; }
        }
        string codzona;

        public string Codzona
        {
            get { return codzona; }
            set { codzona = value; }
        }
        string codproducto;

        public string Codproducto
        {
            get { return codproducto; }
            set { codproducto = value; }
        }
        double kilos;

        public double Kilos
        {
            get { return kilos; }
            set { kilos = value; }
        }
    }
}