using LFG.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LFG.Pages
{
  public class IndexModel : PageModel
  {
    private readonly LFGContext _context;

    public IndexModel(LFGContext context)
    {
      _context = context;
    }

    public async Task<IActionResult> OnGetAsync()
    {
      if (!User.Identity.IsAuthenticated)
      {
        return Page();
      }

      var foundUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == User.Identity.Name);
      return RedirectToPage("/Profile/Profile", new { foundUser.Username });
    }
  }
}
