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
    public DbSet<Ticket> Tickets { get; set; }
    
    
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
    
    //Fluent API
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Ticket>()
            .Property<int>("fkEventId");
        modelBuilder.Entity<Ticket>()
            .Property<int>("fkVisitorId");
        //Ticket * - 1 Event
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Event)
            .WithMany(e => e.Tickets)
            .HasForeignKey("fkEventId")
            .IsRequired();
        //Visitor * - 1 Ticket
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Visitor)
            .WithMany(v => v.Tickets)
            .HasForeignKey("fkVisitorId")
            .IsRequired();

        modelBuilder.Entity<Ticket>()
            .HasKey("fkEventId","fkVisitorId");
        
        
        modelBuilder.Entity<Event>()
            .HasOne(e => e.Organisation)
            .WithMany(o => o.Events)
            .IsRequired(false);
    }

    

}