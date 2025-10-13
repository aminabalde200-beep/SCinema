namespace SCinema.Models
{
    public class Salle
    {
        public int Id { get; set; }
        public int NumeroSalle { get; set; }
        public int Capacite { get; set; }

        // FK Fournisseur (ApplicationUser)
        public string FournisseurId { get; set; } = "";
        public ApplicationUser? Fournisseur { get; set; }

        public ICollection<Siege> Sieges { get; set; } = new List<Siege>();
        public ICollection<Seance> Seances { get; set; } = new List<Seance>();  
    }
}
