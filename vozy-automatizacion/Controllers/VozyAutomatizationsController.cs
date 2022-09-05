#nullable disable
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using vozy_automatizacion.Models;
using vozy_automatizacion.Services;
using System.Text.Json;

namespace vozy_automatizacion.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VozyAutomatizationsController : ControllerBase
{
    private CollectionsWeb2Service web2Service = new CollectionsWeb2Service();

    // POST: api/VozyAutomatizations
    [HttpPost]
    public async Task<IActionResult> PostVozyAutomatization(JsonData jsonObject)
    {
        var resultado = Transform.AsignarDetalle(jsonObject);
        resultado = Transform.observacionesGestion(resultado);
        int primerNumeroTelefono = short.Parse(resultado.tel_llamada.Substring(3, 1));
        //sp SP_VOZY_CARGA_GESTION @idempresa @idmoneda @noprestamo @situacion @resultado @detalle @fechagestion @horainicio @minutos @telefono @montopromesa @observacionesGestion

        var valores = "SP_VOZY_CARGA_GESTION";
        using (var connection =
               new SqlConnection(
                   "Server=192.168.8.6;Database=InteligenciaDB_Fase2;User ID=ddonis;Password=0TkZDbcSPpn8"))
        {
            connection.Open();
            SqlCommand cmd = new(valores, connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter param;
            param = cmd.Parameters.Add("@IDEMPRESA", SqlDbType.Int);
            param.Value = resultado.codigo_banco;
            param = cmd.Parameters.Add("@MONEDA", SqlDbType.VarChar, 50);
            param.Value = resultado.moneda;
            param = cmd.Parameters.Add("@NOPRESTAMO", SqlDbType.VarChar, 50);
            param.Value = resultado.identificacion;
            param = cmd.Parameters.Add("@SITUACION", SqlDbType.VarChar, 150);
            param.Value = resultado.situacion;
            param = cmd.Parameters.Add("@RESULTADO", SqlDbType.VarChar, 150);
            param.Value = resultado.resultado;
            param = cmd.Parameters.Add("@DETALLE", SqlDbType.VarChar, 150);
            param.Value = resultado.detalle;
            param = cmd.Parameters.Add("@FECHAGESTION", SqlDbType.VarChar, 25);
            param.Value = resultado.fecha;
            param = cmd.Parameters.Add("@HORAINICIO", SqlDbType.VarChar, 25);
            param.Value = resultado.hora;
            param = cmd.Parameters.Add("@MINUTOS", SqlDbType.VarChar, 10);
            param.Value = resultado.minutos;
            param = cmd.Parameters.Add("@TELEFONO", SqlDbType.VarChar, 25);
            param.Value = resultado.tel_llamada.Substring(3);
            param = cmd.Parameters.Add("@MONTOPROMESA", SqlDbType.Money);
            param.Value = resultado.pago_de_intencion;
            param = cmd.Parameters.Add("@OBSERVACIONESGESTION", SqlDbType.NText);
            param.Value = resultado.gestionASubir;

            try
            {
                var rows = 0;
                var bandera = primerNumeroTelefono > 2 && primerNumeroTelefono < 8;
                if (bandera) rows = await cmd.ExecuteNonQueryAsync();
                connection.Close();
                return Ok(new { objeto = resultado, seIngreso = bandera, r = rows });
            }
            catch (Exception er)
            {
                return BadRequest(new { error = er.Message, body = resultado });
            }
        }
    }

    //POST: api/VozyAutomatizations/web2
    [Route("web2")]
    [HttpPost]
    public IActionResult PostWeb2(dynamic body)
    {
        string json = JsonSerializer.Serialize(body);
        Console.WriteLine(json);
        json.Replace("call origin", "call_origin");
        Console.WriteLine(json);
        //CollectionsWeb2 result = JsonSerializer.Deserialize<CollectionsWeb2>(json);
        //return Ok(web2Service.transformData(body));
        return Ok(json);
    }
}