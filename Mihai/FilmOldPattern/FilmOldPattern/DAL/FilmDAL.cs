using FilmOldPattern.Model;
using Kyklos.Kernel.DAL;
using Kyklos.Kernel.SpringSupport.Data;
using Kyklos.Kernel.SpringSupport.Data.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmOldPattern.DAL
{
    public class FilmDAL : NewBaseDal, IFilmDAL
    {
        private const string aliasFilm = "film";


        public void InsertFilm(Film film)
        {
            this.InsertEntity(film);
        }

        public void UpdateFilm(Film film)
        {
            this.UpdateEntity(film);
        }

        public void DeleteFilm(Film film)
        {
            this.DeleteEntity(film);
        }

        public IList<Film> GetFilmByKind(string kindFilm)
        {
            Film film = null;

            var builder =
                NewSqlQueryBuilder()
                .CustomSql(film.BuildSelectForEntity("FILMS", Schema, false, DaoHelper.EscapeFieldName, true))
                .Where()
                .True()
                .AndCondition(aliasFilm, film.GetFieldName(x => x.FilmKind), WhereOperator.EqualTo, kindFilm);

            try
            {
                return GetItemList<Film>(builder);
            }
            catch (Exception ex)
            {
                throw BuildKyklosDALException(ex);
            }
        }

        public IList<Film> GetFilms()
        {
            Film film = null;
            var builder =
                NewSqlQueryBuilder()
                .CustomSql(film.BuildSelectForEntity(aliasFilm, Schema, false, DaoHelper.EscapeFieldName));

            try
            {
                return this.GetItemList<Film>(builder);
            }
            catch (Exception ex)
            {
                throw BuildKyklosDALException(ex);
            }
        }
        
    }
}
