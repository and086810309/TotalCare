using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Data.Odbc;

namespace WSMyDealerSAPv3
{
    public class General
    {

        public static String obtenerDatosTablaVista(String nombreTablaVista, int inicio, int cantidad)
        {
            String retorno = "<?xml version='1.0' encoding='ISO-8859-1'?>" +
                          "<Respuesta>" +
                              "<Exito>{0}</Exito>" +
                              "<ExistePedido>{1}</ExistePedido>" +
                              "<CodigoError>{2}</CodigoError>" +
                              "<CodigoRespuesta>{3}</CodigoRespuesta>" +
                              "<DescripcionError>{4}</DescripcionError>" +
                              "<NumeroPedidoSAP>{5}</NumeroPedidoSAP>" +
                              "<ErrorConexion>{6}</ErrorConexion>" +
                              "<Registros>{7}</Registros>" +
                              "<MaxRegistros>{8}</MaxRegistros>" +
                          "</Respuesta>";

            string[] words = nombreTablaVista.Split('|');
            String miWhere = "";
            String miOrder = "";
            if (words.Count() > 1)
            {
                miWhere = words[1];
                nombreTablaVista = words[0];
            }
            if (words.Count() > 2)
            {
                miOrder = words[2];
            }
            DBSqlServer.ConectaDB();
            if (!DBSqlServer.Respuesta.Exito)
            {
                retorno = retorno
                            .Replace("{0}", "false")
                            .Replace("{1}", "false")
                            .Replace("{2}", DBSqlServer.Respuesta.CodigoError)
                            .Replace("{3}", DBSqlServer.Respuesta.CodigoRespuesta)
                            .Replace("{4}", DBSqlServer.Respuesta.DescripcionError)
                            .Replace("{5}", "0")
                            .Replace("{6}", "true")
                            .Replace("{7}", "")
                            .Replace("{8}", "0");
            }
            else
            {
                try
                {
                    String sql = "";
                    /*LQ 2023-02-27 carga en tabla de paso los registos de la vista */
                    if (nombreTablaVista.Equals("MD_PASO") && inicio == 0)
                    {
                        sql = "drop table \"MD_PASO\"";
                        OdbcCommand com1 = new OdbcCommand(sql, DBSqlServer.Conexion);
                        try
                        {
                            com1.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            logs.grabarLog(nombreTablaVista.ToUpper() + "_LQ", ex.Message);
                        }
                        sql = "create table \"MD_PASO\" as  (select * from \"MD_CTASXCOBRAR\" )";
                        OdbcCommand com2 = new OdbcCommand(sql, DBSqlServer.Conexion);
                        try
                        {
                            com2.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            logs.grabarLog(nombreTablaVista.ToUpper() + "_LQ", ex.Message);
                        }

                    }


                    /*fin carta tabla paso*/
                    /*
                     *  OBTENEMOS LA CANTIDAD DE REGISTROS EN LA TABLA
                     */
                     sql = " select count(*) \"cantidad\" from \"" + nombreTablaVista + "\"  " + miWhere;

                    OdbcCommand com = new OdbcCommand(sql, DBSqlServer.Conexion);
                    com.CommandType = CommandType.Text;

                    OdbcDataReader record = com.ExecuteReader();
                    int cantidadRegistros = 0;
                    if (record.HasRows)
                    {
                        if (record.Read())
                        {
                            if (record.GetValue(0) != null && record.GetValue(0).ToString() != "")
                                cantidadRegistros = Int32.Parse(record.GetValue(0).ToString());
                        }
                    }
                    sql = "";
                    record.Close(); record.Dispose(); record = null;
                    com.Dispose(); com = null;


                    /*
                     * OBTENEMOS LOS REGISTROS DE LA TABLA
                     */
                    String auxXML = "";
                    sql = " select * from " + nombreTablaVista + " " + miWhere + " " + miOrder;
                    if (cantidad > 0)
                        //sql += " where \"indice\" between " + inicio + " and " + (inicio+cantidad-1);
                        sql += " LIMIT " + cantidad + " OFFSET " + inicio + "";
                    com = new OdbcCommand(sql, DBSqlServer.Conexion);
                    com.CommandType = CommandType.Text;
                    com.CommandTimeout = 120;

                    record = com.ExecuteReader();
                    if (record.HasRows)
                    {
                        String[] arrColumsn = null;
                        String[] arrTypes = null;

                        while (record.Read())
                        {
                            auxXML += "<Registro>";
                            int max = record.FieldCount;
                            if (arrColumsn == null) arrColumsn = new String[max];
                            if (arrTypes == null) arrTypes = new String[max];


                            for (int i = 0; i < max; i++)
                            {
                                if (arrColumsn[i] == null || "".Equals(arrColumsn[i])) arrColumsn[i] = record.GetName(i).ToLower();
                                if (arrTypes[i] == null || "".Equals(arrTypes[i])) arrTypes[i] = record.GetDataTypeName(i).ToLower();

                                auxXML += "<" + arrColumsn[i] + "><![CDATA[";
                                switch (arrTypes[i])
                                {
                                    case "int":
                                    case "bigint":
                                    case "smallint":
                                    case "numeric":
                                    case "decimal":
                                    case "float":
                                        // string s = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
                                        if (record.GetValue(i) != null && record.GetValue(i).ToString() != "")
                                        {
                                            if (record.GetValue(i).ToString().Contains(","))
                                                auxXML += record.GetValue(i).ToString().Replace(".", "").Replace(",", ".");
                                            else
                                                auxXML += record.GetValue(i).ToString();
                                        }
                                        break;

                                    default:
                                        auxXML += record.GetValue(i);
                                        break;
                                }
                                // logs.grabarLog(nombreTablaVista.ToUpper() + "_DATA", i + " - " + arrColumsn[i] + ": " + record.GetValue(i));
                                auxXML += "]]></" + arrColumsn[i] + ">";
                            }
                            auxXML += "</Registro>";
                        }
                    }
                    sql = "";
                    record.Close(); record.Dispose(); record = null;
                    com.Dispose(); com = null;

                    retorno = retorno
                                .Replace("{0}", "true")
                                .Replace("{1}", "")
                                .Replace("{2}", "")
                                .Replace("{3}", "")
                                .Replace("{4}", "")
                                .Replace("{5}", "0")
                                .Replace("{6}", "false")
                                .Replace("{7}", auxXML)
                                .Replace("{8}", cantidadRegistros.ToString());

                    // logs.grabarLog(nombreTablaVista.ToUpper() + "_XML", retorno);

                }
                catch (Exception e)
                {
                    retorno = retorno
                                .Replace("{0}", "false")
                                .Replace("{1}", "false")
                                .Replace("{2}", "")
                                .Replace("{3}", "")
                                .Replace("{4}", e.Message)
                                .Replace("{5}", "0")
                                .Replace("{6}", "true")
                                .Replace("{7}", e.StackTrace)
                                .Replace("{8}", "0");

                    logs.grabarLog(nombreTablaVista.ToUpper() + "", e.Message);
                    logs.grabarLog(nombreTablaVista.ToUpper() + "_DEBUG", e.StackTrace);
                }
                finally
                {
                    DBSqlServer.DesconectaDB();
                }


            }

            return retorno;
        }
    }
}
