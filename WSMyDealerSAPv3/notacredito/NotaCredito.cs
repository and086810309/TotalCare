using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMyDealerSAPv3.notacredito
{
    public class NotaCredito
    {
        public static RespuestaNC crearNotaCredito(CabeceraNC cabecera, DetalleNC[] detalles)
        {
            RespuestaNC respuesta = new RespuestaNC();
            respuesta.Estado = 0;
            respuesta.Mensaje = "";
            respuesta.NumeroDocumento = "";

            SAPbobsCOM.Company company = null;

            company = DataBase.conectar();


            if (!DataBase.Respuesta.Exito)
            {
                respuesta.Mensaje = "Error al conectar a la empresa";
                respuesta.NumeroDocumento = "";
            }

            logs.grabarLog("NotaCredito", "Procesando solicitud: " + cabecera.IdDevolucion);

            SAPbobsCOM.Documents oDoc;

            try
            {

                oDoc = (SAPbobsCOM.Documents)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts); //oDrafts
                oDoc.DocObjectCode = SAPbobsCOM.BoObjectTypes.oCreditNotes;

                oDoc.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items;
                oDoc.DocDate = cabecera.DocDate;
                oDoc.CardCode = cabecera.CardCode;
                oDoc.Comments = cabecera.Comments;

                oDoc.SalesPersonCode = cabecera.SalesPersonCode;
                oDoc.Series = cabecera.Series;
                oDoc.DocCurrency = "USD";

                if (true)
                {
                    oDoc.UserFields.Fields.Item("U_num_soldev").Value = cabecera.IdDevolucion;
                    oDoc.UserFields.Fields.Item("U_BPP_MDCO").Value = cabecera.folionum;
                    oDoc.UserFields.Fields.Item("U_BPP_MDSD").Value = cabecera.puntoemision;
                    oDoc.UserFields.Fields.Item("U_BPP_MDSO").Value = cabecera.foliopref;
                }

                //if (DatosEnlace.empresa == "LLP")
                //{
                //    oDoc.UserFields.Fields.Item("U_num_soldev").Value = int.Parse(cabecera.IdDevolucion);
                //}

                int linea = 0;
                foreach (DetalleNC detalle in detalles)
                {
                    if (linea > 0) oDoc.Lines.Add();
                    oDoc.Lines.SetCurrentLine(linea);

                    if (cabecera.DocumentoBase == "SI")
                    {
                        oDoc.Lines.BaseEntry = detalle.BaseEntry;
                        oDoc.Lines.BaseLine = detalle.BaseLine;
                        oDoc.Lines.BaseType = 13;
                    }

                    oDoc.Lines.ItemCode = detalle.ItemCode;
                    oDoc.Lines.Quantity = detalle.Quantity;
                    oDoc.Lines.UnitPrice = detalle.UnitPrice;
                    oDoc.Lines.DiscountPercent = detalle.DiscountPercent;
                    oDoc.Lines.LineTotal = detalle.LineTotal;
                    oDoc.Lines.VatGroup = detalle.VatGroup;
                    oDoc.Lines.WarehouseCode = detalle.WarehouseCode;

                    oDoc.Lines.COGSCostingCode = cabecera.costo_region;
                    oDoc.Lines.CostingCode = cabecera.costo_region;
                    oDoc.Lines.CostingCode2 = cabecera.costo_area;
                    oDoc.Lines.CostingCode3 = cabecera.costo_departamento;

                    linea++;
                }

                int error_documento = oDoc.Add();

                if (error_documento != 0)
                {
                    int error = 0;
                    string mensaje = "";

                    company.GetLastError(out error, out mensaje);

                    logs.grabarLog("NotaCredito", error + " : " + mensaje);

                    respuesta.Mensaje = error + " : " + mensaje;
                }
                else
                {
                    string NumeroDocumento = company.GetNewObjectKey();

                    logs.grabarLog("NotaCredito", "Exito: " + NumeroDocumento);

                    respuesta.Estado = 1;
                    respuesta.Mensaje = "Exito";
                    respuesta.NumeroDocumento = NumeroDocumento;
                }

            }
            catch (Exception e)
            {
                respuesta.Estado = 0;
                respuesta.Mensaje = e.Message;
                respuesta.NumeroDocumento = "";

                logs.grabarLog("NotaCredito", e.Message);
                logs.grabarLog("NotaCredito_DEBUG", e.StackTrace);
            }

            oDoc = null;

            return respuesta;
        }
    }
}