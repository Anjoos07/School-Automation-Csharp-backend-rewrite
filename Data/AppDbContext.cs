using Microsoft.EntityFrameworkCore;
using DbModelForms;

namespace Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Form> Forms => Set<Form>();
}