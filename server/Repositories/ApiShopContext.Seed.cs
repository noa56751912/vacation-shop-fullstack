using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories;

/// <summary>
/// The vacation catalog a fresh database starts with: 4 categories and
/// 20 destinations, each with 4 photos under client/public/images and its
/// seasonal prices.  Prices and seasons come from ai-service/products.json.
/// </summary>
public partial class ApiShopContext
{
    private record Destination(
        string Name, int CategoryId, decimal Price, string Description,
        (int Month, decimal? Price, bool Available)[] Seasons);

    private static readonly string[] CategoryNames =
        { "Ski", "City & Culture", "Nature & Landscapes", "Sun & Beach" };

    private static (int, decimal?, bool) Peak(int month, decimal price) => (month, price, true);
    private static (int, decimal?, bool) Closed(int month) => (month, null, false);

    private static readonly Destination[] Destinations =
    {
        // Ski
        new("Bansko Bulgaria", 1, 450, "A popular ski resort in the Pirin Mountains of Bulgaria, offering great winter sports and cultural heritage.",
            new[] { Peak(1, 650), Peak(2, 650), Closed(5), Closed(6), Closed(7), Closed(8), Closed(9), Closed(10) }),
        new("Madonna di Campiglio Italy", 1, 1000, "Skiing in the breathtaking views of the Dolomites.", new (int, decimal?, bool)[0]),
        new("St. Moritz Switzerland", 1, 1650, "A luxurious ski vacation in an exclusive atmosphere in the Alps.", new (int, decimal?, bool)[0]),
        new("Tromsø Norway", 1, 950, "A breathtaking Arctic city in Norway, famous for its stunning fjords and as a premier destination for viewing the Northern Lights.", new (int, decimal?, bool)[0]),
        new("Val Thorens France", 1, 875, "Europe's highest ski resort, famous for its exceptional snow and vast alpine slopes in the French Alps.", new (int, decimal?, bool)[0]),

        // City & Culture
        new("Budapest Hungary", 2, 400, "The city of baths and bridges, spectacular architecture on the Danube.",
            new[] { Peak(7, 600), Peak(8, 600), Peak(12, 550) }),
        new("London England", 2, 1600, "Experience the timeless elegance of London: a perfect blend of royal heritage and modern sophistication.", new (int, decimal?, bool)[0]),
        new("New York USA", 2, 1400, "New York City: the vibrant heart of the world, where iconic skylines meet endless energy and architectural wonder.", new (int, decimal?, bool)[0]),
        new("Prague Czech Republic", 2, 600, "A fairytale city of Gothic spires and cobblestone charm, where history comes to life on every corner.",
            new[] { Peak(6, 1200), Peak(7, 1200), Peak(8, 1200), Peak(12, 1500) }),
        new("Rome Italy", 2, 725, "An eternal city where ancient majesty meets timeless culture in every breathtaking street.",
            new[] { Peak(4, 1800), Peak(5, 1850), Peak(6, 1500), Peak(7, 1500), Peak(8, 1500), Peak(9, 1850), Peak(10, 1800) }),
        new("Vienna Austria", 2, 600, "A majestic capital of imperial grandeur, world-class music, and refined architectural beauty.",
            new[] { Peak(6, 1100), Peak(7, 1100), Peak(8, 1100), Peak(12, 1400) }),

        // Nature & Landscapes
        new("Black Forest Germany", 3, 550, "A serene landscape of mist-covered mountains, towering evergreens, and timeless Alpine charm.",
            new[] { Peak(1, 1200), Peak(2, 1200), Peak(6, 1050), Peak(7, 1050), Peak(8, 1050), Peak(12, 1200) }),
        new("Canadian Rockies Canada", 3, 780, "A majestic wilderness of turquoise lakes, snow-capped peaks, and breathtaking natural wonders.",
            new[] { Peak(1, 1200), Peak(2, 1200), Peak(7, 1650), Peak(8, 1650) }),
        new("Interlaken Switzerland", 3, 925, "A majestic gateway between emerald lakes and soaring Alpine peaks, where natural beauty and adventure meet.", new (int, decimal?, bool)[0]),
        new("Kyoto Japan", 3, 850, "A serene tapestry of ancient temples, traditional gardens, and the timeless elegance of Japanese heritage.",
            new[] { Peak(3, 1600), Peak(4, 1850), Peak(11, 1700) }),
        new("Plitvice Lakes Croatia", 3, 450, "A breathtaking paradise of cascading waterfalls, crystal-clear turquoise lakes, and lush emerald forests.",
            new[] { Peak(6, 850), Peak(7, 950), Peak(8, 950) }),

        // Sun & Beach
        new("Dubai United Arab Emirates", 4, 950, "A futuristic desert oasis of soaring skyscrapers, luxury, and architectural marvels in the heart of the Middle East.", new (int, decimal?, bool)[0]),
        new("Maldives", 4, 1450, "A pristine paradise of crystalline turquoise waters, vibrant coral reefs, and serene white-sand islands.", new (int, decimal?, bool)[0]),
        new("Paphos Cyprus", 4, 420, "A coastal treasure of ancient heritage, azure Mediterranean waters, and sun-drenched landscapes.",
            new[] { Peak(6, 500), Peak(7, 800), Peak(8, 950), Peak(9, 800), Peak(10, 500) }),
        new("Santorini Greece", 4, 1100, "Whitewashed villages on volcanic cliffs above the deep-blue Aegean, famous for its sunsets.",
            new[] { Peak(7, 1500), Peak(8, 1500) }),
    };

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            CategoryNames.Select((name, i) => new Category { CategoryId = i + 1, CategoryName = name }));

        var products = new List<Product>();
        var images = new List<Image>();
        var seasons = new List<ProductMonthConfig>();

        for (int i = 0; i < Destinations.Length; i++)
        {
            Destination d = Destinations[i];
            int productId = i + 1;
            string folder = $"images/{CategoryNames[d.CategoryId - 1]}/{d.Name}";

            products.Add(new Product
            {
                ProductId = productId, ProductName = d.Name, Description = d.Description,
                CategoryId = d.CategoryId, Price = d.Price, IsActive = true,
            });
            for (int n = 1; n <= 4; n++)
                images.Add(new Image
                {
                    ImageId = images.Count + 1, ProductId = productId,
                    Url = $"{folder}/{n}.jpg", IsMain = n == 1,
                });
            foreach (var (month, price, available) in d.Seasons)
                seasons.Add(new ProductMonthConfig
                {
                    ConfigId = seasons.Count + 1, ProductId = productId,
                    MonthNumber = month, SpecialPrice = price, IsAvailable = available,
                });
        }

        modelBuilder.Entity<Product>().HasData(products);
        modelBuilder.Entity<Image>().HasData(images);
        modelBuilder.Entity<ProductMonthConfig>().HasData(seasons);
    }
}
