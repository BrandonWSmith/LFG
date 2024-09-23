using LFG.Data;
using LFG.Enums;
using LFG.Interface;
using LFG.Models;
using LFG.Pages.Group;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LFG.Hubs;

public class GroupPageHub : Hub
{
  private readonly LFGContext _context;
  private readonly IRazorPartialToStringRenderer _renderer;
  private readonly IHubContext<ThreadVoteHub> _threadVoteHubContext;
  private readonly IHubContext<CommentVoteHub> _commentVoteHubContext;

  public GroupPageHub(LFGContext context, IRazorPartialToStringRenderer renderer, IHubContext<ThreadVoteHub> threadVoteHubContext, IHubContext<CommentVoteHub> commentVoteHubContext)
  {
    _context = context;
    _renderer = renderer;
    _threadVoteHubContext = threadVoteHubContext;
    _commentVoteHubContext = commentVoteHubContext;
  }

  public GroupModel GroupModel { get; set; }

  public async Task UpdateThreads(int groupId)
  {
    GroupModel = new GroupModel(_context, _threadVoteHubContext, _commentVoteHubContext, null);
    GroupModel.GroupThreads = await _context.Threads.Where(t => t.GroupId == groupId).OrderByDescending(t => t.Pinned == true).ThenByDescending(t => t.Created).ThenBy(t => t.Id).ToListAsync();
    GroupModel.GroupThreads.ForEach(thread =>
    {
      thread.User = _context.Users.FirstOrDefault(u => u.Id == thread.UserId);
      GroupModel.ThreadComments = _context.Comments.Where(c => c.ThreadId == thread.Id).OrderByDescending(c => c.Created).ThenBy(c => c.Id).ToList();
      GroupModel.ThreadComments.ForEach(comment =>
      {
        comment.User = _context.Users.FirstOrDefault(u => u.Id == comment.UserId);
      });
    });

    var groupThreadsPartial =
      await _renderer.RenderPartialToStringAsync("_GroupThreadsPartial", GroupModel);

    await Clients.All.SendAsync("refreshThreads", groupThreadsPartial);
    await _threadVoteHubContext.Clients.All.SendAsync("restartThreadRatingConnection");
    await _commentVoteHubContext.Clients.All.SendAsync("restartCommentRatingConnection");
  }

  public async Task UpdateComments(int threadId)
  {
    GroupModel = new GroupModel(_context, _threadVoteHubContext, _commentVoteHubContext, null);
    GroupModel.ThreadComments = _context.Comments.Where(c => c.ThreadId == threadId).OrderByDescending(c => c.Created).ThenBy(c => c.Id).ToList();
    GroupModel.ThreadComments.ForEach(comment =>
    {
      comment.User = _context.Users.FirstOrDefault(u => u.Id == comment.UserId);
    });

    var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { { "Thread", _context.Threads.Find(threadId) }, { "ThreadComments", GroupModel.ThreadComments } };

    var threadCommentsPartial =
      await _renderer.RenderPartialToStringAsync("_ThreadCommentsPartial", GroupModel, viewData);

    await Clients.All.SendAsync("refreshComments", threadId, threadCommentsPartial);
    await _threadVoteHubContext.Clients.All.SendAsync("restartThreadRatingConnection");
    await _commentVoteHubContext.Clients.All.SendAsync("restartCommentRatingConnection");
  }

  public async Task UpdateGroupInfo(int groupId)
  {
    GroupModel = new GroupModel(_context, _threadVoteHubContext, _commentVoteHubContext, null);
    GroupModel.Group = await _context.Groups.FindAsync(groupId);
    GroupModel.Owner = await _context.Users.FirstOrDefaultAsync(u => u.Id == GroupModel.Group.Owner);
    GroupModel.GroupMemberCount = _context.UsersGroups.Where(g => g.GroupId == groupId).Count();
    GroupModel.GroupLinks = await _context.GroupsLinks.Where(l => l.GroupId == groupId).ToListAsync();

    var groupCardPartial =
      await _renderer.RenderPartialToStringAsync("_GroupCardPartial", GroupModel);

    await Clients.All.SendAsync("refreshGroupInfo", groupCardPartial);
  }
  public async Task UpdateEditGroupInfo(int groupId, Website? selectedSite)
  {
    GroupModel = new GroupModel(_context, _threadVoteHubContext, _commentVoteHubContext, null);
    GroupModel.Group = await _context.Groups.FindAsync(groupId);
    GroupModel.Owner = await _context.Users.FirstOrDefaultAsync(u => u.Id == GroupModel.Group.Owner);
    GroupModel.GroupLinks = await _context.GroupsLinks.Where(l => l.GroupId == groupId).ToListAsync();
    GroupModel.GroupGames = await _context.GroupsGames
        .Where(g => g.GroupId == GroupModel.Group.Id)
        .Include(g => g.Game)
        .Select(g => new Game
        {
          Name = g.Game.Name,
          CoverId = g.Game.CoverId
        })
        .ToListAsync();
    GroupModel.GroupGameNames = GroupModel.GroupGames.Select(g => g.Name).ToList();
    GroupModel.AllGamesList = await _context.Games.Select(g => g.Name).ToListAsync();

    if (selectedSite != null)
    {
      GroupModel.SelectedSite = (Website)selectedSite;
    }

    var editGroupCardPartial =
      await _renderer.RenderPartialToStringAsync("_EditGroupCardPartial", GroupModel);

    await Clients.All.SendAsync("refreshEditGroupInfo", editGroupCardPartial);
  }

  public async Task UpdateGroupGames(int groupId)
  {
    GroupModel = new GroupModel(_context, _threadVoteHubContext, _commentVoteHubContext, null);
    GroupModel.Group = await _context.Groups.FindAsync(groupId);
    GroupModel.GroupGames = await _context.GroupsGames
        .Where(g => g.GroupId == GroupModel.Group.Id)
        .Include(g => g.Game)
        .Select(g => new Game
        {
          Name = g.Game.Name,
          CoverId = g.Game.CoverId
        })
        .ToListAsync();

    var gamesListPartial =
      await _renderer.RenderPartialToStringAsync("_GamesListPartial", GroupModel);

    await Clients.All.SendAsync("refreshGroupGames", gamesListPartial);
  }
}