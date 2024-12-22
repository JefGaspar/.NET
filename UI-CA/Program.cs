// See https://aka.ms/new-console-template for more information

using BL;
using EM.DAL;
using UI;
using EM.DAL.EF;
using Microsoft.EntityFrameworkCore;

//Composition Root
//initialiseer repo, manager en DB
var projectRoot = Path.Combine(AppContext.BaseDirectory, "../../../../EMDatabase.db");

var dbOptionBuilder = new DbContextOptionsBuilder<EMDbContext>();
dbOptionBuilder.UseSqlite($"Data Source={projectRoot}");

var contxt = new EMDbContext(dbOptionBuilder.Options);
IRepository repository = new Repository(contxt);
IManager manager = new Manager(repository);
var consoleUi  = new ConsoleUi(manager);

// Roep CreateDatabase aan en controleer of de database nieuw werd aangemaakt
if (contxt.CreateDatabase(deleteIfExists: true))
{
    Console.WriteLine("De databank is nieuw aangemaakt.");
    DataSeeder.Seed(contxt);
}
else
{
    Console.WriteLine("De databank bestond al en bleef behouden.");
}
consoleUi.Run();
