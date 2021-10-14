using Kyklos.Kernel.Data.Async.SqlBuilders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Film
{
    public interface IUtilities
    {
        Task DropTables();
        Task CreateTables();
        Task<IList<Actor>> GetActors();
        Task<IList<Film>> Filter(IQueryBuilder builder);
        Task UpdateActor(Actor actor);
        Task UpdateList(Actor actor);
        Task InsertIntoActor(Actor actor);
        Task DeleteActor(Actor actor);
        Task WriteSqlLiteFile();
        Task WriteSqlServer();
        IQueryBuilder Query();
        Task<IList<Actor>> GetYoungActors(int year, bool equalyear);
        Task<IList<Actor>> GetActorsInList(List<int> ActorId);
        Task<(Actor, string, Cast, string)[]> GetTuple();
        Task<string[]> GetActorNameByAge(int age);
        Task<Actor[]> GetInfoActors(int age);
        Task<IList<Actor>> getActorsWhoAreTheSalaryGreaterThenMediumSalary();
        Task<IList<Film>> GetFilmWherelanguageISIn(List<string> langaguages);
        Task<DateTime> getDateFilmByTitleFilm(string filmTitle);

        Task<IList<string>> GetActorsNameWhoMostPartecipateAtFilm();

        Task<IList<string>> GetFilmTitleWhereWasPartecipateSpecificActor(long? idActor);


    }
}
