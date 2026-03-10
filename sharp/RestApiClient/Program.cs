using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static async Task Main(string[] args)
    {
        using HttpClient client = new HttpClient();
        Console.WriteLine("REST API CLIENT === Succes ===");
        Console.Write("URL: ");
        string url = Console.ReadLine();

        try
        {
            string response = await client.GetStringAsync(url);
            
            using JsonDocument doc = JsonDocument.Parse(response);
            JsonElement root = doc.RootElement;

            List<JsonElement> itemsToDisplay = new List<JsonElement>();

            if (root.ValueKind == JsonValueKind.Array)
            {
                itemsToDisplay = root.EnumerateArray().ToList();
            }
            else if (root.ValueKind == JsonValueKind.Object)
            {
                if (root.TryGetProperty("results", out JsonElement resultsProp))
                    itemsToDisplay = resultsProp.EnumerateArray().ToList();
                else if (root.TryGetProperty("data", out JsonElement dataProp))
                    itemsToDisplay = dataProp.EnumerateArray().ToList();
                else
                {
                    Console.WriteLine("The data has been received, but it is a single object, not a list.");
                    Console.WriteLine(root.GetRawText());
                    return;
                }
            }

            // ШАГ 2: Вывод данных в консоль
            if (itemsToDisplay.Count > 0)
            {
                DisplayAsTable(itemsToDisplay);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void DisplayAsTable(List<JsonElement> items)
    {
        var properties = items[0].EnumerateObject().Select(p => p.Name).ToList();

        Console.WriteLine(new string('-', 80));
        foreach (var item in items)
        {
            foreach (var prop in item.EnumerateObject())
            {
                Console.WriteLine($"{prop.Name}: {prop.Value}");
            }
            Console.WriteLine(new string('-', 80));
        }
    }
}
