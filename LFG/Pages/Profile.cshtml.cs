using LFG.Data;
using LFG.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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

    public User User { get; set; }
    public List<Platform> UserPlatforms { get; set; }
    public List<string> UserPlatformNames { get; set; }
    public List<Group> UserGroups { get; set; }

    public async Task OnGetAsync()
    {
      User  = await _context.Users.FirstOrDefaultAsync(u => u.Username == RouteData.Values["username"]);

      UserPlatforms = await _context.UsersPlatforms
        .Where(p => p.UserId == User.Id)
        .Include(p => p.Platform)
        .Select(p => new Platform
        {
          Name = p.Platform.Name
        })
        .ToListAsync();

      UserPlatformNames = UserPlatforms.Select(p => p.Name).ToList();

      UserGroups = await _context.UsersGroups
        .Where(g => g.UserId == User.Id)
        .Include(g => g.Group)
        .Select(g => new Group
        {
          Name = g.Group.Name,
          AvatarId = g.Group.AvatarId
        })
        .ToListAsync();
    }
  }
}