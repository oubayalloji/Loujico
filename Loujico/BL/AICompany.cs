using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Loujico.Models;
using Google.GenAI;

using Google.GenAI.Types;

namespace Loujico.BL
{
    public record CompanyIDDto
    {
        public int id { get; init; }
        public string name { get; init; } = string.Empty;
    }

public interface IGeminiService
    {
        Task<string> GenerateAsync(string prompt);
    }

    public class GeminiService : IGeminiService
    {
        private readonly Client _client;

        public GeminiService(IConfiguration config)
        {
            var apiKey = config["Gemini:ApiKey"];
            // Client يفضل أن يكون Singleton إذا كانت المكتبة تدعم ذلك، لكن هنا لا بأس
            _client = new Client(apiKey: apiKey);
        }
      
    public async Task<string> GenerateAsync(string prompt)
        {
            int maxRetries = 3; // عدد محاولات الإعادة
            int delay = 1000; // الانتظار المبدئي 1 ثانية

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    // تأكد من استخدام موديل خفيف وسريع مثل flash
                    // gemini-1.5-flash هو الأفضل حالياً لهذه المهام
                    var config = new GenerateContentConfig
                    {
                     
                        Temperature = 0.0f, // لضمان عدم الهلوسة في الحروف العربية
                        ResponseMimeType = "application/json" // إجبار الموديل على إرجاع JSON
                    };
                    var response = await _client.Models.GenerateContentAsync(
                        model: "gemini-2.5-flash",
                        contents: prompt,
                        config:config
                        
                    );

                    var text = response?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
                    if (!string.IsNullOrEmpty(text)) return text;
                }
                catch (Exception ex)
                {
                    // إذا كان الخطأ هو Overloaded وصلنا لآخر محاولة
                    if (i == maxRetries - 1) throw; // ارمي الخطأ للكنترولر ليعالجه

                    // إذا لم تكن المحاولة الأخيرة، انتظر قليلاً ثم جرب
                    await Task.Delay(delay * (i + 1)); // Exponential Backoff (1s -> 2s -> 3s)
                }
            }

            throw new Exception("Failed to get response from Gemini after retries.");
        }

        private string CleanJson(string json)
        {
            if (json.StartsWith("```json"))
            {
                json = json.Replace("```json", "").Replace("```", "");
            }
            else if (json.StartsWith("```"))
            {
                json = json.Replace("```", "");
            }
            return json.Trim();
        }
    }

    }
