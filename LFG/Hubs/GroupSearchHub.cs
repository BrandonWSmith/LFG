using LFG.Data;
using LFG.Interface;
using LFG.Pages.Group;
using Microsoft.AspNetCore.SignalR;

namespace LFG.Hubs;

public class GroupSearchHub : Hub
{
  private readonly LFGContext _context;
  private readonly IRazorPartialToStringRenderer _renderer;

  public GroupSearchHub(LFGContext context, IRazorPartialToStringRenderer renderer)
  {
    _context = context;
    _renderer = renderer;
  }

  public GameGroupsModel GameGroupsModel { get; set; }

  public async Task GroupSearch(List<Models.Group> gameGroups, string selectedGroup)
  {
    GameGroupsModel = new GameGroupsModel(_context, null);
    GameGroupsModel.GameGroups = gameGroups.Where(g => g.Name.Contains(selectedGroup, StringComparison.CurrentCultureIgnoreCase)).Select(g => g).ToList();

    var gameGroupCardPartial =
      await _renderer.RenderPartialToStringAsync("_GameGroupCardsPartial", GameGroupsModel);

    await Clients.Caller.SendAsync("loadGroups", gameGroupCardPartial);
  }
}