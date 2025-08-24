
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InterViewProject.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; } = new Credential();

        private readonly ILogger<LoginModel> _logger;
        private readonly IAuthorizationRepository _authorizationRepository;
        public LoginModel(ILogger<LoginModel> logger, IAuthorizationRepository authorizationRepository)
        {
            _logger = logger;
            _authorizationRepository = authorizationRepository;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {

                return Page();
            }
            var result = await _authorizationRepository.CheckAccount(Credential);
            if (result)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                // 登入失敗，添加錯誤訊息
                ModelState.AddModelError(string.Empty, "登入失敗，請檢查您的帳號和密碼。");
                return Page();
            }
        }
    }
}
