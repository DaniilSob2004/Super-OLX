using System.Security.Claims;

using OnlineClassifieds.DAL.Repository.IRepository;
using OnlineClassifieds.Models;

namespace OnlineClassifieds.Services
{
    // сервис для работы с пользователем
    public class CurrentUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;

        public CurrentUserProvider(
            IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        public string? GetCurrentUserId()
        {
            ClaimsPrincipal? user = _httpContextAccessor?.HttpContext?.User;
            if (user is null) { return null; }

            Claim? userId = user.FindFirst(ClaimTypes.NameIdentifier);
            return userId?.Value;
        }

        public async Task<User?> GetCurrentUser()
        {
            string? userId = GetCurrentUserId();
            if (userId is null) { return null; }

            return await _userRepository.FirstOrDefault(u => u.Id.Equals(userId));
        }
    }
}
