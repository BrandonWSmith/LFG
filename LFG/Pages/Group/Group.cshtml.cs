using LFG.Data;
using LFG.Enums;
using LFG.Hubs;
using LFG.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LFG.Pages.Group
{
  [Authorize(Policy = "Registered")]
  public class GroupModel : PageModel
  {
    private readonly LFGContext _context;
    private readonly IHubContext<ThreadVoteHub> _threadVoteHubContext;
    private readonly IHubContext<CommentVoteHub> _commentVoteHubContext;

    public GroupModel(LFGContext context, IHubContext<ThreadVoteHub> threadVoteHubContext, IHubContext<CommentVoteHub> commentVoteHubContext)
    {
      _context = context;
      _threadVoteHubContext = threadVoteHubContext;
      _commentVoteHubContext = commentVoteHubContext;
    }

    [BindProperty(SupportsGet = true)]
    public User User { get; set; }

    [BindProperty(SupportsGet = true)]
    public UserGroup? UserGroup { get; set; }

    [BindProperty(SupportsGet = true)]
    public GroupRole UserRole { get; set; }

    [BindProperty(SupportsGet = true)]
    public Models.Group Group { get; set; }

    [BindProperty(SupportsGet = true)]
    public User Owner { get; set; }

    [BindProperty(SupportsGet = true)]
    public List<Game> GroupGames { get; set; }

    [BindProperty(SupportsGet = true)]
    public List<Models.Thread> GroupThreads { get; set; }

    [BindProperty(SupportsGet = true)]
    public List<Comment> ThreadComments { get; set; }

    public async Task OnGetAsync()
    {
      Group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == RouteData.Values["groupname"]);
      Owner = await _context.Users.FirstOrDefaultAsync(u => u.Id == Group.Owner);
      User = await _context.Users.FirstOrDefaultAsync(u => u.Username == HttpContext.User.Identity.Name);
      UserGroup = await _context.UsersGroups.FirstOrDefaultAsync(u => u.UserId == User.Id && u.GroupId == Group.Id);

      if (UserGroup != null)
      {
        UserRole = UserGroup.Role;
      }

      GroupGames = await _context.GroupsGames
        .Where(g => g.GroupId == Group.Id)
        .Include(g => g.Game)
        .Select(g => new Models.Game
        {
          Name = g.Game.Name,
          CoverId = g.Game.CoverId
        })
        .ToListAsync();

      GroupThreads = await _context.Threads.Where(t => t.GroupId == Group.Id).OrderByDescending(t => t.Pinned == true).ThenByDescending(t => t.Created).ThenBy(t => t.Id).ToListAsync();
    }

    public async Task<IActionResult> OnPostJoin()
    {
      Group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == RouteData.Values["groupname"]);
      User = await _context.Users.FirstOrDefaultAsync(u => u.Username == HttpContext.User.Identity.Name);
      UserGroup = await _context.UsersGroups.FirstOrDefaultAsync(u => u.UserId == User.Id && u.GroupId == Group.Id);

      if (UserGroup != null) return Page();
      
      await _context.UsersGroups.AddAsync(new UserGroup
      {
        UserId = User.Id,
        GroupId = Group.Id,
        Rank = 1,
        Role = GroupRole.Member
      });

      await _context.SaveChangesAsync();

      return RedirectToPage();
    }

    public async Task<List<Comment>> GetThreadComments(int threadId)
    {
      ThreadComments = await _context.Comments.Where(c => c.ThreadId == threadId).OrderByDescending(c => c.Created).ThenBy(c => c.Id).ToListAsync();

      return ThreadComments;
    }

    public async Task OnPostUpvoteThread(int threadId)
    {
      User = await _context.Users.FirstOrDefaultAsync(u => u.Username == HttpContext.User.Identity.Name);
      var thread = await _context.Threads.FirstOrDefaultAsync(t => t.Id == threadId);
      var poster = await _context.Users.FirstOrDefaultAsync(u => u.Id == thread.UserId);

      if (thread.HasUpVoted.Contains(User.Id))
      {
        await _threadVoteHubContext.Clients.All.SendAsync("upvoteThread", thread.Rating, thread.Id);
        return;
      }
      if (thread.HasDownVoted.Contains(User.Id))
      {
        thread.HasDownVoted.Remove(User.Id);
        thread.Rating++;
        poster.Score++;
      }

      thread.Rating++;
      poster.Score++;
      thread.HasUpVoted.Add(User.Id);

      await _context.SaveChangesAsync();

      await _threadVoteHubContext.Clients.All.SendAsync("upvoteThread", thread.Rating, thread.Id);
    }

    public async Task OnPostDownvoteThread(int threadId)
    {
      User = await _context.Users.FirstOrDefaultAsync(u => u.Username == HttpContext.User.Identity.Name);
      var thread = await _context.Threads.FirstOrDefaultAsync(t => t.Id == threadId);
      var poster = await _context.Users.FirstOrDefaultAsync(u => u.Id == thread.UserId);

      if (thread.HasDownVoted.Contains(User.Id))
      {
        await _threadVoteHubContext.Clients.All.SendAsync("downvoteThread", thread.Rating, thread.Id);
        return;
      }
      if (thread.HasUpVoted.Contains(User.Id))
      {
        thread.HasUpVoted.Remove(User.Id);
        thread.Rating--;
        poster.Score--;
      }

      thread.Rating--;
      poster.Score--;
      thread.HasDownVoted.Add(User.Id);

      await _context.SaveChangesAsync();

      await _threadVoteHubContext.Clients.All.SendAsync("downvoteThread", thread.Rating, thread.Id);
    }

    public async Task OnPostUpvoteComment(int commentId)
    {
      User = await _context.Users.FirstOrDefaultAsync(u => u.Username == HttpContext.User.Identity.Name);
      var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
      var poster = await _context.Users.FirstOrDefaultAsync(u => u.Id == comment.UserId);

      if (comment.HasUpVoted.Contains(User.Id))
      {
        await _commentVoteHubContext.Clients.All.SendAsync("upvoteComment", comment.Rating, comment.Id);
        return;
      }
      if (comment.HasDownVoted.Contains(User.Id))
      {
        comment.HasDownVoted.Remove(User.Id);
        comment.Rating++;
        poster.Score++;
      }

      comment.Rating++;
      poster.Score++;
      comment.HasUpVoted.Add(User.Id);

      await _context.SaveChangesAsync();

      await _commentVoteHubContext.Clients.All.SendAsync("upvoteComment", comment.Rating, comment.Id);
    }

    public async Task OnPostDownvoteComment(int commentId)
    {
      User = await _context.Users.FirstOrDefaultAsync(u => u.Username == HttpContext.User.Identity.Name);
      var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
      var poster = await _context.Users.FirstOrDefaultAsync(u => u.Id == comment.UserId);

      if (comment.HasDownVoted.Contains(User.Id))
      {
        await _commentVoteHubContext.Clients.All.SendAsync("downvoteComment", comment.Rating, comment.Id);
        return;
      }
      if (comment.HasUpVoted.Contains(User.Id))
      {
        comment.HasUpVoted.Remove(User.Id);
        comment.Rating--;
        poster.Score--;
      }

      comment.Rating--;
      poster.Score--;
      comment.HasDownVoted.Add(User.Id);

      await _context.SaveChangesAsync();

      await _commentVoteHubContext.Clients.All.SendAsync("downvoteComment", comment.Rating, comment.Id);
    }

    public async Task<IActionResult> OnPostDeleteThread(int threadId)
    {
      User = await _context.Users.FirstOrDefaultAsync(u => u.Username == HttpContext.User.Identity.Name);
      var thread = await _context.Threads.FirstOrDefaultAsync(t => t.Id == threadId);
      var poster = await _context.Users.FirstOrDefaultAsync(u => u.Id == thread.UserId);

      if (poster != User)
      {
        return Page();
      }

      _context.Threads.Remove(thread);

      await _context.SaveChangesAsync();

      return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteComment(int commentId)
    {
      User = await _context.Users.FirstOrDefaultAsync(u => u.Username == HttpContext.User.Identity.Name);
      var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
      var poster = await _context.Users.FirstOrDefaultAsync(u => u.Id == comment.UserId);

      if (poster != User)
      {
        return Page();
      }

      _context.Comments.Remove(comment);

      await _context.SaveChangesAsync();

      return RedirectToPage();
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