using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3 // md_direccioncliente
{
    public class MBWDireccionCliente
    {
        string coddireccion;

        public string Coddireccion
        {
            get { return coddireccion; }
            set { coddireccion = value; }
        }
        string codcliente;

        public string Codcliente
        {
            get { return codcliente; }
            set { codcliente = value; }
        }
        string direccion;

        public string Direccion
        {
            get { return direccion; }
            set { direccion = value; }
        }
        string ciudad;

        public string Ciudad
        {
            get { return ciudad; }
            set { ciudad = value; }
        }
        string pais;

        public string Pais
        {
            get { return pais; }
            set { pais = value; }
        }
        string estado;

        public string Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        string codAddress;

        public string CodAddress
        {
            get { return codAddress; }
            set { codAddress = value; }
        }

        int orden;

        public int Orden
        {
            get { return orden; }
            set { orden = value; }
        }

        string cliente;

        public string Cliente
        {
            get { return cliente; }
            set { cliente = value; }
        }
    }
}