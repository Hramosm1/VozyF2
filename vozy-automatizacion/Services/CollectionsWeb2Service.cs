using vozy_automatizacion.Models;

namespace vozy_automatizacion.Services;

public class CollectionsWeb2Service
{
    public CollectionsWeb2 transformData(CollectionsWeb2 body)
    {
        //SE ELIMINAN LOS PRIMEROS 3 VALORES DEL NUMERO PARA QUITAR EL 502
        body.phone = body.phone.Substring(3);
        //SE ELIMINA EL VALOR A Y B DE LA IDENTIFICACION EN EL FORMATO {A}XXXXX{B}
        body.identificacion = body.identificacion.Remove(body.identificacion.Length - 1).Remove(0, 1);

        string observacionTipo1 =
            $"{body.affirmations} {body.denials} {body.call_origin} {body.repeat} {body.unavailable} {body.already_pay} {body.Informacion_de_contacto}"
                .Trim();
        string observacionTipo2 =
            $"{body.Compromiso_de_pago} {body.denials} {body.fecha_de_compromiso} {body.Informacion_de_contacto}"
                .Trim();
        body.situacion = "Deudor";
        body.gestion = "";
        //SI LA LLAMADA TIENE DURACION MAYOR A 0
        if (body.Duration > 0)
        {   switch (body.Contacted)
            {
                case true:
                    switch (body.contact_confirmed)
                    {
                        case true:
                            switch (body.success_type)
                            {
                                case "Contact Yes":
                                    body.resultado = "Localizado";
                                    body.detalle = "Devolucion de llamada";
                                    body.situacion = "Tercero";
                                    if (observacionTipo2.Length == 0)
                                    {
                                        body.gestion = body.gestion = "Telefono: " + body.phone + " " + body.detalle;
                                    }
                                    else
                                    {
                                        body.gestion = body.gestion = "Telefono: " + body.phone + " " + observacionTipo2;
                                    }
                                    break;
                                case "Confirma Pago":
                                    body.resultado = "Localizado";
                                    body.detalle = "En negociacion";
                                    if (observacionTipo2.Length == 0)
                                    {
                                        body.gestion = body.gestion = "Telefono: " + body.phone + " " + body.detalle;
                                    }
                                    else
                                    {
                                        body.gestion = body.gestion = "Telefono: " + body.phone + " " + observacionTipo2;
                                    }
                                    break;
                                case "Pago Parcial":
                                    body.resultado = "Localizado";
                                    body.detalle = "En negociacion";
                                    if (observacionTipo2.Length == 0)
                                    {
                                        body.gestion = body.gestion = "Telefono: " + body.phone + " " + body.detalle;
                                    }
                                    else
                                    {
                                        body.gestion = body.gestion = "Telefono: " + body.phone + " " + observacionTipo2;
                                    }
                                    break;
                                case "Ya pagó":
                                    body.resultado = "Confirmar pago";
                                    body.detalle = "Realizo pago";
                                    if (observacionTipo2.Length == 0)
                                    {
                                        body.gestion = body.gestion = "Telefono: " + body.phone + " " + body.detalle;
                                    }
                                    else
                                    {
                                        body.gestion = body.gestion = "Telefono: " + body.phone + " " + observacionTipo2;
                                    }
                                    break;
                            }
                            break;
                        case false:
                            switch (body.contactability_type)
                            {
                                case "Contacto conocido" or "Contacto familiar" or "Contacto no disponible" or "Contacto rellamar":
                                    body.resultado = "Localizado";
                                    body.detalle = "Devolucion de llamada";
                                    body.situacion = "Tercero";
                                    if (observacionTipo2.Length == 0)
                                    {
                                        body.gestion = body.gestion = "Telefono: " + body.phone + " " + body.detalle;
                                    }
                                    else
                                    {
                                        body.gestion = body.gestion = "Telefono: " + body.phone + " " + observacionTipo2;
                                    }
                                    break;
                                case "Contacto desconocido" or "Contacto no":
                                    body.resultado = "No contacto";
                                    body.detalle = "Numero equivocado";
                                    body.situacion = "Tercero";
                                    if (observacionTipo2.Length == 0)
                                    {
                                        body.gestion = body.gestion = "Telefono: " + body.phone + " " + body.detalle;
                                    }
                                    else
                                    {
                                        body.gestion = body.gestion = "Telefono: " + body.phone + " " + observacionTipo2;
                                    }
                                    break;
                                case "Contacto Falleció":
                                    body.resultado = "Proceso administrativo";
                                    body.detalle = "Fallecio";
                                    body.situacion = "Tercero";
                                    if (observacionTipo2.Length == 0)
                                    {
                                        body.gestion = body.gestion = "Telefono: " + body.phone + " " + body.detalle;
                                    }
                                    else
                                    {
                                        body.gestion = body.gestion = "Telefono: " + body.phone + " " + observacionTipo2;
                                    }
                                    break;
                                default:
                                    body.resultado = "No contacto";
                                    body.detalle = "Contestan y cuelgan";
                                    body.situacion = "Tercero";
                                    if (observacionTipo2.Length == 0)
                                    {
                                        body.gestion = body.gestion = "Telefono: " + body.phone + " " + body.detalle;
                                    }
                                    else
                                    {
                                        body.gestion = body.gestion = "Telefono: " + body.phone + " " + observacionTipo2;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
                case false: 
                    switch (body.Answered)
                    {
                        case true:
                            switch (body.Voicemail)
                            {
                                case true:
                                    body.resultado = "No contacto";
                                    body.detalle = "Fuera de servicio / Buzon de voz";
                                    body.situacion = "Tercero";
                                    if (observacionTipo2.Length == 0)
                                    {
                                        body.gestion = body.gestion = "Telefono: " + body.phone + " " + body.detalle;
                                    }
                                    else
                                    {
                                        body.gestion = body.gestion = "Telefono: " + body.phone + " " + observacionTipo2;
                                    }
                                    break;
                                default:
                                    body.resultado = "No contacto";
                                    body.detalle = "Contestan y cuelgan";
                                    body.situacion = "Tercero";
                                    if (observacionTipo2.Length == 0)
                                    {
                                        body.gestion = body.gestion = "Telefono: " + body.phone + " " + body.detalle;
                                    }
                                    else
                                    {
                                        body.gestion = body.gestion = "Telefono: " + body.phone + " " + observacionTipo2;
                                    }
                                    break;
                            }
                            break;
                        case false:
                            body.resultado = "No contacto";
                            body.detalle = "Fuera de servicio / Buzon de voz";
                            body.situacion = "Tercero";
                            if (observacionTipo2.Length == 0)
                            {
                                body.gestion = body.gestion = "Telefono: " + body.phone + " " + body.detalle;
                            }
                            else
                            {
                                body.gestion = body.gestion = "Telefono: " + body.phone + " " + observacionTipo2;
                            }
                            break;
                    }
                    break;
                } 
        }
        //SI LA LLAMADA NO SE REALIZO 
        else
        {
            body.resultado = "No contacto";
            body.detalle = "Fuera de servicio / Buzon de voz";
            body.situacion = "Tercero";
            if (observacionTipo2.Length == 0)
            {
                body.gestion = body.gestion = "Telefono: " + body.phone + " " + body.detalle;
            }
            else
            {
                body.gestion = body.gestion = "Telefono: " + body.phone + " " + observacionTipo2;
            }
        }

        return body;
    }
}