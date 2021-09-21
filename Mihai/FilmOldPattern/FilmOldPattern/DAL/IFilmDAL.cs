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
        IList<Film> GetFilms();
        void InsertFilm();
        void DeleteFilm();
        void UpdateFilm();

    }
}
