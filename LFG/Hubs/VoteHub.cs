using Microsoft.AspNetCore.SignalR;

namespace LFG.Hubs;

public class VoteHub : Hub
{
    public async Task OnVote(int rating, int threadId)
    {
        await Clients.All.SendAsync("updateRating", rating, threadId);
    }
}