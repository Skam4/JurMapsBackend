using JurMaps.Model;
using Microsoft.EntityFrameworkCore;

public class MyDbContext : DbContext
{

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Place> Places { get; set; }
    public DbSet<Map> Maps { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<MapCountry> MapCountries { get; set; }
    public DbSet<UserMapLike> UserMapLikes { get; set; }

    public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>().HasData(
            new Role { RoleId = 1, RoleName = "user" },
            new Role { RoleId = 2, RoleName = "admin" }
        );

        modelBuilder.Entity<Map>()
            .HasOne(v => v.MapCreator)
            .WithMany(v => v.UserMaps)
            .HasForeignKey(m => m.MapCreatorId);

        modelBuilder.Entity<Place>()
            .HasOne(p => p.PlaceMap)
            .WithMany(m => m.MapPlaces)
            .HasForeignKey(p => p.MapId);

        modelBuilder.Entity<Tag>()
            .HasMany(v => v.MapsWithThisTag)
            .WithMany(v => v.MapTags);

        modelBuilder.Entity<MapCountry>()
                .HasKey(mc => new { mc.MapId, mc.CountryId });

        modelBuilder.Entity<MapCountry>()
            .HasOne(mc => mc.Map)
            .WithMany(m => m.MapCountries)
            .HasForeignKey(mc => mc.MapId);

        modelBuilder.Entity<MapCountry>()
            .HasOne(mc => mc.Country)
            .WithMany(c => c.MapCountries)
            .HasForeignKey(mc => mc.CountryId);

        modelBuilder.Entity<UserMapLike>()
        .HasKey(um => new { um.UserId, um.MapId });

        modelBuilder.Entity<UserMapLike>()
            .HasOne(um => um.User)
            .WithMany(u => u.UserLikedMaps)
            .HasForeignKey(um => um.UserId);

        modelBuilder.Entity<UserMapLike>()
            .HasOne(um => um.Map)
            .WithMany(m => m.UserLikes)
            .HasForeignKey(um => um.MapId);

        modelBuilder.Entity<User>().HasOne(u => u.UserRole).WithMany().HasForeignKey(u => u.UserRoleId);

        modelBuilder.HasPostgresExtension("postgis");


        modelBuilder.Entity<User>().HasData(
            new User
            {
                UserId = 1,
                UserName = "admin",
                UserEmail = "admin@admin.pl",
                UserPassword = BCrypt.Net.BCrypt.EnhancedHashPassword("admin", 13),
                UserRoleId = 2
            },
                        new User
                        {
                            UserId = 2,
                            UserName = "qwerty",
                            UserEmail = "qwerty@qwerty.pl",
                            UserPassword = BCrypt.Net.BCrypt.EnhancedHashPassword("qwerty", 13),
                            UserRoleId = 1,
                            IsVerified = true
                        }
            );
    }

}