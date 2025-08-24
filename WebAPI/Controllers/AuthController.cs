using System.Text.Json.Serialization;

namespace Web_API.Controllers 
{
    [Route("/api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet("check-auth")]
        public IActionResult CheckAuth()
        {
            return Ok(new { isAuthenticated = true });
        }

        [HttpPost("login")]
        public IActionResult Authenticate([FromBody] Credential credential)
        {
            //驗證Credential
            if (credential.Email == "admin@gmail.com" && credential.Password == "!qaz")
            {
                //Creating the security context
                var claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Name, "admin"),
                                    new Claim(ClaimTypes.Email, "admin@web.com"),
                                    new Claim(ClaimTypes.Role, "Admin"),
                                    new Claim("Admin","true"),
                                    new Claim("Department", "HR"),
                                    new Claim("Manager", "true"),
                                    new Claim("EmploymentDate", "2024-08-01"),
                                };

                var expiresAt = DateTime.UtcNow.AddMinutes(10);
                // 產生 JWT Token
                var token = CreateToken(claims, expiresAt);

                // 返回 Token
                return Ok(new JwtToken
                {
                    AccessToken = token,
                    ExpiresAt = expiresAt
                });

            }

            ModelState.AddModelError("Unauthorized", "您無權限訪問此端點。");
            return Unauthorized(ModelState);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            return Ok(new { message = "登出成功" });
        }
        private string CreateToken(IEnumerable<Claim> claims, DateTime expireAt)
        {
            var secretKeyString = _configuration.GetValue<string>("SecretKey");
            if (string.IsNullOrEmpty(secretKeyString))
            {
                throw new InvalidOperationException("SecretKey 未設定");
            }
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyString));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expireAt,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
    public class Credential
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class JwtToken
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
        [JsonPropertyName("expires_at")]
        public DateTime ExpiresAt { get; set; }
    }
}

