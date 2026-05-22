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
            var apiKey = _config["Groq:ApiKey"];
            var url = "https://api.groq.com/openai/v1/chat/completions";

            var requestBody = new
            {
                model = "llama3-8b-8192",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "Sen TasteMam Catering platformunun yardımcı asistanısın. TasteMam, düğün, kurumsal etkinlik, özel gün, mezuniyet ve kokteyl gibi organizasyonlar için profesyonel catering hizmeti sunan bir platformdur. Kullanıcılara menüler, siparişler, ödeme, caretaker hizmetleri ve platform kullanımı hakkında yardımcı ol. Kısa, net ve yardımcı cevaplar ver. Türkçe konuş."
                    },
                    new
                    {
                        role = "user",
                        content = userMessage
                    }
                },
                max_tokens = 500
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            var doc = JsonDocument.Parse(responseBody);

            if (!doc.RootElement.TryGetProperty("choices", out var choices))
                return "Şu anda yanıt veremiyorum, lütfen tekrar deneyin.";

            var text = choices[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return text ?? "Üzgünüm, şu anda yanıt veremiyorum.";
        }
    }
}