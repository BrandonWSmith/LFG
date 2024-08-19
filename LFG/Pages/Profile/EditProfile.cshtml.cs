using LFG.Data;
using LFG.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;

namespace LFG.Pages.Profile
{
  [Authorize(Policy = "Registered")]
  public class EditProfileModel : PageModel
  {
    private readonly LFGContext _context;

    public EditProfileModel(LFGContext context)
    {
      _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public User User { get; set; }

    [BindProperty(SupportsGet = true)]
    public List<Platform> UserPlatforms { get; set; }

    [BindProperty(SupportsGet = true)]
    public List<string> UserPlatformNames { get; set; }

    public SelectList AllPlatformsList { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SelectedPlatform { get; set; }

    public async Task OnGetAsync()
    {
      User = await _context.Users.FirstOrDefaultAsync(u => u.Username == RouteData.Values["username"]);

      UserPlatforms = await _context.UsersPlatforms
        .Where(p => p.UserId == User.Id)
        .Include(p => p.Platform)
        .Select(p => new Platform
        {
          Name = p.Platform.Name
        })
        .ToListAsync();

      UserPlatformNames = UserPlatforms.Select(p => p.Name).ToList();

      var allPlatforms = _context.Platforms.Select(p => p.Name);

      AllPlatformsList = new SelectList(await allPlatforms.ToListAsync());
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (string.IsNullOrEmpty(SelectedPlatform))
      {
        return Page();
      }

      User = await _context.Users.FirstOrDefaultAsync(u => u.Username == HttpContext.User.Identity.Name);

      var platformId = await _context.Platforms
        .Where(p => p.Name == SelectedPlatform)
        .Select(p => p.Id)
        .SingleAsync();

      var platformToAdd = new UserPlatform
      {
        UserId = User.Id,
        PlatformId = platformId
      };

      await _context.UsersPlatforms.AddAsync(platformToAdd);

      await _context.SaveChangesAsync();
      return Page();
    }
  }
}