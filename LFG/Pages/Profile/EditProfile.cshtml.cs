using System.Runtime.InteropServices.JavaScript;
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
    public string? SelectedPlatform { get; set; }

    [BindProperty]
    public string? PlatformToRemove { get; set; }

    [ViewData]
    public string UpdateMessage { get; set; }

    [ViewData]
    public string InvalidMessage { get; set; }

    [ViewData]
    public string PlatformExists { get; set; }

    public async Task OnGetAsync()
    {
      User = await _context.Users.FirstOrDefaultAsync(u => u.Username == RouteData.Values["username"]);

      RepopulateSelectList();
    }

    public async Task OnPostAsync()
    {
      //Update User Info
      if (!ModelState.IsValid)
      {
        InvalidMessage = "Invalid fields. Please try again.";
        RepopulateSelectList();
        return;
      }
      _context.Users.Attach(User).State = EntityState.Modified;
      await _context.SaveChangesAsync();

      UpdateMessage = "Profile Updated";

      //Repopulate SelectList
      RepopulateSelectList();
    }

    public void OnPostAddPlatform()
    {
      //Add Platform
      if (string.IsNullOrEmpty(SelectedPlatform))
      {
        RepopulateSelectList();
        return;
      }

      User = _context.Users.FirstOrDefault(u => u.Username == HttpContext.User.Identity.Name);

      var platformId = _context.Platforms
        .Where(p => p.Name == SelectedPlatform)
        .Select(p => p.Id)
        .Single();

      var platformToAdd = new UserPlatform
      {
        UserId = User.Id,
        PlatformId = platformId
      };

      if (!_context.UsersPlatforms.Contains(platformToAdd))
      {
        _context.UsersPlatforms.Add(platformToAdd);
        _context.SaveChanges();
      }
      else
      {
        PlatformExists = "Platform already added";
      }

      //Repopulate SelectList
      RepopulateSelectList();
    }

    public void OnPostRemovePlatform()
    {
      //Remove Platform
      if (string.IsNullOrEmpty(PlatformToRemove))
      {
        RepopulateSelectList();
        return;
      }

      User = _context.Users.FirstOrDefault(u => u.Username == HttpContext.User.Identity.Name);

      var platformToRemoveName = Request.Form["PlatformToRemove"].ToString();

      var platformId = _context.Platforms
        .Where(p => p.Name == platformToRemoveName)
        .Select(p => p.Id)
        .Single();

      var platformToRemove = new UserPlatform
      {
        UserId = User.Id,
        PlatformId = platformId
      };

      if (_context.UsersPlatforms.Contains(platformToRemove))
      {
        _context.UsersPlatforms.Remove(platformToRemove);
        _context.SaveChanges();
      }
      else
      {
        PlatformExists = "Platform has not been added";
      }

      //Repopulate SelectList
      RepopulateSelectList();
    }

    private void RepopulateSelectList()
    {
      //Get User's Platforms as Platform objects
      UserPlatforms = _context.UsersPlatforms
        .Where(p => p.UserId == User.Id)
        .Include(p => p.Platform)
        .Select(p => new Platform
        {
          Name = p.Platform.Name
        })
        .ToList();

      //Get names of Platforms in UserPlatforms
      UserPlatformNames = UserPlatforms.Select(p => p.Name).ToList();

      //Get names of all Platforms
      AllPlatformsList = new SelectList(_context.Platforms.Select(p => p.Name).ToList());
    }
  }
}