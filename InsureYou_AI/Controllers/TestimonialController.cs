using InsureYou_AI.Context;
using InsureYou_AI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace InsureYou_AI.Controllers
{
    public class TestimonialController : Controller
    {
        private readonly InsureContext _context;

        public TestimonialController(InsureContext context)
        {
            _context = context;
        }

        public IActionResult TestimonialList()
        {
            var values = _context.Testimonials.ToList();
            return View(values);
        }

        [HttpGet]
        public IActionResult CreateTestimonial()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateTestimonial(Testimonial testimonial)
        {
            _context.Testimonials.Add(testimonial);
            _context.SaveChanges();
            return RedirectToAction("TestimonialList");
        }

        public IActionResult RemoveTestimonial(int id)
        {
            var value = _context.Testimonials.Find(id);
            _context.Testimonials.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("TestimonialList");
        }

        [HttpGet]
        public IActionResult UpdateTestimonial(int id)
        {
            var value = _context.Testimonials.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateTestimonial(Testimonial testimonial)
        {
            _context.Testimonials.Update(testimonial);
            _context.SaveChanges();
            return RedirectToAction("TestimonialList");
        }

        public async Task<IActionResult> CreateTestimonialWithClaudeAI()
        {
            string apiKey = ""; // <- API Key Girişi

            string prompt = "Bir sigorta şirketi için müşteri deneyimlerine dair yorum oluşturmak istiyorum. Yani ingilizce karşılığı ile testimonial. Bu alanda Türkçe olarak 6 tane yorum, 6 tane müşteri adı ve soyadı, bu müşterilerin ünvanı olsun. Buna göre içeriği hazırla.";

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
                ViewBag.testimonials = new List<string>
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

            var testimonials = fullText.Split('\n')
                                .Where(x => !string.IsNullOrEmpty(x))
                                .Select(x => x.TrimStart('1', '2', '3', '4', '5', '.', ' '))
                                .ToList();

            ViewBag.testimonials = testimonials;
            return View();
        }


        public async Task<IActionResult> CreateTestimonialsWithGemini()
        {
            var apiKey = ""; // <- Gemini API Key
            var model = "gemini-1.5-pro"; // veya ihtiyacına göre başka model
            var url = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={apiKey}";

            string prompt = "Bir sigorta şirketi için müşteri deneyimlerine dair yorum oluşturmak istiyorum. Yani ingilizce karşılığı ile testimonial. Bu alanda Türkçe olarak 6 tane yorum, 6 tane müşteri adı ve soyadı, bu müşterilerin ünvanı olsun. Buna göre içeriği hazırla.";

            using var client = new HttpClient();

            var requestBody = new
            {
                contents = new[]
                {
            new
            {
                role = "user",
                parts = new[]
                {
                    new { text = prompt }
                }
            }
        }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.testimonials = new List<string>
        {
            $"Gemini API'den Cevap Alınamadı. Hata: {response.StatusCode}"
        };
                return View();
            }

            using var jsonDoc = JsonDocument.Parse(json);

            // Metin çıktısını al
            var fullText = jsonDoc.RootElement
                                  .GetProperty("candidates")[0]
                                 .GetProperty("content")
                                 .GetProperty("parts")[0]
                                 .GetProperty("text")
                                 .GetString();

            // Her satırı ayrı testimonial olarak ayır
            var testimonials = fullText.Split('\n')
                                       .Where(x => !string.IsNullOrEmpty(x))
                                       .Select(x => x.TrimStart('1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', ' '))
                                       .ToList();

            ViewBag.testimonials = testimonials;
            return View();
        }

    }
}
