using Microsoft.EntityFrameworkCore;

// DataBaseContext
public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    { }

    public DbSet<Blog> Blogs { get; set; }
}