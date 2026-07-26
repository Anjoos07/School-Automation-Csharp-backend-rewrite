using Microsoft.EntityFrameworkCore;
using DbModels;

namespace Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Form> Forms => Set<Form>();

    public DbSet<Kdrrod> Kdrrod => Set<Kdrrod>();
}