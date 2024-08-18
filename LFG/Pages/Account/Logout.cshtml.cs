using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LFG.Pages.Account
{
  public class LogoutModel : PageModel
  {
    public async Task<IActionResult> OnPostAsync()
    {
      HttpContext.SignOutAsync("CookieAuth");
      return RedirectToPage("/Index");
    }
  }
}
