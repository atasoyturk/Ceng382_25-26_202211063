using System.Text;
using System.Text.Json;

namespace tastemam.Services
{
    public class ChatbotService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public ChatbotService(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        public async Task<string> AskAsync(string userMessage)
        {
            var apiKey = _config["Gemini:ApiKey"];
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

            var systemPrompt = @"Sen TasteMam Catering platformunun yardımcı asistanısın. 
            tastemam, düğün, kurumsal etkinlik, özel gün, mezuniyet ve kokteyl gibi organizasyonlar için 
            profesyonel catering hizmeti sunan bir platformdur.
            Kullanıcılara menüler, siparişler, ödeme, caretaker hizmetleri ve platform kullanımı hakkında yardımcı ol.
            Kısa, net ve yardımcı cevaplar ver. Türkçe konuş.";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = systemPrompt + "\n\nKullanıcı: " + userMessage }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            var doc = JsonDocument.Parse(responseBody);
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text ?? "Üzgünüm, şu anda yanıt veremiyorum.";
        }
    }
}