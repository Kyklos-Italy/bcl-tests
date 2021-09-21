using FilmOldPattern.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmOldPattern.DAL
{
    public interface IFilmDAL
    {
        void InsertFilm(Film film);
        void DeleteFilm(Film film);
        void UpdateFilm(Film film);

        IList<Film> GetFilms();
        IList<Film> GetFilmByKind(string kindFilm);

        string[] GetTitleFilms(string kindFilm);
    }
}
