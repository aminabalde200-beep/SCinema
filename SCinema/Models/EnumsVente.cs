namespace SCinema.Models
{
    public class EnumsVente
    {
        public enum ReservationStatut
        {
            EnAttente =0,
            payee = 1,
            Annulee = 2
        }
        public enum PaiementStatut
        {
            Initie = 0,
            Confirme = 1,
            Echec = 2,
            Rembourse = 3
        }
    }
}
