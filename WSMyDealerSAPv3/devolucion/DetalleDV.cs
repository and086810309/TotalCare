using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3.devolucion
{
    public class DetalleDV
    {
        public int iddevdet { get; set; }
        public int iddevcab { get; set; }
        public int docentry { get; set; }
        public int linenum { get; set; }
        public string itemcode { get; set; }
        public string dscription { get; set; }
        public string numeroparte { get; set; }
        public string whscode { get; set; }
        public int quantity { get; set; }
        public int quantity_sap { get; set; }
        public double price { get; set; }
        public double pricebefdi { get; set; }
        public double discprcnt { get; set; }
        public double linesubtotal { get; set; }
        public int vatprcnt { get; set; }
        public double linevat { get; set; }
        public double linetotal { get; set; }
        public string tiene_img { get; set; }
        public string nombre_img { get; set; }
        public string tiene_doc { get; set; }
        public string nombre_doc { get; set; }
        public DateTime fecha_creacion { get; set; }
        public int keyorganizacion { get; set; }
    }
}