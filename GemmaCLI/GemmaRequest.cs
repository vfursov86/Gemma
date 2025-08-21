public class Message
{
    public string role { get; set; }
    public string content { get; set; }
}

public class GemmaRequest
{
    public string model { get; set; } = "gemma3:27b";
    public List<Message> messages { get; set; } = new();
    public double temperature { get; set; } = 0.7;
}
