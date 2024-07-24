using Carros.Classes.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace Carros.infraestrutura
{
    public class CarroDbContext : DbContext
    {
        private IConfiguration _configuration;

        public DbSet<Carro> Carros { get; set; }

        public CarroDbContext(IConfiguration configuration, DbContextOptions options) : base(options)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var typeDatabase = _configuration["TypeDatabase"];
            var connectionString = _configuration.GetConnectionString(typeDatabase);

            if(typeDatabase == "Mysql")
            {
                optionsBuilder.UseMySQL(connectionString);
            }
        }
    }
}
