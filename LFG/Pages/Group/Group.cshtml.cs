using LFG.Data;
using LFG.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LFG.Pages.Group
{
    public class GroupModel : PageModel
    {
        private readonly LFGContext _context;

        public GroupModel(LFGContext context)
        {
          _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public Models.Group Group { get; set; }

        [BindProperty(SupportsGet = true)]
        public User Owner { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<Game> GroupGames { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool ShowGames { get; set; }

        public async Task OnGetAsync()
        {
          Group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == RouteData.Values["groupname"]);
          Owner = await _context.Users.FirstOrDefaultAsync(u => u.Id == Group.Owner);

          GroupGames = await _context.GroupsGames
            .Where(g => g.GroupId == Group.Id)
            .Include(g => g.Game)
            .Select(g => new Models.Game
            {
              Name = g.Game.Name,
              CoverId = g.Game.CoverId
            })
            .ToListAsync();
        }

        public async Task OnPostToggleGames()
        {
          ShowGames = ShowGames == false;
          Group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == RouteData.Values["groupname"]);
          Owner = await _context.Users.FirstOrDefaultAsync(u => u.Id == Group.Owner);

          GroupGames = await _context.GroupsGames
            .Where(g => g.GroupId == Group.Id)
            .Include(g => g.Game)
            .Select(g => new Models.Game
            {
              Name = g.Game.Name,
              CoverId = g.Game.CoverId
            })
            .ToListAsync();
        }
  }
}
