using LFG.Data;
using LFG.Models;
using LFG.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LFG.Pages.Account
{
  public class LoginModel : PageModel
  {
    private readonly LFGContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public LoginModel(LFGContext context, IPasswordHasher passwordHasher)
    {
      _context = context;
      _passwordHasher = passwordHasher;
    }

    public IActionResult OnGet()
    {
      return Page();
    }

    [BindProperty]
    public User User { get; set; }

    public string ErrorMessage;

    public async Task<IActionResult> OnPostAsync()
    {
      var foundUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == User.Username);

      if (foundUser == null)
      {
        ErrorMessage = "Username not found. Please try again.";
        return Page();
      }

      if (!_passwordHasher.Verify(foundUser.Password, User.Password))
      {
        ErrorMessage = "Invalid password. Please try again.";
        return Page();
      }

      var claims = new List<Claim>
      {
        new (ClaimTypes.Name, foundUser.Username),
        new (ClaimTypes.Email, foundUser.Email)
      };
      var identity = new ClaimsIdentity(claims, "CookieAuth");
      var claimsPrincipal = new ClaimsPrincipal(identity);

      var authProperties = new AuthenticationProperties
      {
        IsPersistent = User.Persistent
      };

      await HttpContext.SignInAsync("CookieAuth", claimsPrincipal, authProperties);

      return RedirectToPage("/Profile/Profile", new { foundUser.Username });
    }
  }
}
