namespace OnlineClassifieds.Services
{
    public interface ICookieService
    {
        string? GetCookie(string key);
        void SetCookie(string key, string value, DateTimeOffset? expireTime = null);
        void RemoveCookie(string key);
    }

    public class CookieService : ICookieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetCookie(string key)
        {
            return _httpContextAccessor?.HttpContext?.Request.Cookies[key];
        }

        public void SetCookie(string key, string value, DateTimeOffset? expireTime = null)
        {
            // если значение в куки нет или куки изменены, то обновляем
            string? oldValue = GetCookie(key);
            if (oldValue is null || !oldValue.Equals(value))
            {
                var cookieOptions = new CookieOptions()
                {
                    Path = "/",
                    HttpOnly = true,
                    IsEssential = true
                };
                cookieOptions.Expires = expireTime is null ? DateTime.Now.AddSeconds(30) : expireTime;
                _httpContextAccessor?.HttpContext?.Response.Cookies.Append(key, value, cookieOptions);
            }
        }

        public void RemoveCookie(string key)
        {
            _httpContextAccessor?.HttpContext?.Response.Cookies.Delete(key);
        }
    }
}
