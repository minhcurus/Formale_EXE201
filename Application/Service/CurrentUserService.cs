using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.Service
{
    public class CurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public int? UserId => int.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;

        public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;
        public string? FullName => User?.FindFirst("full_name")?.Value;
        public string? UserName => User?.FindFirst("username")?.Value;
        public string? PhoneNumber => User?.FindFirst(ClaimTypes.MobilePhone)?.Value;
        public string? Address => User?.FindFirst("address")?.Value;
        public string? Role => User?.FindFirst(ClaimTypes.Role)?.Value;
        public string? DateOfBirth => User?.FindFirst("dob")?.Value;
        public string? ImageUser => User?.FindFirst("image_user")?.Value;
        public string? BackgroundImage => User?.FindFirst("background_image")?.Value;
        public string? Description => User?.FindFirst("description")?.Value;
    }
}
