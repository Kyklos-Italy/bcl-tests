using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Kyklos.Kernel.SpringSupport.Core;

namespace Film
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var loggerFactory = new NLogLoggerFactory();
            var logger = loggerFactory.CreateLogger(typeof(Program).FullName);

            IUtilities ut = Instantiator.GetObject<IUtilities>("Utilities");

            logger.LogDebug("Application started");

            IList<Actor> ActorList = new List<Actor>();
            Dictionary<string, List<Actor>> groupactor = new Dictionary<string, List<Actor>>();
            bool IsFirtTime = false;

            while (true)
            {
                Console.WriteLine("");
                Console.WriteLine("Digita:\n1)per creare le tabelle\n2)per eliminare le tabelle\n3)Per Visualizzare i dati della tabella Actor\n4)Per inserire un nuovo actor\n5)Per modificare un actor\n6)Per cancellare un actor\n7)Filtro\n8)Per chiudere raggruppare gli attori\n9)Per visualizzare lista dei attori raggruppati in base alla chiave\n10)Per visualizzare i giovani attori\n11)Get List in in collection\n12)Get Tuple\n13)Scrivere su SqlLite file\n14)Scrivere su SqlServer db\n15)Visualizzare gli attori che hanno uno stipedio maggiore del salario medio\n16)Visualizzare la data di un film in base al titolo\n17)Get name actors who most partecipate at film\n18)Per chiudere il programma");
                Console.WriteLine("");
                string command = Console.ReadLine();

                switch (command)
                {
                    case "1":
                        await ut.CreateTables();
                        break;

                    case "2":
                        await ut.DropTables();
                        break;

                    case "3":
                        ActorList = ut.GetActors().Result;

                        if (ActorList.Count() == 0)
                        {
                            Console.WriteLine("List is empty");
                            break;
                        }

                        ActorList.ToList().ForEach(actor => Console.WriteLine(actor.GetInfo()));
                        break;

                    case "4":
                        Console.WriteLine("Inserire Id");
                        int Id = int.Parse(Console.ReadLine());

                        Console.WriteLine("Inserire ActorName");
                        string ActorName = Console.ReadLine();

                        Console.WriteLine("Inserire Actorlastname");
                        string ActorLastName = Console.ReadLine();

                        Console.WriteLine("Inserire ActorBibliography");
                        string ActorBibliography = Console.ReadLine();

                        Console.WriteLine("Inserire ActorTaxIdCode");
                        string ActorTaxIdCode = Console.ReadLine();

                        Console.WriteLine("Inserire anno di nascita");
                        int year = int.Parse(Console.ReadLine());

                        //Console.WriteLine("Inserire mese di nascita");
                        //int day = int.Parse(Console.ReadLine());

                        //Console.WriteLine("Inserire giorno di nascita");
                        //int month = int.Parse(Console.ReadLine());

                        //DateTime ActorDateBirth = new DateTime(year, day, month);

                        await ut.InsertIntoActor(new Actor {
                            Id = Id,
                            ActorName = ActorName,
                            ActorLastName = ActorLastName,
                            ActorBibliography = ActorBibliography,
                            ActorTaxIdCode = ActorTaxIdCode,
                            ActorYear = year
                        });

                        break;

                    case "5":
                        Console.WriteLine("");
                        Console.WriteLine("Inserire indice dell'elemento da modificare");
                        int index = int.Parse(Console.ReadLine());
                        ActorList[index].ActorName = "Luis";
                        await ut.UpdateActor(ActorList[index]);
                        break;

                    case "6":
                        Console.WriteLine("Inserire indice dell'elemento da eliminare");
                        int ind = int.Parse(Console.ReadLine());
                        await ut.DeleteActor(ActorList[ind]);
                        break;

                    case "7":
                        await ut.Filter(ut.Query());
                        break;

                    case "8":

                        var query = ActorList.GroupBy(actor => actor.ActorLastName);
                        groupactor = query.ToDictionary(g => g.Key, g => g.ToList());

                        List<string> keysgroup = groupactor.Keys.ToList();

                        int count = 0;


                        while (count < keysgroup.Count())
                        {
                            Console.WriteLine($"key:{keysgroup[count]}");
                            groupactor.TryGetValue(keysgroup[count], out List<Actor> values);

                            foreach (var actor in values)
                            {
                                Console.WriteLine($"(Name:{actor.ActorName}, LastName:{actor.ActorLastName}, DateBirth:{actor.ActorYear}, Bibliography: {actor.ActorBibliography})");
                            }

                            count++;
                        }

                        break;

                    case "9":

                        Console.WriteLine("Inserire chiave per cui si vuole visualizzare la lista dei attori");
                        var key = Console.ReadLine();

                        if (groupactor.Count() == 0)
                            break;

                        if (groupactor.ContainsKey(key))
                        {
                            Console.WriteLine($"List of Actor by LastName:{key}");

                            groupactor.TryGetValue(key, out List<Actor> values);

                            foreach (var actor in values)
                            {
                                Console.WriteLine($"(Name:{actor.ActorName}, LastName:{actor.ActorLastName}, DateBirth:{actor.ActorYear}, Bibliography: {actor.ActorBibliography})");
                            }
                        }

                        break;

                    case "10":
                        bool isequal = false;
                        Console.WriteLine("Inserire anno ");
                        int yearselect = int.Parse(Console.ReadLine());

                        Console.WriteLine("Anno uguale :\n Yes \n No");
                        string result = Console.ReadLine();

                        if (result.Equals("Yes"))
                        {
                            isequal = true;
                        }

                        ActorList = ut.GetYoungActors(yearselect, isequal).Result;

                        foreach (var youngactor in ActorList)
                        {
                            Console.WriteLine($"(Name:{youngactor.ActorName}, LastName:{youngactor.ActorLastName}, DateBirth:{youngactor.ActorYear}, Bibliography: {youngactor.ActorBibliography})");
                        }

                        break;

                    case "11":
                        List<int> ActorsId = new List<int> {2, 4, 8, 16 };
                        ActorList = ut.GetActorsInList(ActorsId).Result;

                        foreach (var actor in ActorList)
                        {
                            Console.WriteLine($"(Name:{actor.ActorName}, LastName:{actor.ActorLastName}, DateBirth:{actor.ActorYear}, Bibliography: {actor.ActorBibliography})");
                        }

                        break;

                    case "12":
                        //  (Actor, string, Cast, string)[] arraytuple = await ut.GetTuple();
                        Console.WriteLine("Inserire età");
                        int age = int.Parse(Console.ReadLine());
                        var resultActor = ut.GetInfoActors(age).Result;
                        break;



                    case "13":
                        await ut.WriteSqlLiteFile();
                        break;

                    case "14":
                        await ut.WriteSqlServer();
                        break;

                    case "15":
                        ActorList = ut.getActorsWhoAreTheSalaryGreaterThenMediumSalary().Result;

                        ActorList.ToList().ForEach(actor => Console.WriteLine($"(Name:{actor.ActorName}, LastName:{actor.ActorLastName}, DateBirth:{actor.ActorYear}, Bibliography: {actor.ActorBibliography})"));

                        break;

                    case "16":
                        Console.WriteLine("Inserire titolo di un film");
                        string title = Console.ReadLine();
                        DateTime dateFilm = ut.getDateFilmByTitleFilm(title).Result;
                        break;

                    case "17":
                        List<string> actors = ut.GetActorsNameWhoMostPartecipateAtFilm().Result.ToList();
                        break;

                    case "18":
                        return;

                    default:
                        break;
                }

            }
            

        }
    }
}
