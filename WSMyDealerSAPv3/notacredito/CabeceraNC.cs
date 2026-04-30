using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3.notacredito
{
    public class CabeceraNC
    {
        public DateTime DocDate { get; set; }
        public string CardCode { get; set; }
        public string Comments { get; set; }
        public string DocumentoBase { get; set; }
        public int SalesPersonCode { get; set; }
        public int Series { get; set; }
        public string IdDevolucion { get; set; }
        public string folionum { get; set; }
        public string foliopref { get; set; }
        public string puntoemision { get; set; }
        public string costo_region { get; set; }
        public string costo_area { get; set; }
        public string costo_departamento { get; set; }
    }
}