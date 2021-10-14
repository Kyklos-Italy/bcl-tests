using FilmOldPattern.Model;
using System;
using System.Collections.Generic;

namespace FilmOldPattern.DAL
{
    public interface IFilmDAL
    {
        void InsertFilm(Film film);
        void DeleteFilm(Film film);
        void DeleteFilmByFilmTitle(string filmTitle);
        void UpdateFilm(Film film);

        void ViewTable();

        IList<Film> GetFilms();
        IList<Film> GetFilmByKind(string kindFilm);

        IList<string> GetTitleFilms(string kindFilm);

        IList<Film> GetFilmsByActorName(string actorName);

        IList<string> GetTitle(string kindFilm);

        DateTime GetDateFilmByTitle(string filmtitle);

        int GetCountFilm();

        int GetNextId();


        IList<string> FilmTitleWhereWasPartecipateSpecificActor(int idActor);


        Tuple<IList<Film>, IList<Actor>, IList<Cast>> GetTuple();
    }
}
