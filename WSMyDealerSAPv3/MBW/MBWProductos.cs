using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class MBWProductos
    {
        string codproducto;

        public string Codproducto
        {
            get { return codproducto; }
            set { codproducto = value; }
        }
        string nombre;

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
        string unidadmedida;

        public string Unidadmedida
        {
            get { return unidadmedida; }
            set { unidadmedida = value; }
        }
        double peso;

        public double Peso
        {
            get { return peso; }
            set { peso = value; }
        }
        string vendible;

        public string Vendible
        {
            get { return vendible; }
            set { vendible = value; }
        }
        string pagaimpuesto;

        public string Pagaimpuesto
        {
            get { return pagaimpuesto; }
            set { pagaimpuesto = value; }
        }
        int codtipoproducto;

        public int Codtipoproducto
        {
            get { return codtipoproducto; }
            set { codtipoproducto = value; }
        }

        string codalterno;

        public string Codalterno
        {
            get { return codalterno; }
            set { codalterno = value; }
        }
        double costo;

        public double Costo
        {
            get { return costo; }
            set { costo = value; }
        }
        double stock;

        public double Stock
        {
            get { return stock; }
            set { stock = value; }
        }
        double precio;

        public double Precio
        {
            get { return precio; }
            set { precio = value; }
        }

        int umv;

        public int UMV
        {
            get { return umv; }
            set { umv = value; }
        }

        string estado;

        public string Estado
        {
            get { return estado; }
            set { estado= value; }
        }

        double cantReservada;
        public double CantReservada
        {
            get { return cantReservada; }
            set { cantReservada = value; }
        }



        int undEntry;
        public int UndEntry
        {
            get { return undEntry; }
            set { undEntry = value; }
        }

        string undCode;
        public string UndCode
        {
            get { return undCode; }
            set { undCode= value; }
        }



        string noDiscount;
        public string NoDiscount
        {
            get { return noDiscount; }
            set { noDiscount = value; }
        }

        string lote;
        public string Lote
        {
            get { return lote; }
            set { lote = value; }
        }
    }
}