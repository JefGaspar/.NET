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



consoleUi.Run();
