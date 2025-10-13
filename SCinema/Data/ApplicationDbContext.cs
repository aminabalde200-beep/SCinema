using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SCinema.Models;

namespace SCinema.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets catalogue
        public DbSet<Film> Films => Set<Film>();
        public DbSet<Salle> Salles => Set<Salle>();
        public DbSet<Siege> Sieges => Set<Siege>();
        public DbSet<Seance> Seances => Set<Seance>();
        public DbSet<TarifSeance> TarifsSeance => Set<TarifSeance>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            // Film -> Seances
            b.Entity<Film>()
                .HasMany(f => f.Seances)
                .WithOne(s => s.Film!)
                .HasForeignKey(s => s.FilmId)
                .OnDelete(DeleteBehavior.Restrict);

            // Salle -> Sieges
            b.Entity<Salle>()
                .HasMany(s => s.Sieges)
                .WithOne(x => x.Salle!)
                .HasForeignKey(x => x.SalleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Salle -> Seances
            b.Entity<Salle>()
                .HasMany(s => s.Seances)
                .WithOne(x => x.Salle!)
                .HasForeignKey(x => x.SalleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Salle <= Fournisseur (User)
            b.Entity<Salle>()
                .HasOne(s => s.Fournisseur)
                .WithMany() // pas de navigation inverse obligatoire
                .HasForeignKey(s => s.FournisseurId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seance <= Fournisseur (User)
            b.Entity<Seance>()
                .HasOne(s => s.Fournisseur)
                .WithMany()
                .HasForeignKey(s => s.FournisseurId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seance -> Tarifs (1..*)
            b.Entity<TarifSeance>()
                .HasOne(t => t.Seance!)
                .WithMany(s => s.Tarifs)
                .HasForeignKey(t => t.SeanceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unicités utiles
            b.Entity<Siege>()
                .HasIndex(x => new { x.SalleId, x.Rangee, x.NumeroSiege })
                .IsUnique();

            b.Entity<TarifSeance>()
                .HasIndex(t => new { t.SeanceId, t.Categorie })
                .IsUnique();

            // Enum stocké en string (lisible)
            b.Entity<TarifSeance>()
                .Property(t => t.Categorie)
                .HasConversion<string>();
        }
    }
}
