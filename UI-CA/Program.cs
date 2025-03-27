// See https://aka.ms/new-console-template for more information

using EM.BL;
using EM.DAL;
using EM.DAL.EF;
using EM.UI.CA;
using Microsoft.EntityFrameworkCore;

//Composition Root
//initialiseer repo, manager en DB
var projectRoot = Path.Combine(AppContext.BaseDirectory, "../../../../EMDatabase.db");

var dbOptionBuilder = new DbContextOptionsBuilder<EmDbContext>();
dbOptionBuilder.UseSqlite($"Data Source={projectRoot}");

var contxt = new EmDbContext(dbOptionBuilder.Options);
IRepository repository = new Repository(contxt);
IManager manager = new Manager(repository);
var consoleUi  = new ConsoleUi(manager);

// Roep CreateDatabase aan en controleer of de database nieuw werd aangemaakt
if (contxt.CreateDatabase(deleteIfExists: true))
{
    Console.WriteLine("De databank is nieuw aangemaakt.");
    /*DataSeeder.SeedAsync(contxt, )*/
}
else
{
    Console.WriteLine("De databank bestond al en bleef behouden.");
}
consoleUi.Run();
