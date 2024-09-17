using LFG.Data;
using LFG.Interface;
using LFG.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace LFG.Pages.Account
{
  public class RegisterModel : PageModel
  {
    private readonly LFGContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterModel(LFGContext context, IPasswordHasher passwordHasher)
    {
      _context = context;
      _passwordHasher = passwordHasher;
    }

    public IActionResult OnGet()
    {
      return Page();
    }

    [BindProperty]
    public NewUser User { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!ModelState.IsValid)
      {
        return Page();
      }

      User.Password = _passwordHasher.Hash(User.Password);
      User.Created = DateTime.Now;

      _context.Users.Add(User);
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
