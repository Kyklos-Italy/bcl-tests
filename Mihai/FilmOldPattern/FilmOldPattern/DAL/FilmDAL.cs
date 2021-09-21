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
        private const string aliasActor = "actors";
        private const string aliasCast = "casts";
        private const string aliasFilm = "films";

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
                .CustomSql(film.BuildSelectForEntity(aliasFilm, Schema, false, DaoHelper.EscapeFieldName, true))
                .Where()
                .Condition<Film>(aliasFilm, f => f.FilmKind, WhereOperator.EqualTo, kindFilm);

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

        public string[] GetTitleFilms(string kindFilm)
        {
            throw new NotImplementedException();
        }

        public IList<Film> GetFilmsByActorName(string actorName)
        {
            Actor actor = null;
            Cast cast = null;
            Film film = null;

            string idFilm = film.GetFieldName(f => f.Id);
            string filmId = cast.GetFieldName(c => c.FilmId);
            string actorId = cast.GetFieldName(c => c.ActorId);
            string idActor = actor.GetFieldName(a => a.Id);


            var builder =
                NewSqlQueryBuilder()
                .CustomSql(film.BuildSelectForEntity(aliasFilm, Schema, false, DaoHelper.EscapeFieldName, true))
                .InnerJoin(new TableDef { Alias = aliasCast, Schema = Schema, TableName = cast.GetTableNameForEntity() })
                .On()
                .JoinCondition("FILMS", idFilm, WhereOperator.EqualTo, "CAST", filmId)
                .InnerJoin(new TableDef { Alias = aliasActor, Schema = Schema, TableName = actor.GetTableNameForEntity() })
                .On()
                .JoinCondition("CAST", actorId, WhereOperator.EqualTo, "ACTORS", idActor)
                .Where()
                .Condition(aliasActor, actor.GetFieldName(a => a.ActorName), WhereOperator.EqualTo, actorName);

            try
            {
                return GetItemList<Film>(builder);
            }
            catch (Exception ex)
            {
                throw BuildKyklosDALException(ex);
            }
        }
    }
}
