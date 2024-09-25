using LFG.Data;
using LFG.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LFG.Pages.Group
{
  [Authorize(Policy = "Registered")]
  public class GroupMembersModel : PageModel
  {
    public readonly LFGContext _context;

    public GroupMembersModel(LFGContext context)
    {
      _context = context;
    }

    public Models.Group Group { get; set; }
    public List<UserGroup> GroupMembers { get; set; }
    public SortedList<int, string> Usernames { get; set; } = [];

    public async Task OnGetAsync()
    {
      Group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == RouteData.Values["groupname"]);
      GroupMembers = await _context.UsersGroups.Where(g => g.GroupId == Group.Id).ToListAsync();
      GroupMembers.ForEach(m =>
      {
        var username = _context.Users.Where(u => u.Id == m.UserId).Select(u => u.Username).Single();
        Usernames.Add(m.UserId, username);
      });
    }
  }
}
