using System.Text.Json;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VioVid.Core.Entities;
using VioVid.Core.Identity;

namespace VioVid.Infrastructure.DatabaseContext;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Person> Persons { get; set; }
    public DbSet<Cast> Casts { get; set; }
    public DbSet<Crew> Crews { get; set; }

    public DbSet<Film> Films { get; set; }
    public DbSet<Season> Seasons { get; set; }
    public DbSet<Episode> Episodes { get; set; }
    public DbSet<Review> Reviews { get; set; }

    public DbSet<Genre> Genres { get; set; }
    public DbSet<GenreFilm> GenreFilms { get; set; }

    public DbSet<Topic> Topics { get; set; }
    public DbSet<TopicFilm> TopicFilms { get; set; }

    public DbSet<Plan> Plans { get; set; }
    public DbSet<UserPlan> UserPlans { get; set; }

    public DbSet<UserProfile> UserProfiles { get; set; }

    public DbSet<MyFilm> MyFilms { get; set; }
    
    public DbSet<TrackingProgress> TrackingProgresses { get; set; }
    
    public DbSet<UserNotification> UserNotifications { get; set; }

    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<UserNotification>(entity =>
        {
            entity.Property(n => n.Params)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null));
        });
    }
}