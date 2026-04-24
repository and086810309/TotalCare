using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class MBWGenCliente
    {
        private string nombreCliente;        //nombre del cliente

        public string NombreCliente
        {
            get { return nombreCliente; }
            set { nombreCliente = value; }
        }
        private string apellido;             //apellido del cliente

        public string Apellido
        {
            get { return apellido; }
            set { apellido = value; }
        }
        private string tipoIdentificacion;     //Tipo de identificacion CI/RUC/PAS

        public string TipoIdentificacion
        {
            get { return tipoIdentificacion; }
            set { tipoIdentificacion = value; }
        }
        private string identificacion;       //Numero de CI/RUC/PAS

        public string Identificacion
        {
            get { return identificacion; }
            set { identificacion = value; }
        }
        private string domicilio;            //direccion del cliente

        public string Domicilio
        {
            get { return domicilio; }
            set { domicilio = value; }
        }
        private string codCiudad;               //codigo de la ciudad del cliente

        public string CodCiudad
        {
            get { return codCiudad; }
            set { codCiudad = value; }
        }
        private string telefono1;             //numero de telefono principal del cliente

        public string Telefono1
        {
            get { return telefono1; }
            set { telefono1 = value; }
        }
        private string personeria;             //indicador de personería: jurídica 'J' o natural 'N' (opcional)

        public string Personeria
        {
            get { return personeria; }
            set { personeria = value; }
        }
        private string esEntidadFinanciera;    //indicador de si la entidad es financiera o no

        public string EsEntidadFinanciera
        {
            get { return esEntidadFinanciera; }
            set { esEntidadFinanciera = value; }
        }
        private string codigoUsuario;        //codigo del usuario creador del registro (default)

        public string CodigoUsuario
        {
            get { return codigoUsuario; }
            set { codigoUsuario = value; }
        }
        private DateTime fecha;              //Fecha de creacion del registro (getdate)

        public DateTime Fecha
        {
            get { return fecha; }
            set { fecha = value; }
        }
        private string codigoCobrador;       // El codigo del cobrador (agente relacionado)

        public string CodigoCobrador
        {
            get { return codigoCobrador; }
            set { codigoCobrador = value; }
        }
        private string contribuyenteEspecial; // Determina si el cliente es o no contribuyente especial

        public string ContribuyenteEspecial
        {
            get { return contribuyenteEspecial; }
            set { contribuyenteEspecial = value; }
        }
    }
}