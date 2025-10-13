using static SCinema.Models.EnumsVente;

namespace SCinema.Models
{
    public class Paiement
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public Reservation? Reservation { get; set; }

        public string Provider { get; set; } = "Stripe";
        public string PaymentIntentId { get; set; } = "";
        public DateTime DatePaiement { get; set; } = DateTime.UtcNow;
        public decimal Montant { get; set; }
        public PaiementStatut Statut { get; set; } = PaiementStatut.Initie;
    }
}
