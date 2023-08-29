using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Data;

public class DbInitializer {

    public static async Task InitDb(WebApplication app) {
        await DB.InitAsync("SearchDb", MongoClientSettings
            .FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .CreateAsync();

        var count = await DB.CountAsync<Item>();

        if (count == 0) {
            Console.WriteLine("No data - will attempt to seed");

            var itemData = await File.ReadAllTextAsync("Data/Auctions.json");

            var items = JsonSerializer.Deserialize<Item[]>(itemData, new JsonSerializerOptions());

            await DB.SaveAsync(items);
        }
    }
}