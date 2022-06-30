using vozy_automatizacion.Models;
namespace vozy_automatizacion.Controllers
{
  public static class Transform
  {
    public static JsonData AsignarDetalle(JsonData json)
    {
      if (json.minutos == "0")
      {
        return FueraDeServicio(json);
      }
      else
      {
        switch (json.contactabilidad)
        {
          case "Avaliable_no":
            return DevolucionDeLlamada(json);
          case "no_disponible":
            return DevolucionDeLlamada(json);

          case "buzon_voz":
            return FueraDeServicio(json);

          case "Family_member_response":
            return DevolucionDeLlamada(json);

          case "Family_response":
            return DevolucionDeLlamada(json);
          case "False":

            if (json.conoce_al_contacto == "True")
            {
              return DevolucionDeLlamada(json);
            }
            else
            {
              return NumeroEquivocado(json);
            }
          case "FALSO":
            if (json.conoce_al_contacto == "VERDADERO")
            {
              return DevolucionDeLlamada(json);
            }
            else
            {
              return NumeroEquivocado(json);
            }
          case "True":
            switch (json.promesa_de_pago)
            {
              case "Offer_partial_payment":
                return PagoParcial(json);
              case "Payment_done":
                return ConfirmarPago(json);
              case "True":
                return PagoTotal(json);
              case "False":
                return DevolucionDeLlamada(json);
              default:
                return DevolucionDeLlamada(json);
            }
          case "VERDADERO":
            switch (json.promesa_de_pago)
            {
              case "Offer_partial_payment":
                return PagoParcial(json);
              case "Payment_done":
                return ConfirmarPago(json);
              case "VERDADERO":
                return PagoTotal(json);
              case "FALSO":
                return DevolucionDeLlamada(json);
              default:
                return DevolucionDeLlamada(json);
            }
          default:
            return ContestaYCuelga(json);
        }
      }
    }
    private static JsonData FueraDeServicio(JsonData json)
    {
      json.resultado = "No Contacto";
      json.detalle = "Fuera de Servicio / buzon de Voz";
      json.situacion = "No Contacto";
      return json;
    }
    private static JsonData ContestaYCuelga(JsonData json)
    {
      json.resultado = "No Contacto";
      json.detalle = "Contestan Y Cuelgan";
      json.situacion = "No Contacto";
      return json;
    }
    private static JsonData DevolucionDeLlamada(JsonData json)
    {
      json.resultado = "Localizado";
      json.detalle = "Devolucion de Llamada";
      json.situacion = json.contactabilidad == "Family_member_response" ? "Familiar" : "Tercero";
      return json;
    }
    private static JsonData PagoParcial(JsonData json)
    {
      json.resultado = "Localizado";
      json.detalle = "En Negociacion";
      json.situacion = "Deudor";
      return json;
    }
    private static JsonData PagoTotal(JsonData json)
    {
      json.resultado = "Localizado";
      json.detalle = "En Negociacion";
      json.situacion = "Deudor";
      return json;
    }
    private static JsonData ConfirmarPago(JsonData json)
    {
      json.resultado = "Confirmar Pago";
      json.detalle = "Pago en Confirmacion";
      json.situacion = "Titular";
      return json;
    }
    private static JsonData NumeroEquivocado(JsonData json)
    {
      json.resultado = "No Contacto";
      json.detalle = "Numero Equivocado";
      json.situacion = "No Contacto";
      return json;
    }
    public static JsonData observacionesGestion(JsonData json)
    {
      json.identificacion = json.identificacion.Substring(1, json.identificacion.Length - 2);
      if (json.contactabilidad == "True")
      {
        json.gestion = $"{json.promesa_de_pago_horario} {json.fecha_pagada} {json.promesa_de_pago_2} {json.razon_de_no_pago} {json.acuerdo} {json.telefono_acuerdo_de_pago_dado}";
      }
      else
      {
        json.gestion = $"{json.promesa_de_pago} {json.promesa_de_pago_horario} {json.fecha_pagada} {json.promesa_de_pago_2} {json.razon_de_no_pago} {json.acuerdo} {json.telefono_acuerdo_de_pago_dado}";
      }
      switch (json.resultado)
      {
        case "Promesa de Pago":
          json.gestionASubir = $"{json.tel_llamada.Substring(3)} {json.detalle} el {json.fecha} {json.valor_a_pagar} {json.pago_de_intencion} {json.campaign_id}";
          break;
        case "No Contacto":
          json.gestionASubir = $"{json.tel_llamada.Substring(3)} {json.detalle} {json.campaign_id}";
          break;
        case "Confirmar Pago":
          json.gestionASubir = $"{json.tel_llamada.Substring(3)} {json.detalle} {json.situacion} {json.campaign_id}";
          break;
        default:
          json.gestionASubir = $"{json.tel_llamada.Substring(3)} {json.resultado} {json.detalle} {json.situacion} {json.campaign_id}";
          break;
      }
      return json;
    }
  }
}
