using SharpToken;
using System.Collections.Generic;
using System.Linq;

public class TokenOracle
{
    // gemma3:27b context length.
    private const int MaxTokens = 130_000;
    private GptEncoding encoding;

    public TokenOracle()
    {
        encoding = GptEncoding.GetEncoding("cl100k_base");
    }

    public int CountTokens(string content)
    {
        return encoding.Encode(content).Count;
    }

    public int CountTotalTokens(List<Message> messages)
    {
        return messages.Sum(m => CountTokens(m.content));
    }

    public List<Message> TrimToFit(List<Message> messages, List<int> anchorIndices)
    {
        var preserved = anchorIndices
            .Where(i => i >= 0 && i < messages.Count)
            .Select(i => messages[i])
            .ToList();

        var nonAnchors = messages
            .Where((m, i) => !anchorIndices.Contains(i))
            .ToList();

        var trimmed = new List<Message>();
        int total = preserved.Sum(m => CountTokens(m.content));

        for (int i = nonAnchors.Count - 1; i >= 0; i--)
        {
            var msg = nonAnchors[i];
            int tokens = CountTokens(msg.content);
            if (total + tokens > MaxTokens) break;

            trimmed.Insert(0, msg);
            total += tokens;
        }

        return preserved.Concat(trimmed).ToList();
    }

    public void LogTokenUsage(List<Message> messages)
    {
        for (int i = 0; i < messages.Count; i++)
        {
            int count = CountTokens(messages[i].content);
            Console.WriteLine($"🔹 Message {i} ({messages[i].role}): {count} tokens");
        }
        Console.WriteLine($"🔸 Total tokens: {CountTotalTokens(messages)}");
    }


}
