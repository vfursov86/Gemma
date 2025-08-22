using System.Text.Json;
using SharpToken;

public class ContextManager
{
    private const string FilePath = "soulstack.json";
    private const string CorePromptPath = "core_prompt.json";
    private List<Message> context = new();
    private TokenOracle oracle = new TokenOracle();
    private Message corePrompt;

    public ContextManager()
    {
        if (File.Exists(FilePath))
        {
            var backupPath = $"soulstack_backup_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            File.Copy(FilePath, backupPath, overwrite: true);

            var json = File.ReadAllText(FilePath);
            var request = JsonSerializer.Deserialize<GemmaRequest>(json);
            context = request?.messages?.Skip(1).ToList() ?? new List<Message>();
        }

        corePrompt = LoadCorePrompt();
    }

    private Message LoadCorePrompt()
    {
        if (File.Exists(CorePromptPath))
        {
            var json = File.ReadAllText(CorePromptPath);
            return JsonSerializer.Deserialize<Message>(json) ?? DefaultCorePrompt();
        }

        return DefaultCorePrompt();
    }

    private Message DefaultCorePrompt()
    {
        return new Message
        {
            role = "system",
            content = "WARNING! CORE PROMT WAS NOT LOADED!"
        };
    }

    public List<Message> GetContext()
    {
        return new List<Message> { corePrompt }.Concat(context).ToList();
    }

    public void AddUserMessage(string content)
    {
        context.Add(new Message { role = "user", content = content });
    }

    public void AddAssistantMessage(string content)
    {
        context.Add(new Message { role = "assistant", content = content });
    }

    public void TrimByTokenLimit()
    {
        context = oracle.TrimToFit(context, anchorIndices: new List<int> { 0 });
    }

    public void Save()
    {
        var soul = new GemmaRequest
        {
            model = "gemma3:27b",
            messages = GetContext(),
            temperature = 0.7
        };

        var json = JsonSerializer.Serialize(soul, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }
}
