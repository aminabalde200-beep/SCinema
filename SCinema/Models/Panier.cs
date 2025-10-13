namespace SCinema.Models
{
    public class Panier
    {
        public int Id { get; set; }
        public string ClientId { get; set; } = "";
        public ApplicationUser? Client { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<PanierItem> Items { get; set; } = new List<PanierItem>();
    }
}
