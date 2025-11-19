using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using pgr.Model;

namespace pgr.Banco;
public class PgrDbCon : IdentityDbContext<Usuario>
{
    public DbSet<RegistroDeTreinos> RegistroDeTreinos { get; set; }
    public DbSet<TreinoExercicios> TreinoExercicios { get; set; }
    public DbSet<Treinos> Treinos { get; set; }
    public DbSet<Exercicios> Exercicios { get; set; }
    public PgrDbCon(DbContextOptions<PgrDbCon> ctt) : base(ctt)
    {
    }
}
