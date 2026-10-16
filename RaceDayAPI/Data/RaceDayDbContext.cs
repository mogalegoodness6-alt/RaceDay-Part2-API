using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Models;
namespace RaceDayAPI.Data;
public class RaceDayDbContext : DbContext
{
    public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : base(options) { }
    public DbSet<User> Users => Set<User>();
    public DbSet<RaceEvent> Events => Set<RaceEvent>();
    public DbSet<RaceDayAPI.Models.Route> Routes { get; set; } 
    public DbSet<Weather> Weather => Set<Weather>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<EventCategory> EventCategories => Set<EventCategory>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<RaceResult> Results => Set<RaceResult>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>().ToTable("Users"); b.Entity<User>().HasKey(x => x.UserID); b.Entity<User>().Property(x => x.Email).HasMaxLength(100); b.Entity<User>().HasIndex(x => x.Email).IsUnique();
        b.Entity<RaceEvent>().ToTable("Events"); b.Entity<RaceEvent>().HasKey(x => x.EventID); b.Entity<RaceEvent>().Property(x => x.OrganizerID).HasColumnName("OrganizerID");
        b.Entity<RaceDayAPI.Models.Route>().ToTable("Routes"); b.Entity<RaceDayAPI.Models.Route>().HasKey(x => x.RouteID); b.Entity<RaceDayAPI.Models.Route>().Property(x => x.DistanceKM).HasPrecision(6, 2);
        b.Entity<Weather>().ToTable("Weather"); b.Entity<Weather>().HasKey(x => x.WeatherID); b.Entity<Weather>().Property(x => x.Temp).HasPrecision(5, 2);
        b.Entity<Category>().ToTable("Categories"); b.Entity<Category>().HasKey(x => x.CategoryID); b.Entity<Category>().Property(x => x.DistanceKM).HasPrecision(6, 2);
        b.Entity<EventCategory>().ToTable("EventCategory"); b.Entity<EventCategory>().HasKey(x => new { x.EventID, x.CategoryID });
        b.Entity<Enrollment>().ToTable("Enrollments"); b.Entity<Enrollment>().HasKey(x => x.EnrollmentID);
        b.Entity<RaceResult>().ToTable("Results"); b.Entity<RaceResult>().HasKey(x => x.ResultID);
        base.OnModelCreating(b);
    }
}
