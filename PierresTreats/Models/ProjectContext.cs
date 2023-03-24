using Microsoft.EntityFrameworkCore;
namespace PierresTreats.Models;

public class ProjectContext : DbContext
{
  public DbSet<Flavor> Flavors { get; set; }
  public DbSet<Treat> Treats { get; set; }
  public DbSet<FlavorTreat> FlavorTreats { get; set; }
  public ProjectContext(DbContextOptions options) : base(options) { }
}
