using FilmOldPattern.Model;
using FilmOldPattern.PrintConsole;
using Kyklos.Kernel.DAL;
using Kyklos.Kernel.SpringSupport.Data;
using Kyklos.Kernel.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmOldPattern.DAL
{
    public class CastDAL : NewBaseDal, ICastDAL
    {
        private readonly string aliasCast = "cast";

        public void DeleteCast(Cast cast)
        {
            this.DeleteEntity(cast);
        }

        public IList<Cast> GetCasts()
        {
            Cast cast = null;
            var builder =
                NewSqlQueryBuilder()
                .RegisterTableAlias(aliasCast, cast.GetTableNameForEntity())
                .SelectStar()
                .From(aliasCast);

            try
            {
                return this.GetItemList<Cast>(builder).ToEmptyListIfNull();
            }
            catch (Exception ex)
            {
                throw BuildKyklosDALException(ex);
            }
        }

        public int GetCountCast()
        {
            Cast cast = null;
            var builder =
                 NewSqlQueryBuilder()
                 .RegisterTableAlias(aliasCast, cast.GetTableNameForEntity())
                 .CustomSql
                 (
                     "SELECT COUNT(*)"
                 )
                 .From(aliasCast);

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
            Cast cast = null;
            var builder =
                 NewSqlQueryBuilder()
                 .RegisterTableAlias(aliasCast, cast.GetTableNameForEntity())
                 .CustomSql("SELECT MAX(ID)")
                 .From(aliasCast);

            try
            {
                return this.ExecuteScalar<int>(builder);
            }
            catch (Exception ex)
            {
                throw BuildKyklosDALException(ex);
            }
        }

        public void InsertCast(Cast cast)
        {
            this.InsertEntity(cast);
        }

        public void UpdateCast(Cast cast)
        {
            this.UpdateEntity(cast);
        }

        public void ViewTable()
        {
            List<Cast> casts = this.GetCasts().ToList();
            Console.Clear();
            PrintItems.PrintLine(93);
            PrintItems.PrintRow(93, "ID", "FILMID", "ACTORID", "ROLE");

            foreach (var cast in casts)
            {
                PrintItems.PrintLine(103);
                PrintItems.PrintRow(103, cast.Id.ToString(), cast.FilmId.ToString(), cast.ActorId.ToString(), cast.Role);
                PrintItems.PrintLine(103);
            }
        }
    }
}
