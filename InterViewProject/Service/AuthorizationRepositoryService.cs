namespace InterViewProject.Serverice
{
    public class AuthorizationRepositoryService : IAuthorizationRepository
    {
        private readonly CoffeeContext _dbContext;
        private readonly ILogger<AuthorizationRepositoryService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthorizationRepositoryService(CoffeeContext dbContext, ILogger<AuthorizationRepositoryService> logger, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> CheckAccount(Credential credential)
        {
            var target = await _dbContext.Admins.FirstOrDefaultAsync(x => x.Email == credential.Email);
            if (target is not null)
            {
                if (target.Password == credential.Password)
                {
                    await GetJwtToken(credential);
                    return true;
                }
                return false;
            }
            return false;
        }

        private async Task<bool> GetJwtToken(Credential credential)
        {
            var httpClient = _httpClientFactory.CreateClient("OurWebAPI");
            var res = await httpClient.PostAsJsonAsync("/api/auth/login", new Credential { Email = credential.Email, Password = credential.Password });
            res.EnsureSuccessStatusCode();
            var jwtToken = await res.Content.ReadFromJsonAsync<JwtToken>();


            if (jwtToken != null && !string.IsNullOrEmpty(jwtToken.AccessToken))
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = jwtToken.ExpiresAt,
                    SameSite = SameSiteMode.Strict
                };

                // 使用 HttpContext 設置 Cookie
                _httpContextAccessor.HttpContext?.Response.Cookies.Append("access_token", jwtToken.AccessToken, cookieOptions);

                return true;
            }
            return false;
        }

        public Task LogOut()
        {
            // 清除 access_token Cookie
            if (_httpContextAccessor.HttpContext != null)
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Delete("access_token");
                _logger.LogInformation("用戶登出，已清除認證 Cookie");
            }
            return Task.CompletedTask;
        }
    }
}
