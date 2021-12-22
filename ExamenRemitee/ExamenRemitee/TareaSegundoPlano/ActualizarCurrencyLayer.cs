using ExamenRemitee.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ExamenRemitee.TareaSegundoPlano
{
    public class ActualizarCurrencyLayer : IHostedService, IDisposable
    {
        Timer _Timer;
        bool _PrimeraEjecucion = true;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_PrimeraEjecucion) // para forzar la primera ejecución con el inicio del programa ....
            {
                ActualizarCotizaciones(null);
                _PrimeraEjecucion = false;
            }
            _Timer = new Timer(ActualizarCotizaciones, null, TimeSpan.FromSeconds(120), TimeSpan.FromSeconds(600));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _Timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void ActualizarCotizaciones(object obj)
        {
            try
            {
                Hashtable resultado = ObtenerCotizacionesCurrencyLayer();
                
                if (resultado["success"].ToString() == "True") // ver de castear a bool
                    GuardarCotizaciones(resultado);
                else
                    AlertarError(resultado);
            }
            catch(Exception ex)
            {
                throw new Exception("Error en tarea automatica de cotizaciones: " + Environment.NewLine + ex.Message);
            }
        }

        private static Hashtable ObtenerCotizacionesCurrencyLayer()
        {
            try
            {
                Hashtable resultado;
                string url = "http://api.currencylayer.com/live?access_key=b0cabdcf17010f7ff1325424c91f3959"; // ver de sacarlo del appSetting
                WebRequest request = WebRequest.Create(url);
                //headers
                request.Method = "GET";
                request.ContentType = "application/json;charset=utf-8'";
                request.Timeout = 600;

                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    resultado = JsonSerializer.Deserialize<Hashtable>(streamReader.ReadToEnd());
                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void AlertarError(Hashtable resultado)
        {
            try
            {
                var error = JsonSerializer.Deserialize<Hashtable>(resultado["error"].ToString());
                string MensajeError = $"Codigo de error: {error["code"]}.{Environment.NewLine}" +
                    $"Tipo error: {error["type"]}{Environment.NewLine}" +
                    $"Información adicional: {error["info"]}{Environment.NewLine}";
                SMTP.Smtp.EnviarCorreo(MensajeError);
            }
            catch
            {
                throw;
            }
        }

        private static void GuardarCotizaciones(Hashtable resultado)
        {
            var cot = (JsonSerializer.Deserialize<Dictionary<string, double>>(resultado["quotes"].ToString()));
            RegistroCurrencyLayer reg = new()
            {
                Id = 0,
                Exito = true,
                PoliticaPrivacidad = resultado["privacy"].ToString(),
                Registro = DateTime.Now,
                MonedaFuente = resultado["source"].ToString(),
                TerminosURL = resultado["terms"].ToString(),
                Cotizaciones = cot.Select(c => new Models.Cotizaciones { Nombre = c.Key, Valor = c.Value }).ToList()
            };

            GuardarRegistros(reg);

           // falta hacer que guarde aca.

        }

        private static void GuardarRegistros(RegistroCurrencyLayer registros)
        {
            try
            {
                using var context = new Data.ExamenRemiteeContext();
                context.Registros.Add(registros);
                context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void Dispose()
        {
            _Timer?.Dispose();
        }
    }
}