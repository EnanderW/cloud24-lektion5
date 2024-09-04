using Microsoft.EntityFrameworkCore;

public class ApplicationContext : DbContext
{
    public DbSet<SearchMovie> Movies { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options) { }
}
