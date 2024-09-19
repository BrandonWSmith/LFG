using LFG.Data;
using LFG.Interface;
using LFG.Models;
using LFG.Pages.Profile;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LFG.Hubs;

public class ProfileHub : Hub
{
  private readonly LFGContext _context;
  private readonly IRazorPartialToStringRenderer _renderer;

  public ProfileHub(LFGContext context, IRazorPartialToStringRenderer renderer)
  {
    _context = context;
    _renderer = renderer;
  }

  public ProfileModel ProfileModel { get; set; }

  public async Task UpdateUserInfo(int userId)
  {
    ProfileModel = new ProfileModel(_context, null);
    ProfileModel.User = await _context.Users.FindAsync(userId);
    ProfileModel.UserPlatforms = await _context.UsersPlatforms
        .Where(p => p.UserId == userId)
        .Include(p => p.Platform)
        .Select(p => new Platform
        {
          Name = p.Platform.Name
        })
        .ToListAsync();
    ProfileModel.UserPlatformNames = ProfileModel.UserPlatforms.Select(p => p.Name).ToList();

    var profilePartial =
      await _renderer.RenderPartialToStringAsync("_ProfilePartial", ProfileModel);

    await Clients.All.SendAsync("refreshUserInfo", profilePartial);
  }
  public async Task UpdateEditUserInfo(int userId, string platformExists)
  {
    ProfileModel = new ProfileModel(_context, null);
    ProfileModel.User = await _context.Users.FindAsync(userId);
    ProfileModel.UserPlatforms = await _context.UsersPlatforms
        .Where(p => p.UserId == userId)
        .Include(p => p.Platform)
        .Select(p => new Platform
        {
          Name = p.Platform.Name
        })
        .ToListAsync();
    ProfileModel.UserPlatformNames = ProfileModel.UserPlatforms.Select(p => p.Name).ToList();
    ProfileModel.AllPlatformsList = new SelectList(_context.Platforms.Select(p => p.Name).ToList());
    ProfileModel.PlatformExists = platformExists;

    var editProfilePartial =
      await _renderer.RenderPartialToStringAsync("_EditProfilePartial", ProfileModel);

    await Clients.All.SendAsync("refreshEditUserInfo", editProfilePartial);
  }
}