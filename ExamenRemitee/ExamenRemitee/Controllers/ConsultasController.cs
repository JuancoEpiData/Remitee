using ExamenRemitee.Data;
using ExamenRemitee.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExamenRemitee.Controllers
{
    [Route("api/[controller]/{action}")]
    [ApiController]
    public class ConsultasController : ControllerBase
    {
        private readonly ExamenRemiteeContext _context;

        public ConsultasController(ExamenRemiteeContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ConsultaViewModel> CalcularMontoDestino([FromBody] ConsultaViewModel consulta)
        {
            ValidarConsulta(consulta);
            try
            {
                await ObtenerValoresDeRefernecia(consulta);
                consulta.MonedaDestinoMonto = (consulta.MonedaOrigenMonto * consulta.MonedaDestinoCotizacion) / consulta.MonedaOrigenCotizacion;
                return consulta;
            }
            catch (Exception ex)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Error al procesar la solicitud: " + Environment.NewLine + ex.Message),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }

        [HttpPost]
        public async Task<ConsultaViewModel> CalcularMontoOrigen([FromBody] ConsultaViewModel consulta)
        {
            ValidarConsulta(consulta);
            try
            {
                await ObtenerValoresDeRefernecia(consulta);
                consulta.MonedaOrigenMonto = (consulta.MonedaDestinoMonto * consulta.MonedaOrigenCotizacion) / consulta.MonedaDestinoCotizacion;
                return consulta;
            }
            catch (Exception ex)
            {
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Error al procesar la solicitud: " + Environment.NewLine + ex.Message),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }


        //[HttpPost]
        //public async Task<ConsultaViewModel> CalcularMontoFaltante([FromBody] ConsultaViewModel consulta)// agrupo los dos metodos en una llamada.
        //{
        //    try
        //    {
        //        ValidarConsulta(consulta);
        //        await ObtenerValoresDeRefernecia(consulta);
        //        if(consulta.MonedaDestinoMonto > 0)
        //            consulta.MonedaOrigenMonto = (consulta.MonedaDestinoMonto * consulta.MonedaOrigenCotizacion) / consulta.MonedaDestinoCotizacion;
        //        else
        //            consulta.MonedaDestinoMonto = (consulta.MonedaOrigenMonto * consulta.MonedaDestinoCotizacion) / consulta.MonedaOrigenCotizacion;
        //        return consulta;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
        //        {
        //            Content = new StringContent("Error al procesar la solicitud: " + Environment.NewLine + ex.Message),
        //            ReasonPhrase = "Critical Exception"
        //        });
        //    }
        //}

        private static void ValidarConsulta(ConsultaViewModel consulta)
        {
            if (string.IsNullOrWhiteSpace(consulta.MonedaOrigen) || string.IsNullOrWhiteSpace(consulta.MonedaDestino))
                throw new BadHttpRequestException("Los parametros indicados no son correctos.");
        }

        private async Task ObtenerValoresDeRefernecia(ConsultaViewModel consulta)
        {
            try
            {
                var registro = await _context.Registros.AsQueryable()
                .OrderByDescending(r => r.Registro)
                .Where(r => r.Cotizaciones.Any(c => c.Nombre.Contains(consulta.MonedaOrigen))
                    && r.Cotizaciones.Any(c => c.Nombre.Contains(consulta.MonedaDestino)))
                .Include(r => r.Cotizaciones.Where(c => c.Nombre.Contains(consulta.MonedaOrigen) || c.Nombre.Contains(consulta.MonedaDestino)))
                .Select(r => new
                {
                    r.Registro,
                    MonedaOrigen = r.Cotizaciones.Where(c => c.Nombre.Contains(consulta.MonedaOrigen)).SingleOrDefault(),
                    MonedaDestino = r.Cotizaciones.Where(c => c.Nombre.Contains(consulta.MonedaDestino)).SingleOrDefault(),
                })
                .FirstOrDefaultAsync();

                if (registro == null || registro.MonedaDestino == null || registro.MonedaOrigen == null)
                    throw new ApplicationException("El sistema no cuenta con datos necesarios para realizar la operación.");

                consulta.RegistroUltimaActualizacion = registro.Registro;
                consulta.MonedaDestino = registro.MonedaDestino.Nombre;
                consulta.MonedaOrigen = registro.MonedaOrigen.Nombre;
                consulta.MonedaOrigenCotizacion = registro.MonedaOrigen.Valor;
                consulta.MonedaDestinoCotizacion = registro.MonedaDestino.Valor;
            }
            catch
            {
                throw;
            }
        }
    }
}
