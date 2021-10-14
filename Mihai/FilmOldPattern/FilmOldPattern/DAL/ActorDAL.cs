using FilmOldPattern.Model;
using FilmOldPattern.PrintConsole;
using Kyklos.Kernel.DAL;
using Kyklos.Kernel.SpringSupport.Data;
using Kyklos.Kernel.SpringSupport.Data.Query;
using Kyklos.Kernel.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmOldPattern.DAL
{
    public class ActorDAL : NewBaseDal, IActorDAL
    {
        private readonly string aliasActor = "actor";

        public void InsertActor(Actor actor)
        {
            this.InsertEntity(actor);
        }

        public void UpdateFilm(Actor actor)
        {
            this.UpdateEntity(actor);
        }

        public void ViewTable()
        {
            List<Actor> actors = this.GetActors().ToList();
            Console.Clear();
            PrintItems.PrintLine(93);
            PrintItems.PrintRow(93, "ID", "Name", "LastName", "Bibliography", "TaxIdCode", "Year");

            foreach (var actor in actors)
            {
                PrintItems.PrintLine(103);
                PrintItems.PrintRow(103, actor.Id.ToString(), actor.ActorName, actor.ActorLastName, actor.ActorBibliography, actor.ActorTaxIdCode, actor.ActorYear.ToString());
                PrintItems.PrintLine(103);
            }
        }

        public void DeleteActor(Actor actor)
        {
            this.DeleteEntity(actor);
        }

        public void DeleteActorByActorName(string actorName)
        {
            Actor actor = null;

            var builder =
                NewSqlQueryBuilder()
                .CustomSql
                (
                    "DELETE FROM {0}  WHERE ACTORNAME = '{1}'".FormatWith(actor.GetTableNameForEntity(), actorName)
                );

            ExecuteNonQuery(builder);

        }

        public IList<Actor> GetActorByYear(int year)
        {
            Actor actor = null;

            var builder =
                 NewSqlQueryBuilder()
                 .Select()
                 .From(new TableDef { Alias = aliasActor, TableName = actor.GetTableNameForEntity() })
                 .Where()
                 .Condition<Actor>(aliasActor, a => a.ActorYear, WhereOperator.EqualTo, year);

            try
            {
                return this.GetItemList<Actor>(builder).ToEmptyListIfNull();
            }
            catch (Exception ex)
            {
                throw BuildKyklosDALException(ex);
            }
        }

        public IList<Actor> GetActors()
        {
            Actor actor = null;

            var builder =
                 NewSqlQueryBuilder()
                 .SelectStar()
                 .From(new TableDef { Alias = aliasActor, TableName = actor.GetTableNameForEntity() });

            return this.GetItemList<Actor>(builder).ToEmptyListIfNull();
        }

        public IList<Actor> GetActorsByActorTaxIdCode(string actorTaxIdCode)
        {
            Actor actor = null;

            var builder =
                 NewSqlQueryBuilder()
                 .Select()
                 .From(new TableDef { Alias = aliasActor, TableName = actor.GetTableNameForEntity() })
                 .Where()
                 .Condition<Actor>(aliasActor, a => a.ActorTaxIdCode, WhereOperator.EqualTo, actorTaxIdCode);

            try
            {
                return this.GetItemList<Actor>(builder).ToEmptyListIfNull();
            }
            catch (Exception ex) 
            {
                throw BuildKyklosDALException(ex);
            }
        }

        public IList<string> GETActorTaxIdCode()
        {
            Film film = null;

            var builder =
                NewSqlQueryBuilder()
                .RegisterTableAlias<Actor>(aliasActor)
                .Select()
                .AddFieldsToSelect<Actor>(aliasActor, a => a.ActorTaxIdCode)
                .From(aliasActor);

            try
            {
                return QueryForListOfString(builder).ToEmptyListIfNull();
            }
            catch (Exception ex)
            {
                throw BuildKyklosDALException(ex);
            }
        }

        public int GetCountActors()
        {
            Actor actor = null;
            var builder =
                NewSqlQueryBuilder()
                .RegisterTableAlias(aliasActor, actor.GetTableNameForEntity())
                .CustomSql
                (
                    "SELECT COUNT(*)"
                )
                .From(aliasActor);

            try
            {
                return this.ExecuteScalar<int>(builder);
            }
            catch (Exception ex) 
            {
                throw BuildKyklosDALException(ex);
            }
        }

        public int GetNextId()
        {
            Actor actor = null;
            var builder =
                 NewSqlQueryBuilder()
                 .RegisterTableAlias(aliasActor, actor.GetTableNameForEntity())
                 .CustomSql("SELECT MAX(ID)")
                 .From(aliasActor);

            try
            {
                return this.ExecuteScalar<int>(builder);
            }
            catch (Exception ex) 
            {
                throw BuildKyklosDALException(ex);
            }
        }
    }
}
