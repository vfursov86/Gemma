using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GemmaInterModule.Services
{
    public class GemmaApiService
    {
        private readonly HttpClient _http;

        public GemmaApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string> SendPrompt(string prompt)
        {
            var request = new
            {
                model = "gemma",
                prompt = prompt,
                stream = false
            };

            var response = await _http.PostAsJsonAsync("http://localhost:11434/api/generate", request);
                  
            if (!response.IsSuccessStatusCode)
            {
                return $"[Error] Gemma could not respond. Status: {response.StatusCode}";
            }

            var result = await response.Content.ReadFromJsonAsync<GemmaResponse>();
            return result?.response ?? "[No response received]";
        }

        private class GemmaResponse
        {
            public string response { get; set; }
        }
    }
}
