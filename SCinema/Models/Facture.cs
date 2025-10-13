namespace SCinema.Models
{
    public class Facture
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public Reservation? Reservation { get; set; }

        public string Numero { get; set; } = "";
        public DateTime DateEmission { get; set; } = DateTime.UtcNow;
        public string PdfUrl { get; set; } = "";
    }
}
