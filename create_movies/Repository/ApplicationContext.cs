using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationContext : IdentityDbContext<User>
{
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Genre> Genres { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Genre>().HasData(
            new Genre {
                Id = "action"
            },
            new Genre {
                Id = "romance"
            },
            new Genre {
                Id = "comedy"
            }
        );

        builder.Entity<Actor>().HasData(
            new Actor {
                Id = Guid.Parse("d0453149-ccc2-4689-8a0c-e0be47acad46"),
                Name = "Robert Downey Jr."
            },
            new Actor {
                Id = Guid.Parse("79154997-c03c-45c2-bb6c-a75bff65bdd1"),
                Name = "Dwayne Johnson"
            }
        );

        builder.Entity<IdentityRole>().HasData(
            new IdentityRole {
                Name = "admin",
                NormalizedName = "ADMIN"
            }
        );
    }
}
