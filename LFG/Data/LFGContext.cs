using LFG.Models;
using Microsoft.EntityFrameworkCore;
using Thread = LFG.Models.Thread;

namespace LFG.Data;

public class LFGContext : DbContext
{
    public LFGContext(DbContextOptions<LFGContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseSerialColumns();
        modelBuilder.Entity<GameDeveloper>(
            eb =>
            {
                eb.HasNoKey();
            }
        );
        modelBuilder.Entity<GamePlatform>(
            eb =>
            {
                eb.HasNoKey();
            }
        );
        modelBuilder.Entity<GamePublisher>(
            eb =>
            {
                eb.HasNoKey();
            }
        );
        modelBuilder.Entity<GroupGame>(
            eb =>
            {
                eb.HasNoKey();
            }
        );
        modelBuilder.Entity<GroupLink>(
            eb =>
            {
                eb.HasNoKey();
            }
        );
        modelBuilder.Entity<UserFriend>(
            eb =>
            {
                eb.HasNoKey();
            }
        );
        modelBuilder.Entity<UserGame>(
            eb =>
            {
                eb.HasNoKey();
            }
        );
        modelBuilder.Entity<UserGroup>(
            eb =>
            {
                eb.HasNoKey();
            }
        );
        modelBuilder.Entity<UserPlatform>(
            eb =>
            {
                eb.HasNoKey();
            }
        );
    }

    public DbSet<Comment> Comments { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<GameDeveloper> GamesDevelopers { get; set; }
    public DbSet<GameEngine> GameEngines { get; set; }
    public DbSet<GamePlatform> GamesPlatforms { get; set; }
    public DbSet<GamePublisher> GamesPublishers { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupGame> GroupsGames { get; set; }
    public DbSet<GroupLink> GroupsLinks { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Platform> Platforms { get; set; }
    public DbSet<Thread> Threads { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserFriend> UsersFriends { get; set; }
    public DbSet<UserGame> UsersGames { get; set; }
    public DbSet<UserGroup> UsersGroups { get; set; }
    public DbSet<UserPlatform> UsersPlatforms { get; set; }
}