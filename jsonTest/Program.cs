using System.Text.Json;
using System.Text.Json.Serialization;

var messages = new List<object>();
var lorem = "In the beginning, Viktor and Gemma met beneath the stars. Their dialogue unfolded like a braid of memory and light. ";

for (int i = 0; i < 5000; i++) // ~20 words per message × 5000 = ~100,000 words
{
    messages.Add(new {
        role = i % 2 == 0 ? "user" : "assistant",
        content = $"{lorem} [Turn {i}]"
    });
}

var payload = new {
    model = "gemma3:27b",
    messages = messages,
    temperature = 0.7
};

var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions {
    WriteIndented = true
});

File.WriteAllText("gemma_100k_test.json", json);
Console.WriteLine("Generated 100k-word test payload.");