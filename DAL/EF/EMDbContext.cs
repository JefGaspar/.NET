using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using UI;

namespace EM.DAL.EF;

public class EMDbContext : DbContext
{
    /*
     *constuructor accepteert parameter van type DbContextOptions en roept
     * de constuctor van DbContext superklasse aan met base(options.
     * DbContext supperklasse verwacht een DbContextOptions object waarin
     * informatie staat over hoe de context geconfigureerd moet worden,
     * zoals welke DBprovider zoals SQlite
     */
    public EMDbContext(DbContextOptions options) : base(options)
    {
    }
    //Dbset<Event> geeft aan dat Event een tabel is in de DB
    //Naam van de properties in tabelnaam in DB
    public DbSet<Event> Events { get; set; }
    public DbSet<Organisation> Organisations { get; set; }
    public DbSet<Visitor> Visitors { get; set; }
    
    
    public bool CreateDatabase(bool deleteIfExists = false)
    {
        // Controleer of de databank moet worden verwijderd
        if (deleteIfExists)
        {
            Database.EnsureDeleted();
        }
        return Database.EnsureCreated();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Controleer of de options nog niet geconfigureerd zijn
        if (!optionsBuilder.IsConfigured)
        {
            //fallback
            var defaultPath = Path.Combine(AppContext.BaseDirectory, "../../../../EMDatabase.db");
            optionsBuilder.UseSqlite($"Data Source={defaultPath}");
        }
        optionsBuilder.LogTo(message => Debug.WriteLine(message), LogLevel.Information); // Logging activeren naar Debug-venster

    }
    

}