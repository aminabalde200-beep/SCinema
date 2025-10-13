namespace SCinema.Models
{
    public class Siege
    {
        public int Id { get; set; }
        public int NumeroSiege { get; set; }

        // FK Salle
        public int SalleId { get; set; }
   
        public string Rangee { get; set; } = "";

        public string TypeSiege { get; set; } = "";

        public Salle? Salle { get; set; }

    }
}
