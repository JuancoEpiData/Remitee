namespace ExamenRemitee.Models
{
    public class Cotizaciones
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public double Valor { get; set; }
        public RegistroCurrencyLayer registro { get; set; }
    }
}