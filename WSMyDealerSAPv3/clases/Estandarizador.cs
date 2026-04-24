using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace WSMyDealerSAPv3
{
    public class Estandarizador
    {
        /**
         * Envuelve un paquete de peticion soap para entidades desconectadas (exclusivo para login y logout)
         * @param cuerpoXML El cuerpo del xml a enviar
         * @return El xml envuelto en el contenedor SOAP
         */
        public static string envolverPeticion(string cuerpoXML)
        {
            string envio = "<?xml version='1.0' encoding='UTF-16'?><env:Envelope xmlns:env='http://schemas.xmlsoap.org/soap/envelope/'><env:Body>" + cuerpoXML + "</env:Body></env:Envelope>";
            return envio;
        }

        /**
         * Envuelve un paquete soap para entidades conectadas (luego de login)
         * @param cuerpoXML El cuerpo del xml a enviar
         * @param sessionId El id de sesion de SAP (mandatorio)
         * @return El xml envuelto en el contenedor SOAP
         */
        public static string envolverPeticionConectada(string cuerpoXML, string sessionId)
        {
            string envio = "<?xml version='1.0' encoding='UTF-16'?><env:Envelope xmlns:env='http://schemas.xmlsoap.org/soap/envelope/'><env:Header><SessionID>" + sessionId + "</SessionID></env:Header><env:Body>" + cuerpoXML + "</env:Body></env:Envelope>";
            return envio;
        }

        /**
         * Permite normalizar una fecha en el formato de salida indicado
         * @param fecha La fecha de ingreso (como cadena)
         * @param formatoIngreso El formato de ingreso (por defecto dd/MM/yyyy)
         * @param formatoSalida El formato de salida (por defecto yyyyMMdd)
         */
        public static string normalizarFecha(string fecha, string formatoIngreso, string formatoSalida)
        {
            //formatoIngreso = (formatoIngreso != null && !formatoIngreso.Replace(" ", "").Equals("")) ? formatoIngreso : "yyyy-MM-dd";
            //formatoSalida = (formatoSalida != null && !formatoSalida.Replace(" ", "").Equals("")) ? formatoSalida : "yyyyMMdd";

            formatoIngreso = (!Estandarizador.estaVacia(formatoIngreso)) ? formatoIngreso : "yyyy-MM-dd";
            formatoSalida = (!Estandarizador.estaVacia(formatoSalida)) ? formatoSalida : "yyyyMMdd";

            DateTime fechaIngreso = DateTime.ParseExact(fecha, formatoIngreso, null);
            return fechaIngreso.ToString(formatoSalida);
        }

        /**
         * Determina si una cadena es nula o en blanco
         * @param cadena La cadena a evaluar
         * @return Si la cadena esta vacia o nula
         */
        public static bool estaVacia(string cadena)
        {
            return (cadena == null || cadena.Replace(" ", "").Equals(""));
        }

        /**
         * Estandariza una cadena para su ingreso en un bloque XML
         * @param cadena La cadena a estandarizar
         * @return La cadena estandarizada
         */
        public static string estandarizarCadena(string cadena)
        {
            string salida = (!Estandarizador.estaVacia(cadena)) ? cadena : "";

            return "<![CDATA[" + salida + "]]>";
        }

        /**
         * Estandariza un numero para su ingreso a SAP como cadena
         * @param numero El numero a estandarizar (segun los parametros generales)
         * @return El numero estandarizado
         */
        public static Double obtenerNumeroConDecimales(double numero)
        {
            string simboloActual = (ConfigurationManager.AppSettings["separadorDecimal"].Equals("P")) ? "," : ".";
            string simboloCambio = (ConfigurationManager.AppSettings["separadorDecimal"].Equals("P")) ? "." : ",";
            string salida = numero.ToString().Replace(simboloActual, simboloCambio);

            return Double.Parse(salida);
        }

        public static Respuesta procesarRespuesta(List<String> salida) {
            Respuesta respuesta = new Respuesta();

            if (salida[0].Equals("Success"))
            {
                respuesta.Exito = true;
                respuesta.CodigoRespuesta = salida[1];
                respuesta.EntradaRAW = salida[2];
            }
            else
            {
                respuesta.Exito = false;
                respuesta.CodigoError = salida[1];
                respuesta.CodigoRespuesta = salida[2];
                respuesta.DescripcionError = salida[3];
            }

            return respuesta;
        }
    }
}