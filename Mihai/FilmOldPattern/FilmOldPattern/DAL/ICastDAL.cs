using FilmOldPattern.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmOldPattern.DAL
{
    public interface ICastDAL
    {
        void InsertCast(Cast cast);
        void DeleteCast(Cast cast);
        void UpdateCast(Cast cast);
        void ViewTable();
        IList<Cast> GetCasts();
        int GetCountCast();

        int GetNextId();


    }
}
