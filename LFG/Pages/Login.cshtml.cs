using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LFG.Models;

namespace LFG.Pages
{
    public class LoginModel : PageModel
    {
        public User User { get; set; }

        public void OnGet()
        {

        }

        public void OnPost()
        {

        }
    }
}
