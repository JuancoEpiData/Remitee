using System;
using System.Collections.Generic;

namespace ExamenRemitee.Models
{
    public class RegistroCurrencyLayer
    {
        public int Id { get; set; }
        public bool Exito { get; set; }
        public string TerminosURL { get; set; }
        public string PoliticaPrivacidad { get; set; }
        public DateTime Registro { get; set; }
        public string MonedaFuente { get; set; }
        public List<Cotizaciones> Cotizaciones { get; set; }
    }
}