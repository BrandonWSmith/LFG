using System.ComponentModel.DataAnnotations;
using LFG.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LFG.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace LFG.Pages
{
    public class LoginModel : PageModel
    {
        private readonly LFGContext _context;

        public LoginModel(LFGContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public User User { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            /*if (!ModelState.IsValid)
            {
                return Page();
            }*/

            if (User.Username == "Brandon" && User.Password == "Password1")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Brandon"),
                    new Claim(ClaimTypes.Email, "bwsmith09@gmail.com")
                };
                var identity = new ClaimsIdentity(claims, "CookieAuth");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("CookieAuth", claimsPrincipal);

                return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}
