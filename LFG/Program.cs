using LFG.Data;
using LFG.Utility;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", o =>
    {
        o.Cookie.Name = "CookieAuth";
    }
);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<LFGContext>(o =>
    {
        o.UseNpgsql(builder.Configuration.GetConnectionString("LFGDb"));
    }
);

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

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

app.Run();
