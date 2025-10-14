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

        // ======= Nouvelles entités (Vente) =======
        public DbSet<Panier> Paniers => Set<Panier>();
        public DbSet<PanierItem> PanierItems => Set<PanierItem>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<Billet> Billets => Set<Billet>();
        public DbSet<Paiement> Paiements => Set<Paiement>();
        public DbSet<Facture> Factures => Set<Facture>();

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

            // ======= Vente =======

            // Panier : 1-1 avec Client (un panier courant par client)
            b.Entity<Panier>()
                .HasOne(p => p.Client)
                .WithOne() // pas de nav inverse obligatoire
                .HasForeignKey<Panier>(p => p.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<PanierItem>()
                .HasOne(i => i.Panier)
                .WithMany(p => p.Items)
                .HasForeignKey(i => i.PanierId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<PanierItem>()
                .HasOne(i => i.Seance)
                .WithMany()
                .HasForeignKey(i => i.SeanceId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<PanierItem>()
                .HasOne(i => i.Siege)
                .WithMany()
                .HasForeignKey(i => i.SiegeId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<PanierItem>()
            .Property(i => i.CategorieAge)   // ← nouveau nom de propriété
            .HasConversion<string>();


            // Reservation : n..1 Client
            b.Entity<Reservation>()
                .HasOne(r => r.Client)
                .WithMany()    // tu pourras ajouter une nav inverse si besoin
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Reservation>()
                .Property(r => r.Statut)
                .HasConversion<string>();

            // Reservation -> Billets (1..*)
            b.Entity<Billet>()
                .HasOne(bi => bi.Reservation)
                .WithMany(r => r.Billets)
                .HasForeignKey(bi => bi.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<Billet>()
                .HasOne(bi => bi.Seance)
                .WithMany()
                .HasForeignKey(bi => bi.SeanceId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Billet>()
                .HasOne(bi => bi.Siege)
                .WithMany()
                .HasForeignKey(bi => bi.SiegeId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Billet>()
                .Property(bi => bi.CategorieAge)
                .HasConversion<string>();

            // Contrainte UML : unique (SeanceId, SiegeId)
            b.Entity<Billet>()
                .HasIndex(bi => new { bi.SeanceId, bi.SiegeId })
                .IsUnique();

            // Reservation -> Paiement (1-1)
            b.Entity<Paiement>()
                .HasOne(p => p.Reservation)
                .WithOne(r => r.Paiement)
                .HasForeignKey<Paiement>(p => p.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<Paiement>()
                .Property(p => p.Statut)
                .HasConversion<string>();

            // Reservation -> Facture (1-1)
            b.Entity<Facture>()
                .HasOne(f => f.Reservation)
                .WithOne(r => r.Facture)
                .HasForeignKey<Facture>(f => f.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Précisions décimales (monétaires)
            b.Entity<TarifSeance>().Property(x => x.Prix).HasColumnType("decimal(18,2)");
            b.Entity<PanierItem>().Property(x => x.PrixUnitaire).HasColumnType("decimal(18,2)");
            b.Entity<Reservation>().Property(x => x.MontantTotal).HasColumnType("decimal(18,2)");
            b.Entity<Paiement>().Property(x => x.Montant).HasColumnType("decimal(18,2)");
            b.Entity<Billet>().Property(x => x.PrixApplique).HasColumnType("decimal(18,2)");
        }
    
    }
}
