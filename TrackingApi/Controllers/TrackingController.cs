using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using TrackingApi.Models;

namespace TrackingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackingController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TrackingController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost]
        public IActionResult RecibirDatos([FromBody] TrackingModel data)
        {
            try
            {
                string? connectionString = _configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    return StatusCode(500, "La cadena de conexión no está configurada.");
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("sp_EquipoRemoto_Insert", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@NombreEquipo", data.NombreEquipo);
                        command.Parameters.AddWithValue("@IpEquipo", data.IpEquipo);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok($"Datos insertados correctamente: {data.NombreEquipo}, {data.IpEquipo}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al insertar datos: {ex.Message}");
            }
        }
    }
}
