using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureYou_AI.Controllers
{
    public class ImageAIController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ImageAIController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult CreateImageWithOpenAI()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateImageWithOpenAI(string prompt)
        {
            var apiKey = ""; // <- API Key Girişi
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestData = new
            {
                prompt = prompt,
                n = 1,
                size = "512x512"
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.openai.com/v1/images/generations", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.error = "OpenAI Hatası: " + await response.Content.ReadAsStringAsync();
                return View();
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(json);
            var imageUrl = result.RootElement.GetProperty("data")[0].GetProperty("url").GetString();

            return View(model: imageUrl);
        }

        [HttpGet]
        public IActionResult CreateImageWithGemini()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateImageWithGemini(string prompt)
        {
            var apiKey = ""; // <- Gemini API Key
            var model = "gemini-1.5-pro";
            var url = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateImage?key={apiKey}";

            using var client = new HttpClient();

            // JSON payload burada
            var requestBody = new
            {
                prompt = prompt,
                size = "512x512",
                n = 1
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.error = "Gemini Hatası: " + json;
                return View();
            }

            using var jsonDoc = JsonDocument.Parse(json);

            // JSON'dan base64 veya URL al
            var imageBase64 = jsonDoc.RootElement
                                     .GetProperty("data")[0]
                                     .GetProperty("b64_json")
                                     .GetString();

            if (string.IsNullOrEmpty(imageBase64))
            {
                ViewBag.error = "Görsel oluşturulamadı.";
                return View();
            }

            var imageUrl = $"data:image/png;base64,{imageBase64}";
            return View(model: imageUrl);
        }
    }
}