using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WSMyDealerSAPv3
{
    public class Clientes
    {


        public static string crearCliente(string xmlDatos)
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
            SAPbobsCOM.Company company = null;

            String codcliente = "";
            int error=0;
            String mensaje = "";

            SAPbobsCOM.BusinessPartners partner = null;

            try
            {
                company = DataBase.conectar();
                if (!DataBase.Respuesta.Exito) throw new Exception("Error al conectarse a la Compañia");

                partner = (SAPbobsCOM.BusinessPartners)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);

                XmlDocument xml = new XmlDocument();
                xml.LoadXml(xmlDatos);

                XmlNodeList mainCliente = xml.GetElementsByTagName("Clientes");
                XmlElement datosCliente = (XmlElement) mainCliente[0];


                codcliente = datosCliente["CodigoCliente"].InnerText;

                if (partner.GetByKey(codcliente)) throw new Exception("El cliente ya existe");

                partner = null;
                partner = (SAPbobsCOM.BusinessPartners)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
                    partner.CardType = SAPbobsCOM.BoCardTypes.cLid;
                    partner.CardCode = codcliente;
                    partner.CardName = datosCliente["Nombres"].InnerText;
                    partner.FederalTaxID = datosCliente["Identificacion"].InnerText;

                    if (datosCliente["NombreComercial"] != null) partner.CardForeignName = datosCliente["NombreComercial"].InnerText;
                    if (datosCliente["Telefono"] != null) partner.Phone1 = datosCliente["Telefono"].InnerText;
                    if (datosCliente["CondicionPago"] != null && !string.Empty.Equals(datosCliente["CondicionPago"].InnerText)) partner.PayTermsGrpCode = Int32.Parse(datosCliente["CondicionPago"].InnerText);
                    if (datosCliente["TelefonoCel"] != null) partner.Cellular = datosCliente["TelefonoCel"].InnerText;
                    if (datosCliente["Telefono2"] != null) partner.Phone2 = datosCliente["Telefono2"].InnerText;
                    if (datosCliente["Email"] != null) partner.EmailAddress = datosCliente["Email"].InnerText;
                    if (datosCliente["GrupoClientes"] != null && !string.Empty.Equals(datosCliente["GrupoClientes"].InnerText)) partner.GroupCode = Int32.Parse(datosCliente["GrupoClientes"].InnerText);
                    //if (datosCliente["Observaciones"] != null) partner.Notes = datosCliente["Observaciones"].InnerText;
                    if (datosCliente["Observaciones"] != null) partner.Notes = datosCliente["Observaciones"].InnerText;
                    if (datosCliente["Parroquia"] != null && !string.Empty.Equals(datosCliente["Parroquia"].InnerText)) partner.Territory = Int32.Parse(datosCliente["Parroquia"].InnerText);
                    if (datosCliente["CodigoVendedor"] != null && !string.Empty.Equals(datosCliente["CodigoVendedor"].InnerText)) partner.SalesPersonCode = Int32.Parse(datosCliente["CodigoVendedor"].InnerText);
                
                    partner.set_Properties(2, SAPbobsCOM.BoYesNoEnum.tYES);
                    partner.set_Properties(3, SAPbobsCOM.BoYesNoEnum.tYES);
                    if (datosCliente["CopiaCedula"] != null && "S".Equals(datosCliente["CopiaCedula"].InnerText)) partner.set_Properties(10, SAPbobsCOM.BoYesNoEnum.tYES);
                    if (datosCliente["SoporteBienes"] != null && "S".Equals(datosCliente["SoporteBienes"].InnerText)) partner.set_Properties(12, SAPbobsCOM.BoYesNoEnum.tYES);
                    if (datosCliente["Pagare"] != null && "S".Equals(datosCliente["Pagare"].InnerText)) partner.set_Properties(13, SAPbobsCOM.BoYesNoEnum.tYES);
                    if (datosCliente["Factura"] != null && "S".Equals(datosCliente["Factura"].InnerText)) partner.set_Properties(14, SAPbobsCOM.BoYesNoEnum.tYES);
                    if (datosCliente["Foto"] != null && "S".Equals(datosCliente["Foto"].InnerText)) partner.set_Properties(15, SAPbobsCOM.BoYesNoEnum.tYES);
                    if (datosCliente["ObligadoContabilidad"] != null && "S".Equals(datosCliente["ObligadoContabilidad"].InnerText)) partner.set_Properties(22, SAPbobsCOM.BoYesNoEnum.tYES);
                    if (datosCliente["LocalPropio"] != null && "S".Equals(datosCliente["LocalPropio"].InnerText)) partner.set_Properties(24, SAPbobsCOM.BoYesNoEnum.tYES);
                    if (datosCliente["AnalisisNegocio"] != null && "S".Equals(datosCliente["AnalisisNegocio"].InnerText)) partner.set_Properties(16, SAPbobsCOM.BoYesNoEnum.tYES);

                    //partner.UserFields.Fields.Item("U_TIPO_ID").Value = "C";
                    //partner.UserFields.Fields.Item("U_TIPO_RUC").Value = "N";
                    //partner.UserFields.Fields.Item("U_TIPO_CONTR").Value = "N";
                    //if (datosCliente["AniosAntiguedad"] != null) partner.UserFields.Fields.Item("U_ExxAntiguiedad").Value = datosCliente["AniosAntiguedad"].InnerText;
                    //if (datosCliente["DiasVisita"] != null) partner.UserFields.Fields.Item("U_ExxDiasVisita").Value = datosCliente["DiasVisita"].InnerText;

                    // 

                partner.Addresses.SetCurrentLine(0);
                    partner.Addresses.AddressType = SAPbobsCOM.BoAddressType.bo_ShipTo;
                    partner.Addresses.AddressName = "SUCURSAL";
                    partner.Addresses.County = "001";
                    partner.Addresses.Street = datosCliente["DireccionEnvio"].InnerText;

                    if (datosCliente["Provincia"] != null) partner.Addresses.State = datosCliente["Provincia"].InnerText;
                    if (datosCliente["Ciudad"] != null) partner.Addresses.City = datosCliente["Ciudad"].InnerText;


                partner.Addresses.Add();
                partner.Addresses.SetCurrentLine(1);
                    partner.Addresses.AddressType = SAPbobsCOM.BoAddressType.bo_BillTo;
                    partner.Addresses.AddressName = "PRINCIPAL";
                    partner.Addresses.County = "001";
                    partner.Addresses.Street = datosCliente["DireccionEstablecimiento"].InnerText;

                    if (datosCliente["Provincia"] != null) partner.Addresses.State = datosCliente["Provincia"].InnerText;
                    if (datosCliente["Ciudad"] != null) partner.Addresses.City = datosCliente["Ciudad"].InnerText;



                error = partner.Add();
                if (error != 0)
                {
                    company.GetLastError(out error, out mensaje);
                    throw new Exception(mensaje + " :: " + codcliente + " :: ");
                }
                // partner = null;

                /*
		            <ObligadoContabilidad>S</ObligadoContabilidad>
		            <LocalPropio>N</LocalPropio>
                 */



                retorno = retorno
                            .Replace("{0}", "true")
                            .Replace("{1}", "")
                            .Replace("{2}", "")
                            .Replace("{3}", "")
                            .Replace("{4}", "")
                            .Replace("{5}", "0")
                            .Replace("{6}", "false")
                            .Replace("{7}", "")
                            .Replace("{8}", "");

            }
            catch (Exception e)
            {
                retorno = retorno
                            .Replace("{0}", "false")
                            .Replace("{1}", "false")
                            .Replace("{2}", "")
                            .Replace("{3}", "")
                            .Replace("{4}", e.Message)
                            .Replace("{5}", codcliente)
                            .Replace("{6}", "true")
                            .Replace("{7}", e.StackTrace)
                            .Replace("{8}", "0");

                logs.grabarLog("CLIENTES", e.Message);
                logs.grabarLog("CLIENTES_DEBUG", e.StackTrace);
            }
            finally
            {
                DataBase.DesconectaDB(company);
            }

            if (partner != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(partner);
            partner = null;

            return retorno;

        }


        public static string actualizarEmail(string codcliente, string email) {

            int error = 0; string mensaje = "";
            SAPbobsCOM.Company company = null;

            SAPbobsCOM.BusinessPartners partner = null;
            try
            {
                company = DataBase.conectar();
                if (!DataBase.Respuesta.Exito)
                    throw new Exception("Error al conectarse a la Compañia");


                partner = (SAPbobsCOM.BusinessPartners)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);

                if (partner.GetByKey(codcliente))
                {
                    partner.EmailAddress = email;


                    error = partner.Update();
                    if (error != 0)
                    {
                        company.GetLastError(out error, out mensaje);
                        throw new Exception(mensaje + " :: " + codcliente);
                    }
                }
                else
                {
                    throw new Exception("No se encontro el cliente especificado: " + codcliente);
                }

                mensaje = "registro actualizado satisfactoriamente";
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                logs.grabarLog("CLIENTES", e.Message);
                logs.grabarLog("CLIENTES_DEBUG", e.StackTrace);
            }
            finally
            {
                DataBase.DesconectaDB(company);
            }

            if (partner != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(partner);
            partner = null;

            return mensaje;

        }
    }
}