#nullable disable
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using vozy_automatizacion.Models;
using vozy_automatizacion.Services;

namespace vozy_automatizacion.Controllers;
//{base}/api/vozyAutomatizations
[Route("api/[controller]")]
[ApiController]
public class VozyAutomatizationsController : ControllerBase
{
    private CollectionsWeb2Service web2service = new CollectionsWeb2Service();

    private readonly string cadenaDeConexionBaseDeDatos;

    public VozyAutomatizationsController(IConfiguration config)
    {
        //this.cadenaDeConexionBaseDeDatos = config.GetConnectionString("prod");
        
        this.cadenaDeConexionBaseDeDatos = "Server=192.168.8.6;Database=InteligenciaDB_Fase2;User ID=vozy;Password=C3vX7N5#UeXh";
        //this.cadenaDeConexionBaseDeDatos = "Server=192.168.8.8;Database=InteligenciaDB_Fase2;User ID=vozy;Password=0X6O#0d369@j";


    }
    [HttpGet]
    public async Task<IActionResult> getPrueba()
    {
        return Ok("Funciona");
    }

    // POST: api/VozyAutomatizations
    [HttpPost]
    public async Task<IActionResult> PostVozyAutomatization(JsonData jsonObject)
    {
        var resultado = Transform.AsignarDetalle(jsonObject);
        resultado = Transform.observacionesGestion(resultado);
        int primerNumeroTelefono = short.Parse(resultado.tel_llamada.Substring(3, 1));
        using (var connection = new SqlConnection(this.cadenaDeConexionBaseDeDatos))
        {
            connection.Open();
            SqlCommand cmd = new("SP_VOZY_CARGA_GESTION", connection);
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
    public async Task<IActionResult> PostWeb2(CollectionsWeb2 body)
    {
        CollectionsWeb2 result = this.web2service.transformData(body);
        using (SqlConnection con = new (this.cadenaDeConexionBaseDeDatos))
        {
            try
            {
                await con.OpenAsync();
                SqlCommand cmd = new SqlCommand("SP_VOZY_WEB2", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //SE AGREGAN LOS PARAMETROS DEL SP
                SqlParameter param;
                param = cmd.Parameters.Add("@IDEMPRESA", SqlDbType.Int);
                param.Value = int.Parse(result.Codigo_del_banco);
                param = cmd.Parameters.Add("@MONEDA", SqlDbType.VarChar, 50);
                param.Value = result.Divisa;
                param = cmd.Parameters.Add("@NOPRESTAMO", SqlDbType.VarChar, 50);
                param.Value = result.identificacion;
                param = cmd.Parameters.Add("@SITUACION", SqlDbType.VarChar, 150);
                param.Value = result.situacion;
                param = cmd.Parameters.Add("@RESULTADO", SqlDbType.VarChar, 150);
                param.Value = result.resultado;
                param = cmd.Parameters.Add("@DETALLE", SqlDbType.VarChar, 150);
                param.Value = result.detalle;
                param = cmd.Parameters.Add("@FECHAGESTION", SqlDbType.VarChar, 25);
                param.Value = result.Date;
                param = cmd.Parameters.Add("@HORAINICIO", SqlDbType.VarChar, 25);
                param.Value = result.hour;
                param = cmd.Parameters.Add("@SEGUNDOS", SqlDbType.VarChar, 10);
                param.Value = result.Duration;
                param = cmd.Parameters.Add("@TELEFONO", SqlDbType.VarChar, 25);
                param.Value = result.phone;
                param = cmd.Parameters.Add("@MONTOPROMESA", SqlDbType.Money);  
                //SI VALUE VIENE VACIO "" LO CONVIERTO EN VALOR 0
                try
                {
                    param.Value = double.Parse(result.partial_payment);
                }
                catch (Exception e)
                {
                    param.Value = 0;
                }

                param = cmd.Parameters.Add("@OBSERVACIONESGESTION", SqlDbType.NText);
                param.Value = result.gestion;
                param = cmd.Parameters.Add("@CAMPAINID", SqlDbType.VarChar, 50);
                param.Value = result.campaign_id;
                //EJECUTO EL SP
                var sp = await cmd.ExecuteNonQueryAsync();
                //SI TODO SALE BIEN REGRESO EL BODY TRANSFORMADO
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }
    }
    
    
}