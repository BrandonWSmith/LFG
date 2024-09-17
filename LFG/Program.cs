using LFG.Authorization;
using LFG.Data;
using LFG.Hubs;
using LFG.Interface;
using LFG.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", o =>
  {
    o.Cookie.Name = "CookieAuth";
    o.LoginPath = "/Login";
    o.AccessDeniedPath = "/AccessDenied";
  }
);
builder.Services.AddAuthorization(o =>
  {
    o.AddPolicy("Registered",
      policy => policy
        .RequireClaim(ClaimTypes.Name)
        .RequireClaim(ClaimTypes.Email)
    );
    o.AddPolicy("ProfileOwner",
      policy => policy
        .RequireClaim(ClaimTypes.Name)
        .Requirements.Add(new ProfileOwnerRequirement(new HttpContextAccessor())));
  }
);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<LFGContext>(o =>
  {
    o.UseNpgsql(builder.Configuration.GetConnectionString("LFGDb"));
  }
);

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IAuthorizationHandler, ProfileOwnerRequirementHandler>();
builder.Services.AddTransient<IRazorPartialToStringRenderer, RazorPartialToStringRenderer>();
builder.Services.AddSignalR();

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapHub<ThreadVoteHub>("/hubs/thread-rating");
app.MapHub<CommentVoteHub>("/hubs/comment-rating");
app.MapHub<GroupPageHub>("/hubs/group-page");
app.MapHub<GroupSearchHub>("/hubs/group-search");
app.MapHub<GameSearchHub>("/hubs/game-search");

app.Run();