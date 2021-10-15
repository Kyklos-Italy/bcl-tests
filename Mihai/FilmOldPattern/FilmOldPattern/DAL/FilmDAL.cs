using FilmOldPattern.Model;
using FilmOldPattern.PrintConsole;
using Kyklos.Kernel.DAL;
using Kyklos.Kernel.SpringSupport.Data;
using Kyklos.Kernel.SpringSupport.Data.Query;
using Kyklos.Kernel.Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
                .SelectStar()
                .From(new TableDef { Alias = aliasFilm, TableName = film.GetTableNameForEntity() })
                .Where()
                .Condition<Film>(aliasFilm, f => f.FilmKind, WhereOperator.EqualTo, kindFilm);

            try
            {
                return GetItemList<Film>(builder).ToEmptyListIfNull();
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
                .SelectStar()
                .From(new TableDef { Alias = aliasFilm, TableName = film.GetTableNameForEntity() });

            try
            {
                return GetItemList<Film>(builder).ToEmptyListIfNull();
            }
            catch (Exception ex)
            {
                throw BuildKyklosDALException(ex);
            }
        }

        public IList<string> GetTitleFilms(string kindFilm)
        {
            Film film = null;

            var builder =
                NewSqlQueryBuilder()
                .RegisterTableAlias<Film>(aliasFilm)
                .Select()
                .AddFieldsToSelect<Film>(aliasFilm, f => f.FilmTitle)
                .From(aliasFilm)
                .Where()
                .Condition<Film>(aliasFilm, f => f.FilmKind, WhereOperator.EqualTo, kindFilm);

            try
            {
                return QueryForListOfString(builder).ToEmptyListIfNull();
            }
            catch (Exception ex)
            {
                throw BuildKyklosDALException(ex);
            }
        }

        public IList<string> GetTitle(string kindFilm)
        {
            var builder =
                NewSqlQueryBuilder()
                .CustomSql
                (
                   $"SELECT FILMTITLE FROM FILMS WHERE FILMKIND = '{kindFilm}' "
                );


            try
            {
                List<Film> films = GetItemList<Film>(builder).ToList();
                List<string> titleFilm = new List<string>();

                films.ForEach(f => titleFilm.Add(f.FilmTitle));

                return titleFilm;
            }
            catch (Exception ex)
            {
                throw BuildKyklosDALException(ex);
            }
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
                .SelectStar()
                .From(new TableDef { Alias = aliasFilm, Schema = Schema, TableName = film.GetTableNameForEntity() })
                .InnerJoin(new TableDef { Alias = aliasCast, Schema = Schema, TableName = cast.GetTableNameForEntity() })
                .On()
                .JoinCondition(aliasFilm, idFilm, WhereOperator.EqualTo, aliasCast, filmId)
                .InnerJoin(new TableDef { Alias = aliasActor, Schema = Schema, TableName = actor.GetTableNameForEntity() })
                .On()
                .JoinCondition(aliasCast, actorId, WhereOperator.EqualTo, aliasActor, idActor)
                .Where()
                .Condition<Actor>(aliasActor, a => a.ActorName, WhereOperator.EqualTo, actorName);

            try
            {
                return GetItemList<Film>(builder).ToEmptyListIfNull();
            }
            catch (Exception ex)
            {
                throw BuildKyklosDALException(ex);
            }
        }

        public DateTime GetDateFilmByTitle(string filmtitle)
        {
            //string selectStr = @"SELECT {1} FROM {0}FILMS WHERE 1=1 ".FormatWith(Schema, DaoHelper.EscapeFieldName("DATE"));

            //var parameters = CreateDbParameters();
            //string paramName = string.Empty;
            //string paramName4Sql = string.Empty;


            //paramName = "filmtitle";
            //paramName4Sql = DaoHelper.GetParamNameForProvider(paramName);
            //selectStr += string.Format(" AND {1}(FILMTITLE) = {0} ", paramName4Sql, DaoHelper.SqlTextProvider[SqlTextConstantNames.UpperFunctionName]);
            //parameters.AddWithValue(paramName, filmtitle.ToUpperInvariant());

            Film film = null;

          

            var builder =
                NewSqlQueryBuilder()
                .Select()
                .RegisterTableAlias(aliasFilm, film.GetTableNameForEntity())
                .AddFieldsToSelect<Film>(aliasFilm, f => f.Date)
                .From(aliasFilm)
                .Where()
                .Condition<Film>(aliasFilm, f => f.FilmTitle, WhereOperator.EqualTo, filmtitle);


            try
            {
               return  this.ExecuteScalar<DateTime>(builder);


            }
            catch (Exception ex)
            {
                throw BuildKyklosDALException(ex);
            }

            return DateTime.Now;
        }

        public int GetCountFilm()
        {
            Film film = null;
            var builder =
                NewSqlQueryBuilder()
                .RegisterTableAlias(aliasFilm, film.GetTableNameForEntity())
                .CustomSql
                (
                    "SELECT COUNT(*)"
                )
                .From(aliasFilm);

            try
            {
                int countFilms = ExecuteScalar<int>(builder);

                return countFilms;
            }
            catch (Exception ex)
            {
                throw BuildKyklosDALException(ex);
            }
        }

        public void DeleteFilmByFilmTitle(string filmTitle)
        {
            Film film = null;

            var builder =
                NewSqlQueryBuilder()
                .CustomSql
                (
                    "DELETE FROM {0} WHERE FILMTITLE = '{1}'".FormatWith(film.GetTableNameForEntity(), filmTitle)
                );

            ExecuteNonQuery(builder);
        }

        public void ViewTable()
        {
            List<Film> films = this.GetFilms().ToList();
            Console.Clear();
            PrintItems.PrintLine(93);
            PrintItems.PrintRow(93, "ID", "Title", "Kind", "Plot", "Language", "Date");

            foreach (var film in films)
            {
                PrintItems.PrintLine(103);
                PrintItems.PrintRow(103, film.Id.ToString(), film.FilmTitle, film.FilmKind, film.FilmPlot, film.Language, film.Date.ToString("yyyy - MM - dd"));
                PrintItems.PrintLine(103);
            }
        }

        public int GetNextId()
        {
            Film film = null;
            var builder =
                 NewSqlQueryBuilder()
                 .RegisterTableAlias(aliasFilm, film.GetTableNameForEntity())
                 .CustomSql("SELECT MAX(ID)")
                 .From(aliasFilm);

            try
            {
                return this.ExecuteScalar<int>(builder);
            }
            catch (Exception ex)
            {
                throw BuildKyklosDALException(ex);
            }
        }


        public IList<string> FilmTitleWhereWasPartecipateSpecificActor(int ActorId) 
        {
            var parameters = CreateDbParameters();
            string parameterName = string.Empty;
            string parameterName4Sql = string.Empty;

            parameterName = "idActor";
            parameterName4Sql = DaoHelper.GetParamNameForProvider(parameterName);
            parameters.AddWithValue(parameterName4Sql, ActorId);

            Film film = null;
            //Actor actor = null;
            //Cast cast = null;

            string selectStr = @"SELECT DISTINCT FILMTITLE FROM {0} INNER JOIN CAST ON FILMS.ID = FILMID INNER JOIN ACTORS ON ACTORID = ACTORS.IDACTOR WHERE ACTORS.IDACTOR = {1}".FormatWith(film.GetTableNameForEntity(), parameters[0].Value);

            var builder =
                  NewSqlQueryBuilder()
                  .CustomSql(selectStr);


            


                   //.RegisterTableAlias(aliasFilm, film.GetTableNameForEntity())
                   //.RegisterTableAlias(aliasActor, actor.GetTableNameForEntity())
                   //.RegisterTableAlias(aliasCast, cast.GetTableNameForEntity())
                   //.Select()
                   //.Distinct()
                   //.AddFieldsToSelect<Film>(aliasFilm, x => x.FilmTitle)
                   //.From(aliasFilm)
                   //.InnerJoin(new TableDef {Alias = aliasCast, Schema = Schema, TableName = cast.GetTableNameForEntity() })
                   //.On()
                   //.JoinCondition(aliasFilm, film.GetFieldName( f => f.Id), WhereOperator.EqualTo, aliasCast, cast.GetFieldName(c => c.FilmId))
                   //.InnerJoin(new TableDef {Alias = aliasActor, Schema = Schema, TableName = actor.GetTableNameForEntity() })
                   //.On()
                   //.JoinCondition(aliasCast, cast.GetFieldName(c => c.ActorId), WhereOperator.EqualTo ,aliasActor, actor.GetFieldName(a => a.Id))
                   //.Where()
                   //.Condition<Actor>(aliasActor, a => a.Id, WhereOperator.EqualTo, idActor);

            return QueryForListOfString(builder);
        }

        public Tuple<IList<Film>, IList<Actor>, IList<Cast>> GetTuple()
        {
            var multiBuilder = NewMultiSqlQueryBuilder();

            var filmSqlQueryBuilder =
                multiBuilder
                .NewSqlQueryBuilder()
                .CustomSql
                (
                    "SELECT * FROM FILMS"
                );

            var ActorSqlQueryBuilder =
                multiBuilder
                .NewSqlQueryBuilder()
                .CustomSql
                (
                     "SELECT * FROM ACTORS"
                );

            var CastSqlQueryBuilder =
                multiBuilder
                .NewSqlQueryBuilder()
                .CustomSql
                (
                    "SELECT * FROM CAST"
                );

            try
            {
                return GetItemList<Film, Actor, Cast>(multiBuilder);
            }
            catch (Exception ex)
            {
                throw BuildKyklosDALException(ex);
            }
        }
    }
}
