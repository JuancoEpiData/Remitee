using System;

namespace ExamenRemitee.ViewModels
{
    public class ConsultaViewModel
    {
        public DateTime RegistroUltimaActualizacion { get; set; }
        public string MonedaOrigen { get; set; }
        public string MonedaDestino { get; set; }
        public double MonedaOrigenMonto { get; set; }
        public double MonedaDestinoMonto { get; set; }
        public double MonedaOrigenCotizacion { get; set; }
        public double MonedaDestinoCotizacion { get; set; }
    }
}