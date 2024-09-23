using LFG.Data;
using LFG.Hubs;
using LFG.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace LFG.Pages.Games
{
  [Authorize(Policy = "Registered")]
  public class GamesModel : PageModel
  {
    private readonly LFGContext _context;
    private readonly IHubContext<GameSearchHub> _gameSearchHubContext;

    public GamesModel(LFGContext context, IHubContext<GameSearchHub>? gameSearchHubContext)
    {
      _context = context;
      if (gameSearchHubContext != null) _gameSearchHubContext = gameSearchHubContext;
    }

    public List<Game> Games { get; set; }
    public SortedList<int, int> GameGroupCount { get; set; } = [];

    [BindProperty]
    public string? SelectedGame { get; set; }

    public async Task OnGetAsync()
    {
      Games = _context.Games.ToList();
      Games.ForEach(game =>
      {
        var gameGroups = _context.GroupsGames.Where(g => g.GameId == game.Id).ToList();
        GameGroupCount.Add(game.Id, gameGroups.Count());
      });
    }

    public async Task OnPostGameSearch()
    {
      Games = _context.Games.ToList();

      await _gameSearchHubContext.Clients.All.SendAsync("gameSearch", Games, SelectedGame);
    }
  }
}
