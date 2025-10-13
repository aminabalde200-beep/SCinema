namespace SCinema.Models
{
    public class TarifSeance
    {
        public int Id { get; set; }
        public int SeanceId { get; set; }
        public Seance? Seance { get; set; }

        public AgeCategorie Categorie { get; set; }
        public decimal Prix { get; set; }
    }
}
