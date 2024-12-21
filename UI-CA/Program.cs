// See https://aka.ms/new-console-template for more information

using BL;
using EM.DAL;
using UI;
using EM.DAL.EF;
using Microsoft.EntityFrameworkCore;

//Composition Root
//initialiseer repo, manager en DB
InMemoryRepository repository = new InMemoryRepository();
IManager manager = new Manager(repository);
var dbOptionBuilder = new DbContextOptionsBuilder<EMDbContext>();
dbOptionBuilder.UseSqlite("Data Source=EMDatabase.db");

var context = new EMDbContext(dbOptionBuilder.Options);

// Roep CreateDatabase aan en controleer of de database nieuw werd aangemaakt
bool databaseCreated = context.CreateDatabase(deleteIfExists: true);

if (databaseCreated)
{
    Console.WriteLine("De databank is nieuw aangemaakt.");
    DataSeeder.Seed(context);
}
else
{
    Console.WriteLine("De databank bestond al en bleef behouden.");
}
//Maak nieuwe consoleUI instantie, geef manager mee
var consoleUi  = new ConsoleUi(manager,context);

//seeding en run
InMemoryRepository.Seed();
consoleUi.Run();