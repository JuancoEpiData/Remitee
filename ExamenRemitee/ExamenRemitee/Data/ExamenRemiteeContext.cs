using Microsoft.EntityFrameworkCore;

namespace ExamenRemitee.Data
{
    public class ExamenRemiteeContext : DbContext
    {
        public ExamenRemiteeContext() { }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // sacar del ConnectionString.json
            optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-SV07TOP\SQLEXPRESS;Initial Catalog=RemiteeExamen;Integrated Security=True");
        }

        public DbSet<Models.RegistroCurrencyLayer> Registros { get; set; }
        public DbSet<Models.Cotizaciones> Cotizaciones { get; set; }
    }
}
