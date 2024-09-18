using LFG.Data;
using LFG.Enums;
using LFG.Hubs;
using LFG.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Thread = LFG.Models.Thread;

namespace LFG.Pages.Group
{
  [Authorize(Policy = "Registered")]
  [BindProperties(SupportsGet = true)]
  public class GroupModel : PageModel
  {
    private readonly LFGContext _context;
    private readonly IHubContext<ThreadVoteHub> _threadVoteHubContext;
    private readonly IHubContext<CommentVoteHub> _commentVoteHubContext;
    private readonly IHubContext<GroupPageHub> _groupPageHubContext;

    public GroupModel(LFGContext context, IHubContext<ThreadVoteHub> threadVoteHubContext, IHubContext<CommentVoteHub> commentVoteHubContext, IHubContext<GroupPageHub>? groupPageHubContext)
    {
      _context = context;
      _threadVoteHubContext = threadVoteHubContext;
      _commentVoteHubContext = commentVoteHubContext;
      if (groupPageHubContext != null) _groupPageHubContext = groupPageHubContext;
    }

    public User User { get; set; }
    public UserGroup? UserGroup { get; set; }
    public GroupRole UserRole { get; set; }
    public Models.Group Group { get; set; }
    public User Owner { get; set; }
    public List<Game> GroupGames { get; set; }
    public List<string> GroupGameNames { get; set; }
    public List<string> AllGamesList { get; set; }
    public string? SelectedGame { get; set; }
    public string? GameToRemove { get; set; }
    public List<Thread> GroupThreads { get; set; }
    public List<Comment> ThreadComments { get; set; }
    public Thread Thread { get; set; }
    public Comment Comment { get; set; }

    [ViewData]
    public string GameExists { get; set; }

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
        .Select(g => new Game
        {
          Name = g.Game.Name,
          CoverId = g.Game.CoverId
        })
        .ToListAsync();

      GroupGameNames = GroupGames.Select(g => g.Name).ToList();

      AllGamesList = await _context.Games.Select(g => g.Name).ToListAsync();

      GroupThreads = await _context.Threads.Where(t => t.GroupId == Group.Id).OrderByDescending(t => t.Pinned == true).ThenByDescending(t => t.Created).ThenBy(t => t.Id).ToListAsync();
    }

    public async Task OnPostUpdateGroupInfo(int groupId)
    {
      //Get Group
      Group = await _context.Groups.FindAsync(groupId);

      //Set Values From Form
      Group.Name = Request.Form["Group.Name"].ToString();
      Group.Description = Request.Form["Group.Description"].ToString();
      Group.Public = Request.Form["Group.Public"].Contains("true");
      Group.Status = GroupStatus.Parse<GroupStatus>(Request.Form["Group.Status"].ToString());

      //Update Group
      _context.Groups.Attach(Group).State = EntityState.Modified;
      await _context.SaveChangesAsync();

      //Call SignalR Method To Refresh Group Info
      await _groupPageHubContext.Clients.All.SendAsync("updateGroupInfo", groupId);
    }

    public async Task OnPostAddGame()
    {
      if (string.IsNullOrEmpty(SelectedGame))
      {
        return;
      }

      Group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == RouteData.Values["groupname"]);

      var gameId = await _context.Games
        .Where(g => g.Name == SelectedGame)
        .Select(g => g.Id)
        .SingleAsync();

      var gameToAdd = new GroupGame
      {
        GroupId = Group.Id,
        GameId = gameId
      };

      if (!_context.GroupsGames.Contains(gameToAdd))
      {
        await _context.GroupsGames.AddAsync(gameToAdd);
        await _context.SaveChangesAsync();
      }
      else
      {
        GameExists = "Game already added";
      }

      await _groupPageHubContext.Clients.All.SendAsync("updateEditGroupInfo", Group.Id);
      await _groupPageHubContext.Clients.All.SendAsync("updateGroupGames", Group.Id);
    }

    public async Task OnPostRemoveGame()
    {
      if (string.IsNullOrEmpty(GameToRemove))
      {
        return;
      }

      Group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == RouteData.Values["groupname"]);

      var gameToRemoveName = Request.Form["GameToRemove"].ToString();

      var gameId = await _context.Games
        .Where(g => g.Name == gameToRemoveName)
        .Select(g => g.Id)
        .SingleAsync();

      var gameToRemove = new GroupGame
      {
        GroupId = Group.Id,
        GameId = gameId
      };

      if (_context.GroupsGames.Contains(gameToRemove))
      {
        _context.GroupsGames.Remove(gameToRemove);
        await _context.SaveChangesAsync();
      }
      else
      {
        GameExists = "Game has not been added";
      }

      await _groupPageHubContext.Clients.All.SendAsync("updateEditGroupInfo", Group.Id);
      await _groupPageHubContext.Clients.All.SendAsync("updateGroupGames", Group.Id);

    }

    public async Task<IActionResult> OnPostJoin()
    {
      //Get Group and User Info
      Group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == RouteData.Values["groupname"]);
      User = await _context.Users.FirstOrDefaultAsync(u => u.Username == HttpContext.User.Identity.Name);
      UserGroup = await _context.UsersGroups.FirstOrDefaultAsync(u => u.UserId == User.Id && u.GroupId == Group.Id);

      //Check If User Is Already A Member
      if (UserGroup != null) return Page();

      //Add User To Group
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

    public async Task OnPostCreateThread()
    {
      //Populate Non-Editable Fields
      Thread.UserId = await _context.Users.Where(u => u.Username == HttpContext.User.Identity.Name).Select(u => u.Id)
        .SingleAsync();
      Thread.GroupId = await _context.Groups.Where(g => g.Name == RouteData.Values["groupname"]).Select(g => g.Id)
        .SingleAsync();
      Thread.Created = DateTime.Now;

      //Create Thread
      await _context.Threads.AddAsync(Thread);
      await _context.SaveChangesAsync();

      //Call SignalR Method To Refresh Threads
      await _groupPageHubContext.Clients.All.SendAsync("updateThreads", Thread.GroupId);
    }

    public async Task OnPostUpdateGroupThread(int threadId)
    {
      //Populate Non-Editable Fields
      Thread = await _context.Threads.FindAsync(threadId);
      Thread.Title = Request.Form["Thread.Title"].ToString();
      Thread.Body = Request.Form["Thread.Body"].ToString();
      Thread.Pinned = Request.Form["Thread.Pinned"].Contains("true");

      //Update Thread
      _context.Threads.Attach(Thread).State = EntityState.Modified;
      await _context.SaveChangesAsync();

      //Call SignalR Method To Refresh Threads
      await _groupPageHubContext.Clients.All.SendAsync("updateThreads", Thread.GroupId);
    }

    public async Task OnPostDeleteThread(int threadId)
    {
      //Get Thread And User Info
      Thread = await _context.Threads.FirstOrDefaultAsync(t => t.Id == threadId);
      User = await _context.Users.FirstOrDefaultAsync(u => u.Username == HttpContext.User.Identity.Name);
      var poster = await _context.Users.FirstOrDefaultAsync(u => u.Id == Thread.UserId);

      //Check User Created Thread
      if (poster != User)
      {
        return;
      }

      //Delete Thread
      _context.Threads.Remove(Thread);
      await _context.SaveChangesAsync();

      //Call SignalR Method To Refresh Threads
      await _groupPageHubContext.Clients.All.SendAsync("updateThreads", Thread.GroupId);
    }

    public async Task OnPostCreateComment(int threadId)
    {
      //Populate Non-Editable Fields
      Comment.UserId = await _context.Users.Where(u => u.Username == HttpContext.User.Identity.Name).Select(u => u.Id)
        .SingleAsync();
      Comment.ThreadId = await _context.Threads.Where(t => t.Id == threadId).Select(t => t.Id)
        .SingleAsync();
      Comment.Created = DateTime.Now;

      //Create Comment
      await _context.Comments.AddAsync(Comment);
      await _context.SaveChangesAsync();

      //Call SignalR Method To Refresh Comments
      await _groupPageHubContext.Clients.All.SendAsync("updateComments", threadId);
    }

    public async Task OnPostUpdateComment(int threadId, int commentId)
    {
      //Populate Non-Editable Fields
      Comment = await _context.Comments.FindAsync(commentId);
      Comment.Body = Request.Form["Comment.Body"].ToString();

      //Update Comment
      _context.Comments.Attach(Comment).State = EntityState.Modified;
      await _context.SaveChangesAsync();

      //Call SignalR Method To Refresh Comments
      await _groupPageHubContext.Clients.All.SendAsync("updateComments", threadId);
    }

    public async Task OnPostDeleteComment(int commentId)
    {
      //Get Comment and User Info
      Comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
      User = await _context.Users.FirstOrDefaultAsync(u => u.Username == HttpContext.User.Identity.Name);
      var poster = await _context.Users.FirstOrDefaultAsync(u => u.Id == Comment.UserId);

      //Check User Created Comment
      if (poster != User)
      {
        return;
      }

      //Delete Comment
      _context.Comments.Remove(Comment);
      await _context.SaveChangesAsync();

      //Call SignalR Method To Refresh Comments
      await _groupPageHubContext.Clients.All.SendAsync("updateComments", Comment.ThreadId);
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