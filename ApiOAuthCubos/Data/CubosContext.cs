using ApiOAuthCubos.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiOAuthCubos.Data
{
    public class CubosContext: DbContext
    {
        public CubosContext(DbContextOptions<CubosContext> options) : base(options) { }
        public DbSet<Cubo> Cubos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CompraCubo> ComprasCubos { get; set; }
    }
}
