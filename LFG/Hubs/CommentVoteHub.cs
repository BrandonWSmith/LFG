using LFG.Data;
using LFG.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LFG.Hubs;

public class CommentVoteHub : Hub
{
  private readonly LFGContext _context;

  public CommentVoteHub(LFGContext context)
  {
    _context = context;
  }

  public List<Comment> Comments { get; set; }
  public User User { get; set; }

  public override async Task OnConnectedAsync()
  {
    Comments = await _context.Comments.ToListAsync();
    User = await _context.Users.FirstOrDefaultAsync(u => u.Username == Context.User.Identity.Name);

    Console.WriteLine("Connected to hub");

    Comments.ForEach(async comment =>
    {
      if (comment.HasUpVoted.Contains(User.Id))
      {
        await Clients.Caller.SendAsync("disableCommentUpvoteButton", comment.Id);
      }

      if (comment.HasDownVoted.Contains(User.Id))
      {
        await Clients.Caller.SendAsync("disableCommentDownvoteButton", comment.Id);
      }
    });

    await base.OnConnectedAsync();
  }

  public async Task DisableCommentUpvoteButton(int commentId)
  {
    await Clients.Caller.SendAsync("disableCommentUpvoteButton", commentId);
  }

  public async Task DisableCommentDownvoteButton(int commentId)
  {
    await Clients.Caller.SendAsync("disableCommentDownvoteButton", commentId);
  }

  public async Task EnableCommentUpvoteButton(int commentId)
  {
    await Clients.Caller.SendAsync("enableCommentUpvoteButton", commentId);
  }

  public async Task EnableCommentDownvoteButton(int commentId)
  {
    await Clients.Caller.SendAsync("enableCommentDownvoteButton", commentId);
  }
}