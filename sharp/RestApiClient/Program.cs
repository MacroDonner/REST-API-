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
        Console.WriteLine("=== Универсальный REST API Клиент ===");
        Console.Write("Введите URL: ");
        string url = Console.ReadLine();

        try
        {
            string response = await client.GetStringAsync(url);
            
            using JsonDocument doc = JsonDocument.Parse(response);
            JsonElement root = doc.RootElement;

            List<JsonElement> itemsToDisplay = new List<JsonElement>();

            // ШАГ 1: Умное определение структуры
            if (root.ValueKind == JsonValueKind.Array)
            {
                // Если API сразу выдал список (как Университеты или Котики)
                itemsToDisplay = root.EnumerateArray().ToList();
            }
            else if (root.ValueKind == JsonValueKind.Object)
            {
                // Если API выдал объект (как Rick & Morty), ищем массив внутри
                // Проверяем популярные названия полей: "results", "data", "items"
                if (root.TryGetProperty("results", out JsonElement resultsProp))
                    itemsToDisplay = resultsProp.EnumerateArray().ToList();
                else if (root.TryGetProperty("data", out JsonElement dataProp))
                    itemsToDisplay = dataProp.EnumerateArray().ToList();
                else
                {
                    Console.WriteLine("Данные получены, но это одиночный объект, а не список.");
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
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    static void DisplayAsTable(List<JsonElement> items)
    {
        // Берем ключи из первого элемента для заголовков
        var properties = items[0].EnumerateObject().Select(p => p.Name).ToList();

        // Рисуем простую таблицу (без обрезки!)
        Console.WriteLine(new string('-', 80));
        foreach (var item in items)
        {
            foreach (var prop in item.EnumerateObject())
            {
                // Выводим Ключ: Значение (так ссылки не будут обрезаться в кашу)
                Console.WriteLine($"{prop.Name}: {prop.Value}");
            }
            Console.WriteLine(new string('-', 80));
        }
    }
}
