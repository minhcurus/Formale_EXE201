using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using Infrastructure.Repository;

namespace Application.Service
{
    public class OpenRouterService : IOpenRouterService
    {
        private readonly HttpClient _http;
        private readonly ProductStyleRepository _styleRepo; // chỉ giữ lại repo
        private readonly IUserService _userService;
        private readonly CurrentUserService _currentUser;

        public OpenRouterService(IHttpClientFactory factory,
                                 ProductStyleRepository styleRepo,
                                  IUserService userService,
                                   CurrentUserService currentUserService)
        {
            _http = factory.CreateClient("OpenRouter");
            _styleRepo = styleRepo;
            _userService = userService;
            _currentUser = currentUserService;
        }

        /// <summary>Trả về đúng 1 style (string) hoặc null</summary>
        public async Task<string?> ClassifyAsync(string prompt)
        {
            var user = await _userService.GetUsersById(_currentUser.UserId.Value);
            if (user.PremiumPackageId == null
                || user.PremiumExpiryDate < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Bạn cần nâng cấp gói để sử dụng tính năng này.");
            }

            var styles = _styleRepo.Query()
                               .Select(s => s.StyleName)
                               .ToArray(); // gọi ở đây, EF context đã sẵn sàng
            var body = new
            {
                model = "deepseek/deepseek-r1-0528-qwen3-8b:free",
                messages = new[]
                {
                    //new { role = "system", content = $"You are a fashion assistant. Return only 1 of: {string.Join(", ", styles)}." },
                    new { role = "system", content = $"You are a fashion assistant. Based on the following user description, return only one fashion style from this list: {string.Join(", ", styles)}. The returned value must be exactly one style from the list, nothing else, no explanation, no null.Analyze the user’s preferences, occasion, personality, and tone to match the most suitable style." },
                    new { role = "user",   content = prompt }
                }
            };

            var res = await _http.PostAsJsonAsync("api/v1/chat/completions", body);
            var contentString = await res.Content.ReadAsStringAsync();

            //Console.WriteLine("Response raw content: " + contentString);
            res.EnsureSuccessStatusCode();

            var json = await res.Content.ReadFromJsonAsync<OpenRouterResponse>();
            return json?.choices?[0]?.message?.content?.Trim();
        }

        private record OpenRouterResponse(OpenRouterChoice[] choices);
        private record OpenRouterChoice(OpenRouterMessage message);
        private record OpenRouterMessage(string content);

        //private class OpenRouterResponse
        //{
        //    public OpenRouterChoice[]? choices { get; set; }
        //}

        //private class OpenRouterChoice
        //{
        //    public OpenRouterMessage? message { get; set; }
        //}

        //private class OpenRouterMessage
        //{
        //    public string? content { get; set; }
        //}

    }
}
