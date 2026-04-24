using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace WSMyDealerSAPv3
{
    public class logs
    {
        private static string path_logs = AppDomain.CurrentDomain.BaseDirectory + "logs";
        private static string ext = ".bts";

        // FX. PARA GRABAR LOGS DE SUCESOS
        public static void grabarLog(string tipo, string datos)
        {
            try
            {
                logs.verificarRuta(path_logs);

                StreamWriter log = new StreamWriter(path_logs + "/" + tipo + logs.ext, true);
                String cadena = "";

                cadena += DateTime.Now.ToString("dd-MM-yyyy hh:mm tt") + "\t >>> \t";
                cadena += datos;

                log.WriteLine(cadena);
                log.Close();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("ERROR LOG: " + e.Message);
            }

        }

        // FX. PARA VERIFICAR SI MI RUTA EXISTE, SINO LA CREA
        public static void verificarRuta(String path)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("ERROR LOG: " + e.Message);
            }
        }
    }
}