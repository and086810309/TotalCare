using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class MBWClientes
    {
        string codcliente;

        public string Codcliente
        {
            get { return codcliente; }
            set { codcliente = value; }
        }
        string descripcion;

        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }
        int codsubcanal;

        public int Codsubcanal
        {
            get { return codsubcanal; }
            set { codsubcanal = value; }
        }
        int codzona;

        public int Codzona
        {
            get { return codzona; }
            set { codzona = value; }
        }
        string cardtype;

        public string Cardtype
        {
            get { return cardtype; }
            set { cardtype = value; }
        }
        string address;

        public string Address
        {
            get { return address; }
            set { address = value; }
        }
        string phone1;

        public string Phone1
        {
            get { return phone1; }
            set { phone1 = value; }
        }
        string country;

        public string Country
        {
            get { return country; }
            set { country = value; }
        }
        string city;

        public string City
        {
            get { return city; }
            set { city = value; }
        }
        double creditLine;

        public double CreditLine
        {
            get { return creditLine; }
            set { creditLine = value; }
        }

        string autorizacion;

        public string Autorizacion
        {
            get { return autorizacion; }
            set { autorizacion = value; }
        }

        string cedulaRuc;

        public string CedulaRuc
        {
            get { return cedulaRuc; }
            set { cedulaRuc = value; }
        }

        string codtipocliente;      /* codlista  para precios*/

        public string CodTipoCliente
        {
            get { return codtipocliente; }
            set { codtipocliente = value; }
        }

        string estado;

        public string Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        int diastolerancia;

        public int DiasTolerancia
        {
            get { return diastolerancia; }
            set { diastolerancia = value; }
        }


        private String tiene_retencion;
        public String Tiene_retencion
        {
            get { return tiene_retencion; }
            set { tiene_retencion = value; }
        }


        private String email;
        public String Email
        {
            get { return email; }
            set { email = value; }
        }

        private String codvendedor;
        public String Codvendedor
        {
            get { return codvendedor; }
            set { codvendedor = value; }
        }
        /*
				public $addID;*/
    }
}