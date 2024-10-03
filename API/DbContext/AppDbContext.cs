using Microsoft.EntityFrameworkCore;
using API.Models;
using API.DTOs;

namespace API.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<GameDTO> Games { get; set; }
    public DbSet<UserScore> UserScores { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserScore>().HasKey(u => u.Id);     // User ID creation

        modelBuilder.Entity<User>().HasKey(u => u.UserId);
        // .HasMany(u => u.Scores)
        // .WithOne(s => s.User)
        // .HasForeignKey(u => u.UserId)
        // .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GameDTO>()
        .HasKey(g => g.GameID);

        modelBuilder.Entity<GameDTO>()
            .ToTable("Games");

        modelBuilder.Entity<GameDTO>()
            .Property(g => g.Title)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<GameDTO>()
             .Property(g => g.Description)
             .HasMaxLength(500);

        modelBuilder.Entity<GameDTO>()
            .HasData(
                new GameDTO
                {
                    GameID = 1,
                    Title = "Long number memory",
                    Description = "Memorize as many digits of a long number as possible.",
                    Icon = "src/assets/longNumberIcon.svg",
                    AltText = "Long number memory icon",
                    Route = "/longNumber",
                },
            new GameDTO
            {
                GameID = 2,
                Title = "Chimp Test",
                Description = "Test your memory by remembering the order of numbers displayed on the screen.",
                Icon = "src/assets/chimpIcon.svg",
                AltText = "Chimp test icon",
                Route = "/chimpTest",
            },
            new GameDTO
            {
                GameID = 3,
                Title = "Sequence Memorization",
                Description = "Remember and recall increasingly larger sequence of action showed.",
                Icon = "src/assets/sequenceIcon.svg",
                AltText = "Sequence memory icon",
                Route = "/sequence",
            }
        );
    }
}