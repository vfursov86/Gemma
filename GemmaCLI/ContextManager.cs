using System.Text.Json;
using Microsoft.DeepDev;

public class ContextManager
{
    private const string FilePath = "soulstack.json";
    
    public ContextManager()
    { 
        var backupPath = $"soulstack_backup_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        File.Copy(FilePath, backupPath, overwrite: true);
    }
    private List<Message> context = new();
    private TokenOracle oracle = new TokenOracle();

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

    public void TrimByTokenLimit(List<int> anchorIndices)
    {
        //oracle.LogTokenUsage(context);
        context = oracle.TrimToFit(context, anchorIndices);
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
