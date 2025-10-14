namespace SCinema.Models
{
    public class Film
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Synopsis { get; set; } = "";
        public string Genre { get; set; } = "";
        public int DureeMinutes { get; set; }
        public string AffichereUrl { get; set; } = "";
        public DateOnly DateSortie { get; set; }

        public string AfficheUrl { get; set; } = "";

        public ICollection<Seance> Seances { get; set; } = new List<Seance>();

    }
}
