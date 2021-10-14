using Common.Logging;
using FilmOldPattern.DAL;
using FilmOldPattern.Model;
using Kyklos.Kernel.SpringSupport.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FilmOldPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            try
            {
                CustomAdditionalDbProvider.SetDefaultAdditionalResourceForCustomDbProvidersSqlServer();

                int idFilm = Utility.GetNextId("Film");
                int idActor = Utility.GetNextId("Actor");
                int idCast = Utility.GetNextId("Cast");



                while (true)
                {
                    string cmd = "";

                    Console.WriteLine();
                    Console.WriteLine("Digita:\n1)Per visualizzare i dati\n2)Per inserire nuova entità\n3)Per svuotare Db\n4)Per elimanare un' entità specifica\n5)Per modificare un' entità\n6)GetDateFilmByTitle\n7)Get film title by actor id\n8)Per visualizzare il numero delle entità\n9)Get Tuple\n10)Per uscire");
                    Console.WriteLine();

                    List<Film> films = Utility.GetFilms();
                    List<Actor> actors = Utility.GetActors();
                    List<Cast> casts = Utility.GetCasts();

                    cmd = Console.ReadLine();

                    switch (cmd)
                    {
                        case "1":
                            Console.WriteLine("Insert entity type for to view table\nFilm\nActor");
                            string entityType = Console.ReadLine();

                            switch (entityType)
                            {
                                case "Film":
                                    Utility.PrintTable("FILMS");
                                    break;

                                case "Actor":
                                    Utility.PrintTable("ACTORS");
                                    break;

                                case "Cast":
                                    Utility.PrintTable("CASTS");
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case "2":

                            Console.WriteLine("Iinsert type entity\nFilm\nActor");
                            entityType = Console.ReadLine();


                            switch (entityType)
                            {
                                case "Film":
                                    Console.WriteLine("Insert title");
                                    string titleFilm = Console.ReadLine();

                                    Console.WriteLine("Insert kind");
                                    string kindFilm = Console.ReadLine();

                                    Console.WriteLine("Insert plot");
                                    string plotFilm = Console.ReadLine();

                                    Console.WriteLine("Insert language");
                                    string languageFilm = Console.ReadLine();

                                    Console.WriteLine("Date:");
                                    Console.WriteLine("Insert year");
                                    int year = int.Parse(Console.ReadLine());

                                    Console.WriteLine("Insert month");
                                    int month = int.Parse(Console.ReadLine());

                                    Console.WriteLine("Insert day");
                                    int day = int.Parse(Console.ReadLine());

                                    DateTime date = new DateTime(year, month, day);

                                    Film film = new Film
                                    {
                                        Id = idFilm++,
                                        FilmTitle = titleFilm,
                                        FilmKind = kindFilm,
                                        FilmPlot = plotFilm,
                                        Language = languageFilm,
                                        Date = date
                                    };

                                    Utility.Insert("FILM", film);

                                    break;

                                case "Actor":

                                    Console.WriteLine("Insert Name");
                                    string nameActor = Console.ReadLine();

                                    Console.WriteLine("Insert LastName");
                                    string lastNameActor = Console.ReadLine();

                                    Console.WriteLine("Insert Bibliography");
                                    string bibliographyActor = Console.ReadLine();

                                    Console.WriteLine("Insert TaxIdCode");
                                    string taxIdCodeActor = Console.ReadLine();

                                    Console.WriteLine("Insert year");
                                    int yearActor = int.Parse(Console.ReadLine());

                                    Actor actor = new Actor
                                    {
                                        Id = idActor++,
                                        ActorName = nameActor,
                                        ActorLastName = lastNameActor,
                                        ActorBibliography = bibliographyActor,
                                        ActorTaxIdCode = taxIdCodeActor,
                                        ActorYear = yearActor
                                    };

                                    Utility.Insert("ACTOR", null, actor);
                                    break;

                                case "Cast":

                                    Console.WriteLine("Insert id Film");
                                    int FilmID = int.Parse(Console.ReadLine());

                                    Console.WriteLine("Insert id Actor");
                                    int ActorID = int.Parse(Console.ReadLine());

                                    Console.WriteLine("Insert role");
                                    string role = Console.ReadLine();

                             
                                    Cast cast = new Cast
                                    {
                                        Id = idActor++,
                                        FilmId = FilmID,
                                        ActorId = ActorID,
                                        Role = role
                                    };

                                    Utility.Insert("CAST", null, null, cast);
                                    break;
                            }

                            break;

                        case "3":
                            Console.WriteLine("Insert type entity\nFilm\nActor");
                            entityType = Console.ReadLine();

                            switch (entityType) 
                            {
                                case "Film":
                                    films.ForEach(f => Utility.Delete("FILM", f));
                                    break;

                                case "Actor":
                                    actors.ForEach(a => Utility.Delete("ACTOR", null, a));
                                    break;
                            }

                            break;

                        case "4":
                            Console.WriteLine("Insert type entity\nFILM\nACTOR");
                            entityType = Console.ReadLine();

                            switch (entityType) 
                            {
                                case "FILM":
                                    Console.WriteLine("Insert film title");
                                    string filmTitle = Console.ReadLine();
                                    Utility.DeleteByParameter("FILM", filmTitle);
                                    break;

                                case "ACTOR":
                                    Console.WriteLine("Insert actor name");
                                    string actorName = Console.ReadLine();
                                    Utility.DeleteByParameter("ACTOR", actorName);
                                    break;
                            }
                            break;

                        case "5":
                            Console.WriteLine("Scegli entità per cui visualizzare il numero delle entità\nFilms\nActors");
                            entityType = Console.ReadLine();
                            int count = 0;
                            switch (entityType)
                            {
                                case "Films":
                                    count = Utility.Count("FILM");
                                    break;

                                case "Actors":
                                    count = Utility.Count("Actor");
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case "6":
                            Console.WriteLine("Insert title film");
                            string titl = Console.ReadLine();
                            DateTime dateFilm = Utility.GetDatebyFilmTitle(titl);
                            break;

                        case "7":
                            Console.WriteLine("Insert id actor for search");
                            int actorId = int.Parse(Console.ReadLine());
                            List<string>filmTilte = Utility.GetFilmsTitleByActorId(actorId);
                            filmTilte.ForEach(title => Console.WriteLine($"FilmTitle:{title}"));
                            break;

                        case "9":
                            Tuple<IList<Film>, IList<Actor>, IList<Cast>> Entities = Utility.GetEntities(); 
                            break;

                        case "10":
                            return;
                        default:
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                var logger = LogManager.GetLogger("DEBUG");
                logger.Error("Error", ex);
            }
        }
    }
}
