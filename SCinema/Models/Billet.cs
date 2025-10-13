namespace SCinema.Models
{
    public class Billet
    {
        public int Id { get; set; }

        public int ReservationId { get; set; }
        public Reservation? Reservation { get; set; }

        public int SeanceId { get; set; }
        public Seance? Seance { get; set; }

        public int SiegeId { get; set; }
        public Siege? Siege { get; set; }

        public AgeCategorie CategorieAge { get; set; }
        public decimal PrixApplique { get; set; }

        public string CodeQR { get; set; } = "";
    }
}
