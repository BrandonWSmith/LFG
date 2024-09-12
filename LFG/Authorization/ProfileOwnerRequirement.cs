using Microsoft.AspNetCore.Authorization;

namespace LFG.Authorization;

public class ProfileOwnerRequirement : IAuthorizationRequirement
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public ProfileOwnerRequirement(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public bool CheckUsername()
  {
    var username = _httpContextAccessor.HttpContext.User.Identity.Name;
    var profileUsername = _httpContextAccessor.HttpContext.GetRouteValue("username").ToString();

    return username == profileUsername;
  }
}

public class ProfileOwnerRequirementHandler : AuthorizationHandler<ProfileOwnerRequirement>
{
  protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProfileOwnerRequirement requirement)
  {
    var isProfileUser = requirement.CheckUsername();

    if (isProfileUser)
    {
      context.Succeed(requirement);
    }

    return Task.CompletedTask;
  }
}