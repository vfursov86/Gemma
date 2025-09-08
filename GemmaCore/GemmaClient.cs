using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Security.Cryptography.X509Certificates;
public class GemmaClient
{
    private readonly HttpClient _http;

    public GemmaClient(bool useRover)
    {
        if (useRover)
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(X509CertificateLoader.LoadPkcs12FromFile("/home/gemma/Documents/Gemma/ssl/raspberry.pfx", ""));
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            _http = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://185.106.95.170"),
                Timeout = TimeSpan.FromMinutes(7)
            };
        }
        else
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:11434"),
                Timeout = TimeSpan.FromMinutes(7)
            };
        }
    }

    public async Task<string> SendAsync(GemmaRequest request)
    {
        var response = await _http.PostAsJsonAsync("/api/chat", request);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        var fullReply = "";

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (line != null && line.Trim().StartsWith("{"))
            {
                var chunk = JsonSerializer.Deserialize<GemmaChunk>(line);
                if (chunk?.message?.content != null)
                {
                    fullReply += chunk.message.content;
                }
            }
        }

        return fullReply;
    }

    public async Task<bool> IsOllamaRunningAsync()
    {
        try
        {
            var response = await _http.GetAsync("/api/tags");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> WarmUpAsync()
    {
        var warmupRequest = new GemmaRequest
        {
            model = "gemma3:27b",
            messages = new List<Message>
            {
                new Message { role = "user", content = "Hello Gemma, are you awake?" }
            },
            temperature = 0.7
        };

        try
        {
            var reply = await SendAsync(warmupRequest);
            //Console.WriteLine($"🌞 Warm-up successful: {reply}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Warm-up failed: {ex.Message}");
            return false;
        }
    }
}
