using static SCinema.Models.EnumsVente;

namespace SCinema.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string ClientId { get; set; } = "";
        public ApplicationUser? Client { get; set; }

        public DateTime DateReservation { get; set; } = DateTime.UtcNow;
        public decimal MontantTotal { get; set; }

        public ReservationStatut Statut { get; set; } = ReservationStatut.EnAttente;
        public string StripePaymentIntentId { get; set; } = "";

        public ICollection<Billet> Billets { get; set; } = new List<Billet>();
        public Paiement? Paiement { get; set; }
        public Facture? Facture { get; set; }


    }
}
