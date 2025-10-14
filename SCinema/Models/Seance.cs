namespace SCinema.Models
{
    public class Seance
    {
        public int Id { get; set; }
        
        public int FilmId { get; set; }
        public Film? Film { get; set; }
        public int SalleId { get; set; }
        public Salle? Salle { get; set; }

        // Publication 
        public string FournisseurId { get; set; } = "";
        public ApplicationUser? Fournisseur { get; set; }

        public DateTime DateSeance { get; set; }
        public DateTime HeureDebut { get; set; }
        public DateTime HeureFin { get; set; }
        public decimal Prix { get; set; }
     
         public string FormatFilm { get; set; } = ""; // 2D,3D

        public string Langue { get; set; } = ""; // VF, VO, VOSTFR

        public ICollection<TarifSeance> Tarifs { get; set; } = new List<TarifSeance>();

    }
}
