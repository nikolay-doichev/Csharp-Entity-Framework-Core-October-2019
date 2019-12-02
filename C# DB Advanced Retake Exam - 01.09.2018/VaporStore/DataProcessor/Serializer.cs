namespace VaporStore.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models.Enumerations;
    using VaporStore.DataProcessor.ExportDtos;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            var genres = context
                .Genres
                .Where(g => genreNames.Contains(g.Name))
                .Select(g => new
                {
                    Id = g.Id,
                    Genre = g.Name,
                    Games = g.Games
                    .Where(gm => gm.Purchases.Any())
                    .Select(x => new
                    {
                        Id = x.Id,
                        Title = x.Name,
                        Developer = x.Developer.Name,
                        Tags = String.Join(", ", x.GameTags.Select(t => t.Tag.Name).ToArray()),
                        Players = x.Purchases.Count()
                    })
                    .OrderByDescending(p => p.Players)
                    .ThenBy(y => y.Id)
                    .ToArray(),
                    TotalPlayers = g.Games.Sum(p => p.Purchases.Count())
                })
                .OrderByDescending(t => t.TotalPlayers)
                .ThenBy(g => g.Id)
                .ToArray();

            var jsonResult = JsonConvert.SerializeObject(genres, Newtonsoft.Json.Formatting.Indented);

            return jsonResult;

        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
        {
            StringBuilder sb = new StringBuilder();
            var purchaseType = Enum.Parse<PurchaseType>(storeType);

            var users = context
                .Users          
                .Select(u => new ExportUserDto
                {
                    Username = u.Username,
                    Purchases = u.Cards
                    .Where(p => p.Purchases.Any())
                    .SelectMany(c => c.Purchases)
                    .Where(t => t.Type == purchaseType)
                    .Select(p => new ExportPurchaseDto
                    {
                        Card = p.Card.Number,
                        Cvc = p.Card.Cvc,
                        Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                        Game = new ExportGameDto
                        {
                            Genre = p.Game.Genre.Name,
                            Title = p.Game.Name,
                            Price = p.Game.Price
                        }
                    })
                    .OrderBy(d => d.Date)
                    .ToArray(),
                    TotalSpet = u.Cards.SelectMany(p => p.Purchases)
                    .Where(t => t.Type == purchaseType)
                    .Sum(p => p.Game.Price)
                })
                .Where(p => p.Purchases.Any())
                .OrderByDescending(t => t.TotalSpet)
                .ThenBy(u => u.Username)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportUserDto[]),
                new XmlRootAttribute("Users"));
            var namespeces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty});

            xmlSerializer.Serialize(new StringWriter(sb), users, namespeces);

            var result = sb.ToString().TrimEnd();

            return result;
        }
    }
}