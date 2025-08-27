 using InsureYou_AI.Context;
using InsureYou_AI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace InsureYou_AI.Controllers
{
    public class BlogController : Controller
    {
        private readonly InsureContext _context;

        public BlogController(InsureContext context)
        {
            _context = context;
        }

        public IActionResult BlogList()
        {
            return View();
        }

        public IActionResult BlogDetail(int id)
        {
            ViewBag.articleId = id;
            return View();
        }

        [HttpGet]
        public PartialViewResult AddComment()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            comment.CreatedDate= DateTime.Now;
            comment.AppUserId = "42e42e52-b6d2-4e84-a284-872b3badf749";

            using (var client = new HttpClient())
            {
                var apiKey = ""; // <- API Key Girişi
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                try
                {
                    var translateRequestBody = new
                    {
                        inputs = comment.Detail
                    };

                    var translateJson = JsonSerializer.Serialize(translateRequestBody);
                    var translateContent = new StringContent(translateJson, Encoding.UTF8, "application/json");
                    
                    var translateResponse = await client.PostAsync("https://api-inference.huggingface.co/models/Helsinki-NLP/opus-mt-tr-en", translateContent);
                    var translateResponseString = await translateResponse.Content.ReadAsStringAsync();

                    string englishText = comment.Detail;
                    if (translateResponseString.TrimStart().StartsWith("["))
                    {
                        var translateDoc = JsonDocument.Parse(translateResponseString);
                        englishText = translateDoc.RootElement[0]
                                            .GetProperty("translation_text")
                                            .GetString();
                    }
                    //ViewBag.v = englishText;

                    var toxicRequestBody = new
                    {
                        inputs = englishText
                    };

                    var toxicJson = JsonSerializer.Serialize(toxicRequestBody);
                    var toxicContent = new StringContent(toxicJson, Encoding.UTF8, "application/json");
                    var toxicResponse = await client.PostAsync("https://api-inference.huggingface.co/models/unitary/toxic-bert", toxicContent);
                    var toxicResponseString = await toxicResponse.Content.ReadAsStringAsync();

                    if (toxicResponseString.TrimStart().StartsWith("["))
                    {
                        var toxicDoc = JsonDocument.Parse(toxicResponseString);
                        foreach (var item in toxicDoc.RootElement[0].EnumerateArray())
                        {
                            string label = item.GetProperty("label").GetString();
                            double score = item.GetProperty("score").GetDouble();

                            if (score > 0.5)
                            {
                                comment.Status = "Toksik Yorum";
                                break;
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(comment.Status))
                    {
                        comment.Status = "Yorum Onaylandı";
                    }
                }
                catch (Exception ex)
                {
                    comment.Status = "Onay Bekliyor";
                }
            }

            _context.Comments.Add(comment);
            _context.SaveChanges();
            return RedirectToAction("BlogList");
        }

        [HttpGet]
        public PartialViewResult AddCommentGeminiAI()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> AddCommentGeminiAI(Comment comment)
        {
            comment.CreatedDate = DateTime.Now;
            comment.AppUserId = "42e42e52-b6d2-4e84-a284-872b3badf749";

            try
            {
                var apiKey = ""; // <- Google Gemini API Key
                var model = "gemini-1.5-pro";
                var url = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={apiKey}";

                using var client = new HttpClient();

                // Gemini için prompt: toksik mi değil mi kontrolü
                var requestBody = new
                {
                    contents = new[]
                    {
                new
                {
                    role = "user",
                    parts = new[]
                    {
                        new
                        {
                            text = $"Bu yorumu incele: \"{comment.Detail}\".\n" +
                                   "Eğer toksik, saldırgan veya hakaret içeriyorsa 'Toksik Yorum' yaz. " +
                                   "Eğer güvenli ve uygun bir yorumsa 'Yorum Onaylandı' yaz. " +
                                   "Sadece bu iki ifadeden birini döndür."
                        }
                    }
                }
            }
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                var responseJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var jsonDoc = JsonDocument.Parse(responseJson);
                    var resultText = jsonDoc.RootElement
                                            .GetProperty("candidates")[0]
                                            .GetProperty("content")
                                            .GetProperty("parts")[0]
                                            .GetProperty("text")
                                            .GetString();

                    // Gemini’den gelen cevabı kontrol et
                    if (!string.IsNullOrEmpty(resultText))
                    {
                        if (resultText.Contains("Toksik", StringComparison.OrdinalIgnoreCase))
                            comment.Status = "Toksik Yorum";
                        else
                            comment.Status = "Yorum Onaylandı";
                    }
                    else
                    {
                        comment.Status = "Onay Bekliyor";
                    }
                }
                else
                {
                    comment.Status = "Onay Bekliyor";
                }
            }
            catch (Exception)
            {
                comment.Status = "Onay Bekliyor";
            }

            _context.Comments.Add(comment);
            _context.SaveChanges();
            return RedirectToAction("BlogList");
        }
    }
}
