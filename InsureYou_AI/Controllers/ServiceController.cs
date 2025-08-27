using InsureYou_AI.Context;
using InsureYou_AI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureYou_AI.Controllers
{
    public class ServiceController : Controller
    {
        private readonly InsureContext _context;

        public ServiceController(InsureContext context)
        {
            _context = context;
        }

        public IActionResult ServiceList()
        {
            var values = _context.Services.ToList();
            return View(values);
        }

        [HttpGet]
        public IActionResult CreateService()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateService(Service service)
        {
            _context.Services.Add(service);
            _context.SaveChanges();
            return RedirectToAction("ServiceList");
        }

        public IActionResult RemoveService(int id)
        {
            var value = _context.Services.Find(id);
            _context.Services.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("ServiceList");
        }

        [HttpGet]
        public IActionResult UpdateService(int id)
        {
            var value = _context.Services.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateService(Service service)
        {
            _context.Services.Update(service);
            _context.SaveChanges();
            return RedirectToAction("ServiceList");
        }

        [HttpGet]
        public async Task<IActionResult> CreateServiceWithGoogleGemini()
        {
            var apiKey = ""; // <- Gemini API Key Girişi
            var model = "gemini-1.5-pro";
            var url = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={apiKey}";
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts=new[]
                        {
                            new
                            {
                                text="Bir sigorta şirketi için hizmetler bölümü hazırlamanı istiyorum. Burada 5 farklı hizmet başlığı ve açıklaması olmalı. Bana maksimum 100 karakterden oluşan cümlelerle 5 tane hizmet içeriği yazar mısın başlıkları ile birlikte?"
                            }
                        }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(url, content);
            var responseJson = await response.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(responseJson);
            var fullText = jsonDoc.RootElement
                                 .GetProperty("candidates")[0]
                                 .GetProperty("content")
                                 .GetProperty("parts")[0]
                                 .GetProperty("text")
                                 .GetString();

            var services = fullText.Split('\n')
                        .Where(x => !string.IsNullOrEmpty(x))
                        .Select(x => x.TrimStart('1', '2', '3', '4', '5', '.', ' '))
                        .ToList();

            ViewBag.value = services;
            return View();
        }

        public async Task<IActionResult> CreateServiceWithAnthropicClaude()
        {
            string apiKey = ""; // <- Anthropic Claude API Key Girişi

            string prompt = "Bir sigorta şirketi için hizmetler bölümü hazırlamanı istiyorum. Burada 5 farklı hizmet başlığı ve açıklaması olmalı. Bana maksimum 100 karakterden oluşan cümlelerle 5 tane hizmet içeriği yazar mısın başlıkları ile birlikte?";

            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.anthropic.com/");
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);
            client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var requestBody = new
            {
                model = "claude-3-opus-20240229",
                max_tokens = 512,
                temperature = 0.5,
                messages = new[]
                {
                    new
                    {
                        role="user",
                        content=prompt
                    }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody));
            var response = await client.PostAsync("v1/messages", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.services = new List<string>
                {
                    $"Claude Api'den Cevap Alınamadı. Hata: {response.StatusCode}"
                };
                return View();
            }

            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);

            var fullText = doc.RootElement
                            .GetProperty("content")[0]
                            .GetProperty("text")
                            .GetString();

            var services = fullText.Split('\n')
                                 .Where(x => !string.IsNullOrEmpty(x))
                                 .Select(x => x.TrimStart('1', '2', '3', '4', '5', '.', ' '))
                                 .ToList();
            ViewBag.services = services;

            return View();
        }
    }
}
