using LFG.Data;
using LFG.Hubs;
using LFG.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LFG.Pages.Group
{
  [Authorize(Policy = "Registered")]
  [BindProperties(SupportsGet = true)]
  public class GameGroupsModel : PageModel
  {
    private readonly LFGContext _context;
    private readonly IHubContext<GroupSearchHub> _groupSearchHubContext;

    public GameGroupsModel(LFGContext context, IHubContext<GroupSearchHub>? groupSearchHubContext)
    {
      _context = context;
      if (groupSearchHubContext != null) _groupSearchHubContext = groupSearchHubContext;
    }

    public Game Game { get; set; }
    public Company GameDeveloper { get; set; }
    public Company GamePublisher { get; set; }
    public List<Platform> GamePlatforms { get; set; }
    public List<string> GamePlatformNames { get; set; }
    public List<Models.Group> GameGroups { get; set; }
    public Models.Group Group { get; set; }
    public string? SelectedGroup { get; set; }
    public string? CreateGroupErrorMessage { get; set; } = null;

    public async Task OnGetAsync()
    {
      Game = await _context.Games.FirstOrDefaultAsync(g => g.Name == RouteData.Values["game"]);
      GameDeveloper = await _context.GamesDevelopers
        .Where(d => d.GameId == Game.Id)
        .Include(d => d.Company)
        .Select(d => new Company
        {
          Name = d.Company.Name,
          LogoId = d.Company.LogoId
        })
        .SingleAsync();
      GamePublisher = await _context.GamesPublishers
        .Where(p => p.GameId == Game.Id)
        .Include(p => p.Company)
        .Select(p => new Company
        {
          Name = p.Company.Name,
          LogoId = p.Company.LogoId
        })
        .SingleAsync();
      GamePlatforms = await _context.GamesPlatforms
        .Where(p => p.GameId == Game.Id)
        .Include(p => p.Platform)
        .Select(p => new Platform
        {
          Name = p.Platform.Name
        })
        .ToListAsync();
      GamePlatformNames = GamePlatforms.Select(p => p.Name).ToList();

      GameGroups = await _context.GroupsGames
        .Where(g => g.GameId == Game.Id)
        .Include(g => g.Group)
        .Select(g => new Models.Group
        {
          Name = g.Group.Name,
          Description = g.Group.Description,
          Public = g.Group.Public,
          AvatarId = g.Group.AvatarId
        })
        .ToListAsync();
    }

    public async Task OnPostGroupSearch()
    {
      Game = await _context.Games.FirstOrDefaultAsync(g => g.Name == RouteData.Values["game"]);
      GameGroups = await _context.GroupsGames
        .Where(g => g.GameId == Game.Id)
        .Include(g => g.Group)
        .Select(g => new Models.Group
        {
          Name = g.Group.Name,
          Description = g.Group.Description,
          Public = g.Group.Public,
          AvatarId = g.Group.AvatarId
        })
        .ToListAsync();

      await _groupSearchHubContext.Clients.All.SendAsync("groupSearch", GameGroups, SelectedGroup);
    }

    public async Task<IActionResult> OnPostCreateGroup()
    {
      ModelState.MaxAllowedErrors = 4;
      if (ModelState.HasReachedMaxErrors)
      {
        CreateGroupErrorMessage = "Fields are invalid. Please try again.";
        await OnGetAsync();
        return Page();
      }

      Group.Owner = await _context.Users.Where(u => u.Username == User.Identity.Name).Select(u => u.Id).SingleAsync();
      Group.Created = DateTime.Now;

      await _context.Groups.AddAsync(Group);
      await _context.SaveChangesAsync();

      var newGroup = await _context.Groups.FirstOrDefaultAsync(g => g.Name == Group.Name);
      Game = await _context.Games.FirstOrDefaultAsync(g => g.Name == RouteData.Values["game"]);
      var newGroupGame = new GroupGame
      {
        GroupId = newGroup.Id,
        GameId = Game.Id
      };
      var newUserGroup = new UserGroup
      {
        UserId = Group.Owner,
        GroupId = newGroup.Id,
        Rank = 1,
        Role = Enums.GroupRole.Owner
      };

      await _context.GroupsGames.AddAsync(newGroupGame);
      await _context.UsersGroups.AddAsync(newUserGroup);
      await _context.SaveChangesAsync();

      return RedirectToPage("/Group/Group", new { groupname = newGroup.Name });
    }
  }
}
