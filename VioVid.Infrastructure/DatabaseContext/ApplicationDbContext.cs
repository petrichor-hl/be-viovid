using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VioVid.Core.Entities;
using VioVid.Core.Identity;

namespace VioVid.Infrastructure.DatabaseContext;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    // public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    // {
    // }
    //
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     if (!optionsBuilder.IsConfigured)
    //     {
    //         optionsBuilder.UseNpgsql("Host=aws-0-us-east-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.htganmgiwwqyswitxwtl;Password=uxrW*_D4X_J58sB");
    //     }
    // }


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

    public DbSet<Payment> Payments { get; set; }

    public DbSet<Channel> Channels { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostComment> PostComments { get; set; }
}