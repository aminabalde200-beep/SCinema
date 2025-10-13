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

        public AgeCategorie Categorie { get; set; }
        public decimal PrixUnitaire { get; set; }
    }
}
