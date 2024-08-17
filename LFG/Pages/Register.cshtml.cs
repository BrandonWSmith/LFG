using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using LFG.Data;
using LFG.Models;

namespace LFG.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly LFGContext _context;

        public RegisterModel(LFGContext context)
        {
            _context = context;
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
