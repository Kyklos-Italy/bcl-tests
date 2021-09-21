using FilmOldPattern.Model;
using System.Collections.Generic;

namespace FilmOldPattern.DAL
{
    public interface IFilmDAL
    {
        void InsertFilm(Film film);
        void DeleteFilm(Film film);
        void UpdateFilm(Film film);

        IList<Film> GetFilms();
        IList<Film> GetFilmByKind(string kindFilm);

        // IList<string> GetTitleFilms(string kindFilm);

        IList<Film> GetFilmsByActorName(string actorName);
    }
}
