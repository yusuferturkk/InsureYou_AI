using Microsoft.AspNetCore.SignalR;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InsureYou_AI.Models
{
    public class ChatHub : Hub
    {
        private const string apiKey = "AIzaSyD6zszUqNrfPnTsmiJWqL0WgeZed4PdA9Q";
        private const string modelGemini = "gemini-1.5-pro";

        private readonly IHttpClientFactory _httpClientFactory;

        public ChatHub(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private static readonly Dictionary<string, List<Dictionary<string, object>>> _history = new();

        public override Task OnConnectedAsync()
        {
            _history[Context.ConnectionId] = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    ["role"]= "system",
                    ["parts"] = new object[]
                    {
                        new { text = "You are a helpful assistant. Keep answers concise." }
                    }
                }
            };

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _history.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message)
        {
            await Clients.Caller.SendAsync("ReceiveUserEcho", message);

            var history = _history[Context.ConnectionId];
            history.Add(new Dictionary<string, object>
            {
                ["role"] = "user",
                ["parts"] = new object[]
                {
                new { text = message }
                }
            });

            await StreamGemini(history, Context.ConnectionAborted);
        }

        private async Task StreamGemini(List<Dictionary<string, object>> history, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient();

            var payload = new
            {
                contents = history
            };

            var url = $"https://generativelanguage.googleapis.com/v1/models/{modelGemini}:streamGenerateContent?key={apiKey}";

            using var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using var resp = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            resp.EnsureSuccessStatusCode();

            using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            var sb = new StringBuilder();
            while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (!line.StartsWith("data:")) continue;

                var data = line["data:".Length..].Trim();
                if (data == "[DONE]") break;

                try
                {
                    using var doc = JsonDocument.Parse(data);
                    var text = doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();

                    if (!string.IsNullOrEmpty(text))
                    {
                        sb.Append(text);
                        await Clients.Caller.SendAsync("ReceiveToken", text, cancellationToken);
                    }
                }
                catch
                {
                    // chunk parse hatası
                }
            }

            var full = sb.ToString();
            history.Add(new Dictionary<string, object>
            {
                ["role"] = "assistant",
                ["parts"] = new object[]
                {
                new { text = full }
                }
            });

            await Clients.Caller.SendAsync("CompleteMessage", full, cancellationToken);
        }
    }
}
