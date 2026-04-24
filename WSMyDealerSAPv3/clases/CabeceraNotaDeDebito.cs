using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class CabeceraNotaDeDebito
    {
        public string NumeroNotaDeDebito { get; set; }
        public int Series { get; set; }
        public string FechaGeneracionNotaDeDebito { get; set; }
        public string FechaVencimientoNotaDeDebito { get; set; }
        public string CodigoCliente { get; set; }
        public string Observaciones { get; set; }
        public string CodigoRecibo { get; set; }
        public double ValorTotal { get; set; }
        // Agrega cualquier otro campo necesario para la cabecera de la nota de débito

        public CabeceraNotaDeDebito() { }
        //public CabeceraNotaDeDebito(string numeroNotaDeDebito, int series, string fechaGeneracion, string fechaVencimiento, string codigoCliente, string observaciones, string codigoRecibo, double valorTotal)
        //{
        //    NumeroNotaDeDebito = numeroNotaDeDebito;
        //    Series = series;
        //    FechaGeneracionNotaDeDebito = fechaGeneracion;
        //    FechaVencimientoNotaDeDebito = fechaVencimiento;
        //    CodigoCliente = codigoCliente;
        //    Observaciones = observaciones;
        //    CodigoRecibo = codigoRecibo;
        //    ValorTotal = valorTotal;
        //}
    }
}