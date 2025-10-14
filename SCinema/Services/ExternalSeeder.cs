using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using SCinema.Data;
using SCinema.Models;

namespace SCinema.Services
{
    public sealed class ExternalSeeder
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpClientFactory _http;

        public ExternalSeeder(ApplicationDbContext db, IHttpClientFactory http)
        {
            _db = db;
            _http = http;
        }

        /// <summary>
        /// Récupère ~20 "produits" depuis DummyJSON et les mappe en Films.
        /// N’insère que si la table Films est vide (évite les doublons à chaque démarrage).
        /// </summary>
        public async Task SeedFilmsAsync()
        {
            // Si tu veux re-seeder même si la table n'est pas vide, commente ce guard.
            if (_db.Films.Any()) return;

            var client = _http.CreateClient();

            // DummyJSON: https://dummyjson.com/products?limit=20
            var url = "https://dummyjson.com/products?limit=20";

            RootProducts? data = null;

            try
            {
                data = await client.GetFromJsonAsync<RootProducts>(url);
            }
            catch
            {
                // Si l'appel échoue (pas de réseau, etc.), on ignore silencieusement pour ne pas casser le démarrage.
                return;
            }

            if (data?.Products == null || data.Products.Count == 0)
                return;

            var rnd = new Random();

            var films = new List<Film>();
            foreach (var p in data.Products)
            {
                // Mapping "produit -> film"
                var film = new Film
                {
                    Title = p.Title ?? "Titre inconnu",
                    Synopsis = p.Description ?? "—",
                    Genre = p.Category ?? "Divers",
                    DureeMinutes = rnd.Next(80, 140), // durée factice 80–139 min
                    AfficheUrl = (p.Images != null && p.Images.Count > 0) ? p.Images[0] : p.Thumbnail ?? "",
                    DateSortie = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-rnd.Next(30, 900))) // date factice passée
                };

                films.Add(film);
            }

            _db.Films.AddRange(films);
            await _db.SaveChangesAsync();
        }

        // ====== DTOs pour DummyJSON ======
        private sealed class RootProducts
        {
            [JsonPropertyName("products")]
            public List<Product> Products { get; set; } = new();
        }

        private sealed class Product
        {
            [JsonPropertyName("title")] public string? Title { get; set; }
            [JsonPropertyName("description")] public string? Description { get; set; }
            [JsonPropertyName("category")] public string? Category { get; set; }
            [JsonPropertyName("thumbnail")] public string? Thumbnail { get; set; }
            [JsonPropertyName("images")] public List<string> Images { get; set; } = new();
        }
    }
}
