// See https://aka.ms/new-console-template for more information

using BL;
using EM.DAL;
using UI;

//Composition Root
//initialiseer repo en manager
InMemoryRepository repository = new InMemoryRepository();
IManager manager = new Manager(repository);
//Maak nieuwe consoleUI instantie, geef manager mee
var consoleUi  = new ConsoleUi(manager);

//seeding en run
InMemoryRepository.Seed();
consoleUi.Run();