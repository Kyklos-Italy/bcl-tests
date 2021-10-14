using FilmOldPattern.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmOldPattern.DAL
{
    public interface IActorDAL
    {
        void InsertActor(Actor actor);
        void DeleteActor(Actor actor);

        void DeleteActorByActorName(string actorName);

        void UpdateFilm(Actor actor);

        void ViewTable();

        IList<Actor> GetActors();
        IList<Actor> GetActorByYear(int year);

        IList<Actor> GetActorsByActorTaxIdCode(string actorTaxIdCode);

        IList<string> GETActorTaxIdCode();

        int GetCountActors();

        int GetNextId();
    }
}
