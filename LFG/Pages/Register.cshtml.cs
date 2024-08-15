using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using LFG.Models;

namespace LFG.Pages
{
    public class RegisterModel : PageModel
    {
        public NewUser User { get; set; }

        public void OnGet()
        {
        }
    }

    public class NewUser : User
    {
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
