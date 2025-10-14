using System.ComponentModel.DataAnnotations.Schema;

namespace SCinema.Models
{
    public class PanierItem
    {
        public int Id { get; set; }

        public int PanierId { get; set; }
        public Panier? Panier { get; set; }

        public int SeanceId { get; set; }
        public Seance? Seance { get; set; }

        public int? SiegeId { get; set; }
        public Siege? Siege { get; set; }

        // ✅ On veut utiliser CategorieAge dans le code
        // ✅ On garde le nom de colonne "Categorie" en base pour éviter une migration
        [Column("Categorie")]
        public AgeCategorie CategorieAge { get; set; }

        public decimal PrixUnitaire { get; set; }
    }
}
