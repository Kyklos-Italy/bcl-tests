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
        void InsertFilm();
        void DeleteFilm();
        void UpdateFilm();

        IList<Film> GetFilms();
        IList<Film> GetFilmByKind(string kindFilm);
    }
}
