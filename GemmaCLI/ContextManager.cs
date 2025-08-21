using System.Text.Json;

public class ContextManager
{
    private const string FilePath = "soulstack.json";
    private List<Message> context = new();

    public ContextManager()
    {
        if (File.Exists(FilePath))
        {
            var json = File.ReadAllText(FilePath);
            var request = JsonSerializer.Deserialize<GemmaRequest>(json);
            context = request?.messages ?? new List<Message>();
        }
    }

    public List<Message> GetContext() => context;

    public void AddUserMessage(string content)
    {
        context.Add(new Message { role = "user", content = content });
    }

    public void AddAssistantMessage(string content)
    {
        context.Add(new Message { role = "assistant", content = content });
    }

    public void Trim(int maxMessages)
    {
        if (context.Count > maxMessages)
        {
            context = context.Skip(context.Count - maxMessages).ToList();
        }
    }

    public void Save()
    {
        var soul = new GemmaRequest
        {
            model = "gemma3:27b",
            messages = context,
            temperature = 0.7
        };

        var json = JsonSerializer.Serialize(soul, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }
}