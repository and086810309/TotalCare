using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3.notacredito
{
    public class DetalleNC
    {
        public int BaseEntry { get; set; }
        public int BaseLine { get; set; }
        public string ItemCode { get; set; }
        public double Quantity { get; set; }
        public double LineTotal { get; set; }
        public double UnitPrice { get; set; }
        public double DiscountPercent { get; set; }
        public string VatGroup { get; set; }
        public string WarehouseCode { get; set; }
    }
}