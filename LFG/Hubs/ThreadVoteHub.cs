using LFG.Data;
using LFG.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LFG.Hubs;

public class ThreadVoteHub : Hub
{
    private readonly LFGContext _context;

    public ThreadVoteHub(LFGContext context)
    {
        _context = context;
    }

    public List<Models.Thread> Threads { get; set; }
    public User User { get;  set; }

    public override async Task OnConnectedAsync()
    {
        Threads =  await _context.Threads.ToListAsync();
        User = await _context.Users.FirstOrDefaultAsync(u => u.Username == Context.User.Identity.Name);

        Threads.ForEach(async thread =>
        {
            if (thread.HasUpVoted.Contains(User.Id))
            {
                await Clients.Caller.SendAsync("disableUpvoteButton", thread.Id);
            }

            if (thread.HasDownVoted.Contains(User.Id))
            {
                await Clients.Caller.SendAsync("disableDownvoteButton", thread.Id);
            }
        });

        await base.OnConnectedAsync();
    }
}