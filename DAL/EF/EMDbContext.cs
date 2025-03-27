using System.Diagnostics;
using EM.BL.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace EM.DAL.EF;
/**
 * De EmDbContext klasse fungeert als de databanklaag van de applicatie en:

1. Beheert de configuratie van de database.
2. Definieert entiteiten en hun overeenkomstige database-tabellen.
3. Stelt de relaties tussen de entiteiten expliciet in.
4. Ondersteunt databasebeheer, zoals het (opnieuw) maken van een database.
5. Integreert logging voor debuggen van database-activiteiten.

Deze klasse is essentieel om te zorgen voor een duidelijke en gestructureerde interactie tussen de applicatiecode en de database.
 */
public class EmDbContext : IdentityDbContext
{
    /*
     *constuructor accepteert parameter van type DbContextOptions en roept
     * de constuctor van DbContext superklasse aan met base(options.
     * DbContext supperklasse verwacht een DbContextOptions object waarin
     * informatie staat over hoe de context geconfigureerd moet worden,
     * zoals welke DBprovider zoals SQlite
     */
    public EmDbContext(DbContextOptions options) : base(options)
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

        // Configureren van Ticket-entiteit
        modelBuilder.Entity<Ticket>()
            .Property<int>("fkEventId");
        modelBuilder.Entity<Ticket>()
            .Property<int>("fkVisitorId");
        //vreemde sluitels op ticket instellen
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

        //samengestelde primaire sleutel
        modelBuilder.Entity<Ticket>()
            .HasKey("fkEventId","fkVisitorId");
        
        
        modelBuilder.Entity<Event>()
            .HasOne(e => e.Organisation)
            .WithMany(o => o.Events)
            .IsRequired(false);
    }

    

}