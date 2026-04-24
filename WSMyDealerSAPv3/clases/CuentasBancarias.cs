using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class CuentasBancarias
    {
        string codbanco;

        public string Codbanco
        {
            get { return codbanco; }
            set { codbanco = value; }
        }
        string descripcion;

        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }
        string numcuenta;

        public string Numcuenta
        {
            get { return numcuenta; }
            set { numcuenta = value; }
        }
        string GLAccount;

        public string GLAccount1
        {
            get { return GLAccount; }
            set { GLAccount = value; }
        }
        string tipo;

        public string Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

    }
}