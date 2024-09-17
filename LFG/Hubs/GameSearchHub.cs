using LFG.Data;
using LFG.Interface;
using LFG.Models;
using LFG.Pages.Games;
using Microsoft.AspNetCore.SignalR;

namespace LFG.Hubs;

public class GameSearchHub : Hub
{
  private readonly LFGContext _context;
  private readonly IRazorPartialToStringRenderer _renderer;

  public GameSearchHub(LFGContext context, IRazorPartialToStringRenderer renderer)
  {
    _context = context;
    _renderer = renderer;
  }

  public GamesModel GamesModel { get; set; }

  public async Task GameSearch(List<Game> games, string selectedGame)
  {
    GamesModel = new GamesModel(_context, null);
    GamesModel.Games = games.Where(g => g.Name.Contains(selectedGame, StringComparison.CurrentCultureIgnoreCase)).ToList();
    GamesModel.Games.ForEach(game =>
    {
      var gameGroups = _context.GroupsGames.Where(g => g.GameId == game.Id).ToList();
      GamesModel.GameGroupCount.Add(game.Id, gameGroups.Count());
    });

    var gamesPartial =
      await _renderer.RenderPartialToStringAsync("_GamesPartial", GamesModel);

    await Clients.Caller.SendAsync("loadGames", gamesPartial);
  }
}