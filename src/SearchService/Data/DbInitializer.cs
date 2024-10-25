using System;
using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Data;

public class DbInitializer
{
    public static bool PropertyNameSensetiveCase { get; private set; }

    public static async void Dbinit(WebApplication app){
        await DB.InitAsync("Search" , MongoClientSettings.
        FromConnectionString(app.Configuration.GetConnectionString("MongoConnectionString")));

        await DB.Index<Item>()
            .Key(x => x.Make , KeyType.Text)
            .Key(x=> x.Model , KeyType .Text)
            .Key(x => x.Color , KeyType.Text)
            .CreateAsync();

        var count = await DB.CountAsync<Item>();

        if(count == 0){
            var itemData = await File.ReadAllTextAsync("Data/data.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

            if (items != null)
            {
                await DB.SaveAsync(items);
            }
        }
    }
}