﻿using InsureYou_AI.Context;
using InsureYou_AI.Entities;
using InsureYou_AI.Migrations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace InsureYou_AI.Controllers
{
    public class AppUserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly InsureContext _context;

        public AppUserController(UserManager<AppUser> userManager, InsureContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult UserList()
        {
            var values = _userManager.Users.ToList();
            return View(values);
        }

        public async Task<IActionResult> UserProfileWithAI(string id)
        {
            var value = await _userManager.FindByIdAsync(id);
            ViewBag.name = value.Name;
            ViewBag.surname = value.Surname;
            ViewBag.imageUrl = value.ImageUrl;
            ViewBag.description = value.Description;
            ViewBag.titlevalue = value.Title;
            ViewBag.city = value.City;
            ViewBag.education = value.Education;

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var articles = await _context.Articles
                                    .Where(x => x.AppUserId == id)
                                    .Select(y => y.Content)
                                    .ToListAsync();

            if (articles.Count == 0)
            {
                ViewBag.AIResult = "Bu kullanıcıya ait analiz yapılacak makale bulunamadı!";
                return View(user);
            }

            var allArticles = string.Join("\n\n", articles);
            var apiKey = ""; // <- API Key Girişi

            var prompt = $@"
Siz bir sigorta sektöründe uzman bir içerik analistisin.
Elinizde, bir sigorta şirketinin çalışanının yazdığı tüm makaleler var.
Bu makaleler üzerinden çalışanın içerik üretim tarzını analiz et.

Analiz Başlıkları:

1) Konu çeşitliliği ve odak alanları (sağlık, hayat, kasko, tamamlayıcı, BES vb.)
2) Hedef kitle tahmini (bireysel/kurumsal, segment, persona)
3) Dil ve Anlatım Tarzı (tekniklik seviyesi, okunabilirlik, ikna gücü)
4) Sigorta terimlerini kullanma ve doğruluk düzeyi
5) Müşteri ihtiyaçlarına ve risk yönetimine odaklanma
6) Pazarlama/satış vurgusu, CTA netliği
7) Geliştirilmesi gereken alanlar ve net aksiyon maddeleri

Makaleler:

{allArticles}

Lütfen çıktıyı profesyonel rapor formatında, madde madde ve en sonda 5 maddelik aksiyon listesi ile ver.";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var body = new
            {
                model = "gpt-4o-mini",
                messages = new object[]
                {
                    new {role="system",content="Sen kullanıcı yorum analizi yapan bir uzmansın."},
                    new {role="user",content= prompt }
                },
                max_tokens = 1000,
                temperature = 0.2
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpReponse = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var respText = await httpReponse.Content.ReadAsStringAsync();

            if (!httpReponse.IsSuccessStatusCode)
            {
                ViewBag.AIResult = "Open AI Hatası: " + httpReponse.StatusCode;
                return View(user);
            }

            try
            {
                using var doc = JsonDocument.Parse(respText);
                var aitText = doc.RootElement
                               .GetProperty("choices")[0]
                               .GetProperty("message")
                               .GetProperty("content")
                               .GetString();

                ViewBag.AIResult = aitText ?? "Boş yanıt döndü";
            }
            catch
            {
                ViewBag.AIResult = "OpenAI yanıtı beklenen formatta değil.";

            }
            return View(user);
        }

        public async Task<IActionResult> UserCommentsProfileWithAI(string id)
        {
            var values = await _userManager.FindByIdAsync(id);
            ViewBag.name = values.Name;
            ViewBag.surname = values.Surname;
            ViewBag.imageUrl = values.ImageUrl;
            ViewBag.description = values.Description;
            ViewBag.titlevalue = values.Title;
            ViewBag.city = values.City;
            ViewBag.education = values.Education;

            //Kullanıcı Bilgilerini Çekelim
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            //Kullanıcıya Ait Makale Listesi
            var comments = await _context.Comments
                                       .Where(x => x.AppUserId == id)
                                       .Select(y => y.Detail)
                                       .ToListAsync();

            if (comments.Count == 0)
            {
                ViewBag.AIResult = "Bu kullanıcıya ait analiz yapılacak yorum bulunamadı!";
                return View(user);
            }

            //Makaleleri tek bir metinde toplayalım
            var allComments = string.Join("\n\n", comments);

            var apiKey = ""; // <- API Key Girişi

            //Promptun Yazılması

            var prompt = $@"
Sen kullanıcı davranış analizi yapan bir yapay zeka uzmanısın.
Aşağıdaki yorumlara göre kullanıcı değerlendir.

Analiz Başlıkları:

1) Genel Duygu Durumu (poizitf/negatif/nötr)
2) Toksik içerik var mı? (örnekleriyle)
3) İlgi alanları / konu başlıkları
4) İletişim tarzı (samimi, resmi, agresif vb.)
5) Geliştirilmesi gereken iletişim alanları
6) 5 Maddelik kısa özet

Yorumlar:

{allComments}";


            //OpenAI Chat Completions

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var body = new
            {
                model = "gpt-4o-mini",
                messages = new object[]
                {
                    new {role="system",content="Sen kullanıcı yorum analizi yapan bir uzmansın."},
                    new {role="user",content= prompt }
                },
                max_tokens = 1000,
                temperature = 0.2
            };

            //Json Dönüşümleri

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpReponse = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var respText = await httpReponse.Content.ReadAsStringAsync();

            if (!httpReponse.IsSuccessStatusCode)
            {
                ViewBag.AIResult = "Open AI Hatası: " + httpReponse.StatusCode;
                return View(user);
            }

            //Json Yapı İçinden Veriyi Okuma

            try
            {
                using var doc = JsonDocument.Parse(respText);
                var aitText = doc.RootElement
                               .GetProperty("choices")[0]
                               .GetProperty("message")
                               .GetProperty("content")
                               .GetString();

                ViewBag.AIResult = aitText ?? "Boş yanıt döndü";
            }
            catch
            {
                ViewBag.AIResult = "OpenAI yanıtı beklenen formatta değil.";

            }
            return View(user);
        }

        public async Task<IActionResult> UserProfileWithGeminiAI(string id)
        {
            var value = await _userManager.FindByIdAsync(id);
            ViewBag.name = value.Name;
            ViewBag.surname = value.Surname;
            ViewBag.imageUrl = value.ImageUrl;
            ViewBag.description = value.Description;
            ViewBag.titlevalue = value.Title;
            ViewBag.city = value.City;
            ViewBag.education = value.Education;

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var articles = await _context.Articles
                                    .Where(x => x.AppUserId == id)
                                    .Select(y => y.Content)
                                    .ToListAsync();

            if (articles.Count == 0)
            {
                ViewBag.AIResult = "Bu kullanıcıya ait analiz yapılacak makale bulunamadı!";
                return View(user);
            }

            var allArticles = string.Join("\n\n", articles);
            var apiKey = ""; // <- API Key Girişi

            var prompt = $@"
Siz bir sigorta sektöründe uzman bir içerik analistisin.
Elinizde, bir sigorta şirketinin çalışanının yazdığı tüm makaleler var.
Bu makaleler üzerinden çalışanın içerik üretim tarzını analiz et.

Analiz Başlıkları:

1) Konu çeşitliliği ve odak alanları (sağlık, hayat, kasko, tamamlayıcı, BES vb.)
2) Hedef kitle tahmini (bireysel/kurumsal, segment, persona)
3) Dil ve Anlatım Tarzı (tekniklik seviyesi, okunabilirlik, ikna gücü)
4) Sigorta terimlerini kullanma ve doğruluk düzeyi
5) Müşteri ihtiyaçlarına ve risk yönetimine odaklanma
6) Pazarlama/satış vurgusu, CTA netliği
7) Geliştirilmesi gereken alanlar ve net aksiyon maddeleri

Makaleler:

{allArticles}

Lütfen çıktıyı profesyonel rapor formatında, madde madde ve en sonda 5 maddelik aksiyon listesi ile ver.";


            using var client = new HttpClient();

            // Gemini endpoint
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";

            // Request body (Gemini formatı)
            var body = new
            {
                contents = new object[]
                {
        new {
            role = "user",
            parts = new object[]
            {
                new { text = prompt }
            }
        }
                }
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpResponse = await client.PostAsync(url, content);
            var respText = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
                ViewBag.AIResult = "Gemini API Hatası: " + httpResponse.StatusCode + " - " + respText;
                return View(user);
            }

            try
            {
                using var doc = JsonDocument.Parse(respText);
                var aitText = doc.RootElement
                                .GetProperty("candidates")[0]
                                .GetProperty("content")
                                .GetProperty("parts")[0]
                                .GetProperty("text")
                                .GetString();

                ViewBag.AIResult = aitText ?? "Boş yanıt döndü";
            }
            catch
            {
                ViewBag.AIResult = "Gemini yanıtı beklenen formatta değil.";
            }
            return View(user);
        }

        public async Task<IActionResult> UserCommentsProfileWithGeminiAI(string id)
        {
            var values = await _userManager.FindByIdAsync(id);
            ViewBag.name = values.Name;
            ViewBag.surname = values.Surname;
            ViewBag.imageUrl = values.ImageUrl;
            ViewBag.description = values.Description;
            ViewBag.titlevalue = values.Title;
            ViewBag.city = values.City;
            ViewBag.education = values.Education;

            // Kullanıcı Bilgilerini Çekelim
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Kullanıcıya Ait Yorum Listesi
            var comments = await _context.Comments
                                       .Where(x => x.AppUserId == id)
                                       .Select(y => y.Detail)
                                       .ToListAsync();

            if (comments.Count == 0)
            {
                ViewBag.AIResult = "Bu kullanıcıya ait analiz yapılacak yorum bulunamadı!";
                return View(user);
            }

            var allComments = string.Join("\n\n", comments);
            var apiKey = ""; // <- API Key Girişi

            var prompt = $@"
Sen kullanıcı davranış analizi yapan bir yapay zeka uzmanısın.
Aşağıdaki yorumlara göre kullanıcı değerlendir.

Analiz Başlıkları:

1) Genel Duygu Durumu (pozitif/negatif/nötr)
2) Toksik içerik var mı? (örnekleriyle)
3) İlgi alanları / konu başlıkları
4) İletişim tarzı (samimi, resmi, agresif vb.)
5) Geliştirilmesi gereken iletişim alanları
6) 5 Maddelik kısa özet

Yorumlar:

{allComments}";

            using var client = new HttpClient();

            // Gemini endpoint
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";

            // Gemini body
            var body = new
            {
                contents = new object[]
                {
        new {
            role = "user",
            parts = new object[]
            {
                new { text = prompt }
            }
        }
                }
            };

            // Json dönüşümleri
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpResponse = await client.PostAsync(url, content);
            var respText = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
                ViewBag.AIResult = "Gemini API Hatası: " + httpResponse.StatusCode + " - " + respText;
                return View(user);
            }

            // Json Yapı İçinden Veriyi Okuma
            try
            {
                using var doc = JsonDocument.Parse(respText);
                var aitText = doc.RootElement
                                .GetProperty("candidates")[0]
                                .GetProperty("content")
                                .GetProperty("parts")[0]
                                .GetProperty("text")
                                .GetString();

                ViewBag.AIResult = aitText ?? "Boş yanıt döndü";
            }
            catch
            {
                ViewBag.AIResult = "Gemini yanıtı beklenen formatta değil.";
            }

            return View(user);

        }
    }
}
