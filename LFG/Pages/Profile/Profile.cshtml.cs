using LFG.Data;
using LFG.Enums;
using LFG.Hubs;
using LFG.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace LFG.Pages.Profile
{
  [Authorize(Policy = "Registered")]
  [BindProperties(SupportsGet = true)]
  public class ProfileModel : PageModel
  {
    private readonly LFGContext _context;
    private readonly IHubContext<ProfileHub> _profileHubContext;

    public ProfileModel(LFGContext context, IHubContext<ProfileHub>? profileHubContext)
    {
      _context = context;
      if (profileHubContext != null) _profileHubContext = profileHubContext;
    }

    public User User { get; set; }
    public List<Message> UserMessages { get; set; }
    public List<Platform> UserPlatforms { get; set; }
    public List<string> UserPlatformNames { get; set; }
    public SortedList<int, GroupRole> UserGroupRoles { get; set; }
    public List<Models.Group> UserGroups { get; set; }

    [BindNever]
    public SelectList AllPlatformsList { get; set; }
    public string? SelectedPlatform { get; set; }
    public string? PlatformToRemove { get; set; }
    public string UpdateMessage { get; set; }
    public string PlatformExists { get; set; }

    public async Task OnGetAsync()
    {
      User = await _context.Users.FirstOrDefaultAsync(u => u.Username == RouteData.Values["username"]);
      UserMessages = await _context.Messages.Where(m => m.UserId == User.Id).ToListAsync();

      UserPlatforms = await _context.UsersPlatforms
        .Where(p => p.UserId == User.Id)
        .Include(p => p.Platform)
        .Select(p => new Platform
        {
          Name = p.Platform.Name
        })
        .ToListAsync();
      UserPlatformNames = UserPlatforms.Select(p => p.Name).ToList();

      var usersGroups = _context.UsersGroups.Where(g => g.UserId == User.Id);
      usersGroups.ToList().ForEach(group =>
      {
        UserGroupRoles.Add(group.GroupId, group.Role);
      });
      UserGroups = await usersGroups
        .OrderBy(g => g.Role)
        .Include(g => g.Group)
        .Select(g => new Models.Group
        {
          Id = g.Group.Id,
          Name = g.Group.Name,
          Owner = g.Group.Owner,
          AvatarId = g.Group.AvatarId
        })
        .ToListAsync();

      AllPlatformsList = new SelectList(_context.Platforms.Select(p => p.Name).ToList());
    }

    public async Task OnPostUpdateUserInfo()
    {
      //Populate Non-Editable Fields
      User.Id = await _context.Users.Where(u => u.Username == RouteData.Values["username"]).Select(u => u.Id).SingleAsync();
      User.Username = await _context.Users.Where(u => u.Username == RouteData.Values["username"]).Select(u => u.Username).SingleAsync();
      User.Password = await _context.Users.Where(u => u.Username == RouteData.Values["username"]).Select(u => u.Password).SingleAsync();
      User.Score = await _context.Users.Where(u => u.Username == RouteData.Values["username"]).Select(u => u.Score).SingleAsync();
      User.Created = await _context.Users.Where(u => u.Username == RouteData.Values["username"]).Select(u => u.Created).SingleAsync();
      User.Persistent = await _context.Users.Where(u => u.Username == RouteData.Values["username"]).Select(u => u.Persistent).SingleAsync();

      //Update User Info
      _context.Users.Attach(User).State = EntityState.Modified;
      await _context.SaveChangesAsync();

      UpdateMessage = "Profile Updated";

      await _profileHubContext.Clients.All.SendAsync("updateUserInfo", User.Id);
    }

    public async Task OnPostAddPlatform()
    {
      if (string.IsNullOrEmpty(SelectedPlatform))
      {
        return;
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

      if (!_context.UsersPlatforms.Contains(platformToAdd))
      {
        await _context.UsersPlatforms.AddAsync(platformToAdd);
        await _context.SaveChangesAsync();
      }
      else
      {
        PlatformExists = "Platform already added";
      }

      await _profileHubContext.Clients.All.SendAsync("updateEditUserInfo", User.Id, PlatformExists);
    }

    public async Task OnPostRemovePlatform()
    {
      if (string.IsNullOrEmpty(PlatformToRemove))
      {
        return;
      }

      User = await _context.Users.FirstOrDefaultAsync(u => u.Username == HttpContext.User.Identity.Name);

      var platformToRemoveName = Request.Form["PlatformToRemove"].ToString();

      var platformId = await _context.Platforms
        .Where(p => p.Name == platformToRemoveName)
        .Select(p => p.Id)
        .SingleAsync();

      var platformToRemove = new UserPlatform
      {
        UserId = User.Id,
        PlatformId = platformId
      };

      if (_context.UsersPlatforms.Contains(platformToRemove))
      {
        _context.UsersPlatforms.Remove(platformToRemove);
        await _context.SaveChangesAsync();
      }
      else
      {
        PlatformExists = "Platform has not been added";
      }

      await _profileHubContext.Clients.All.SendAsync("updateEditUserInfo", User.Id, PlatformExists);
    }
  }
}