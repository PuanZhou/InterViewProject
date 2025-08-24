using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InterViewProject.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly ILogger<LogoutModel> _logger;
        public LogoutModel(ILogger<LogoutModel> logger, IAuthorizationRepository authorizationRepository)
        {
            _logger = logger;
            _authorizationRepository = authorizationRepository;
        }
        public IActionResult OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await _authorizationRepository.LogOut();
            return RedirectToPage("/Index");
        }
    }
}
