using System.ComponentModel.DataAnnotations;
using LFG.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LFG.Models;

namespace LFG.Pages
{
    public class LoginModel : PageModel
    {
        private readonly LFGContext _context;

        public LoginModel(LFGContext context)
        {
            _context = context;
        }

        public User User { get; set; }

        public void OnGet()
        {

        }

        public void OnPost()
        {

        }
    }
}
