using LFG.Data;
using LFG.Interface;
using LFG.Models;
using LFG.Pages.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace LFG.Pages
{
  public class IndexModel : PageModel
  {
    private readonly LFGContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public IndexModel(LFGContext context, IPasswordHasher passwordHasher)
    {
      _context = context;
      _passwordHasher = passwordHasher;
    }

    [BindProperty]
    public User User { get; set; }

    [BindProperty]
    public NewUser NewUser { get; set; }

    public string ErrorMessage;

    public async Task<IActionResult> OnGetAsync()
    {
      if (!HttpContext.User.Identity.IsAuthenticated)
      {
        return Page();
      }

      var foundUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == HttpContext.User.Identity.Name);
      return RedirectToPage("/Profile/Profile", new { foundUser.Username });
    }

    public async Task<IActionResult> OnPostLoginAsync()
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

    public async Task<IActionResult> OnPostRegisterAsync()
    {
      if (!ModelState.IsValid)
      {
        return Page();
      }

      NewUser.Password = _passwordHasher.Hash(NewUser.Password);
      NewUser.Created = DateTime.Now;

      _context.Users.Add(NewUser);
      await _context.SaveChangesAsync();

      return RedirectToPage("./Login");
    }
  }

  public class NewUser : User
  {
    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords don't match. Please try again.")]
    [Display(Name = "Re-type Password")]
    public string ConfirmPassword { get; set; }
  }
}
