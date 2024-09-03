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
    public User User { get;  set; }

    public override async Task OnConnectedAsync()
    {
        Comments =  await _context.Comments.ToListAsync();
        User = await _context.Users.FirstOrDefaultAsync(u => u.Username == Context.User.Identity.Name);

        Comments.ForEach(async comment =>
        {
            if (comment.HasUpVoted.Contains(User.Id))
            {
                await Clients.Caller.SendAsync("disableUpvoteButton", comment.Id);
            }

            if (comment.HasDownVoted.Contains(User.Id))
            {
                await Clients.Caller.SendAsync("disableDownvoteButton", comment.Id);
            }
        });

        await base.OnConnectedAsync();
    }
}