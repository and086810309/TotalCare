using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3
{
    public class RespuestaExistenciaPedido
    {
        private bool existePedido;
        public bool ExistePedido {
            get { return existePedido; }
            set { existePedido = value; }
        }

        private string numeroPedidoSAP;
        public string NumeroPedidoSAP {
            get { return numeroPedidoSAP; }
            set { numeroPedidoSAP = value; }
        }

        private string numeroPedidoMyDealer;
        public string NumeroPedidoMyDealer {
            get { return numeroPedidoMyDealer; }
            set { numeroPedidoMyDealer = value; }
        }

        private string detalleError;
        public string DetalleError {
            get { return detalleError; }
            set { detalleError = value; }
        }

        private string tipo;
        public string Tipo {
            get { return tipo; }
            set { tipo = value; }
        }

        private string estado;
        public string Estado
        {
            get { return estado; }
            set { estado = value; }
        }
    }
}