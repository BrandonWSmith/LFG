using LFG.Data;
using LFG.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LFG.Pages
{
  [Authorize(Policy = "Registered")]
  public class ProfileModel : PageModel
  {
    private readonly LFGContext _context;

    public ProfileModel(LFGContext context)
    {
      _context = context;
    }

    [BindProperty(SupportsGet = true)] public User User { get; set; }

    public void OnGet()
    {
    }
  }
}