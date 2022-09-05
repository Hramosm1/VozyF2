using vozy_automatizacion.Models;

namespace vozy_automatizacion.Services;

public class CollectionsWeb2Service
{
    public CollectionsWeb2 transformData(CollectionsWeb2 body)
    {
        string transaction1 = $"{body.affirmations} {body.denials} (callorign) {body.repeat} {body.unavailable} {body.already_pay} {body.Informacion_de_contacto}";
        string transaction2 = $"{body.Compromiso_de_pago} {body.denials} {body.fecha_de_compromiso} {body.Informacion_de_contacto}";
        body.situacion = "Deudor";
        //SI LA LLAMADA TIENE DURACION MAYOR A 0
        if (body.Duration > 0)
        {
            switch (body.contactability_type)
            {
                //si se logro contactar con la persona
                case "Contacto si":
                    body.gestion = transaction2;
                    switch (body.success_type)
                    {
                        case "confirma pago":
                            body.resultado = "Promesa de pago";
                            body.detalle = "Convenio de pago";
                            break;
                        case "pago parcial":
                            body.resultado = "Promesa de pago";
                            body.detalle = "Pago parcial";
                            break;
                        case "ya pagó":
                            body.resultado = "Confirmar pago";
                            body.detalle = "Realizó pago";
                            break;
                        case "razón de no pago":
                            body.resultado = "Localizado";
                            body.detalle = "Negativa de pago";
                            break;
                        
                        //contactact yes, pago limite
                        default:
                            body.resultado = "Localizado";
                            body.detalle = "En negociación";
                            break;
                    }
                    break;

                //si no se pudo contactar con la persona
                case "Contacto no":
                    body.resultado = "No contacto";
                    body.detalle = "Contestan y cuelga";
                    body.gestion = transaction1;
                    break;

                //todos los demas casos
                default:
                    body.resultado = "Localizado";
                    body.detalle = "Devolucion de llamada";
                    body.situacion = "Tercero";
                    body.gestion = transaction1;
                    break;
            }
        }
        //SI LA LLAMADA NO SE REALIZO 
        else
        {
            body.resultado = "No contacto";
            body.detalle = "Fuera de servicio / Buzón de voz";
        }
        return body;
    }
}