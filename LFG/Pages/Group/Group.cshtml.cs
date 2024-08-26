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

    public Models.Group Group { get; set; }
    public User Owner { get; set; }
    public List<Game> GroupGames { get; set; }
    public List<Models.Thread> GroupThreads { get; set; }
    public List<Comment> ThreadComments { get; set; }

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

      GroupThreads = await _context.Threads.Where(t => t.GroupId == Group.Id).ToListAsync();
    }

    public async Task<List<Comment>> GetThreadComments(int threadId)
    {
      ThreadComments = await _context.Comments.Where(c => c.ThreadId == threadId).ToListAsync();

      return ThreadComments;
    }

    public string GetPrettyDate(DateTime d)
    {
      // 1.
      // Get time span elapsed since the date.
      TimeSpan s = DateTime.Now.Subtract(d);

      // 2.
      // Get total number of days elapsed.
      int dayDiff = (int)s.TotalDays;

      // 3.
      // Get total number of seconds elapsed.
      int secDiff = (int)s.TotalSeconds;

      // 4.
      // Don't allow out of range values.
      if (dayDiff < 0 || dayDiff >= 31)
      {
        return null;
      }

      // 5.
      // Handle same-day times.
      if (dayDiff == 0)
      {
        // A.
        // Less than one minute ago.
        if (secDiff < 60)
        {
          return "just now";
        }

        // B.
        // Less than 2 minutes ago.
        if (secDiff < 120)
        {
          return "1 minute ago";
        }

        // C.
        // Less than one hour ago.
        if (secDiff < 3600)
        {
          return string.Format("{0} minutes ago",
            Math.Floor((double)secDiff / 60));
        }

        // D.
        // Less than 2 hours ago.
        if (secDiff < 7200)
        {
          return "1 hour ago";
        }

        // E.
        // Less than one day ago.
        if (secDiff < 86400)
        {
          return string.Format("{0} hours ago",
            Math.Floor((double)secDiff / 3600));
        }
      }

      // 6.
      // Handle previous days.
      if (dayDiff == 1)
      {
        return "yesterday";
      }

      if (dayDiff < 7)
      {
        return string.Format("{0} days ago",
          dayDiff);
      }

      if (dayDiff < 31)
      {
        return string.Format("{0} weeks ago",
          Math.Ceiling((double)dayDiff / 7));
      }

      return null;
    }
  }
}