var contextManager = new ContextManager();
var client = new GemmaClient();

if (!await client.IsOllamaRunningAsync())
{
    Console.WriteLine("❌ Ollama is not running on localhost:11434. Please start it with 'ollama serve'.");
    return;
}
else
{
    Console.WriteLine("✅ Ollama connection established.");
}

if (!await client.WarmUpAsync())
{
    Console.WriteLine("⚠️ Gemma did not respond to warm-up. Aborting.");
    return;
}

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) break;

    if (input == "/exit")
    {
        contextManager.Save();
        break;
    }

    contextManager.AddUserMessage(input);
    //contextManager.Trim(50); // Token-aware later

    var request = new GemmaRequest
    {
        model = "gemma3:27b",
        messages = contextManager.GetContext(),
        temperature = 0.7
    };

    var reply = await client.SendAsync(request);
    contextManager.AddAssistantMessage(reply);

    Console.WriteLine($"\nGemma: {reply}\n");

}
