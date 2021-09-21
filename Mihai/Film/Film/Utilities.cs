﻿using Kyklos.Kernel.Data.Support;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Kyklos.Kernel.Data.Async;
using Kyklos.Kernel.Data.Async.Support;
using Kyklos.Kernel.Data.Async.SqlBuilders;
using Kyklos.Kernel.Data.Query;
using Kyklos.Kernel.Core.KLinq;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Data;

namespace Film
{
    public class Utilities
    {
        private readonly IAsyncDao _dao;
        private readonly IAsyncDao _daosqllite;

        private ILogger Logger { get; }

        public Utilities(ILogger logger)
        {
            Logger = logger;
            _dao =
                AsyncDaoFactory
                .CreateAsyncDao
                (
                    "Data Source=192.168.25.11,1433;Initial Catalog=eProcurementScmX_Gucci_Test;Persist Security Info=True;User Id=sa;Password=KyKlos%2016!",
                    "SqlServer",
                    "dbo",
                    logger: logger
                );

            _daosqllite = AsyncDaoFactory
                .CreateSQLiteAsyncDao
                (
                "Data Source ={$ExecutionPath}\\Data\\test.db; Version = 3; FailIfMissing = false; Foreign Keys = True",
                logger: logger
                );
        }

        private static Type[] GetDbItemsTypes() =>
            new Type[]
            {
                typeof(Film),
                typeof(Actor),
                typeof(Cast),
            }
            .SortTablesByDependecies()
            .ToArray();


        public async Task DropTables()
        {
            foreach (var dbItemType in GetDbItemsTypes().Reverse())
            {
                await DropSingleTableAsync(dbItemType).ConfigureAwait(false);
            }
        }

        private async ValueTask DropSingleTableAsync(Type dbItemType)
        {
            var results = await _dao.DropTableAsync(dbItemType).ConfigureAwait(false);
            foreach (var res in results)
            {
                //Logger.Debug(res.Error, $"{res.Script.ScriptText}");
            }
        }



        public async Task CreateTables()
        {
            var ddlResults = await
                _dao
                .ExecDDLScriptsForTablesAsync(GetDbItemsTypes())
                .ConfigureAwait(false);

            foreach (var res in ddlResults)
            {
                //Logger.Debug(res.Error, $"{(res.Error is null ? "Success:" : "Error:")} {res.Script.ScriptText}");
            }
            //await Dao.CreateTableAsync<TeamDbItem>(tableName: "NewTeams");
        }


        public async Task<IList<Actor>> GetActors()
        {
            const string aliasActor = "actor";

            var builder =
                _dao
                .NewQueryBuilder()
                .Select()
                .AllFields<Actor>(aliasActor)
                .From<Actor>(aliasActor);
            //.Where<Actor>(aliasActor, x => x.Id, WhereOperator.EqualTo, 1);

            Actor[] result = await _dao.GetItemsArrayAsync<Actor>(builder).ConfigureAwait(false);
            result.PrintToConsole();

            return result.ToEmptyListIfNull();

        }


        public async Task<IList<Film>> Filter(IQueryBuilder builder)
        {
            //JoinCastFilm[] result = await _dao.GetItemsArrayAsync<JoinCastFilm>(builder).ConfigureAwait(false);
            //var group = result.GroupBy(x => x.FilmId);

            Film[] result = await _dao.GetItemsArrayAsync<Film>(builder).ConfigureAwait(false);
            return result.ToEmptyListIfNull();
        }

        public async Task UpdateActor(Actor actor)
        {
            await _dao.UpsertEntityAsync(actor);
        }


        public async Task UpdateList(Actor actor)
        {
            await _dao.UpdateEntityAsync(actor);
        }

        public async Task InsertIntoActor(Actor actor)
        {
            await _dao.InsertEntityAsync(actor);
        }

        public async Task DeleteActor(Actor actor)
        {
            await _dao.DeleteEntityAsync(actor);
        }

        public async Task WriteSqlLiteFile()
        {
            var ddlResults = await
                _daosqllite
                .ExecDDLScriptsForTablesAsync(GetDbItemsTypes())
                .ConfigureAwait(false);

            foreach (var res in ddlResults)
            {
                //Logger.Debug(res.Error, $"{(res.Error is null ? "Success:" : "Error:")} {res.Script.ScriptText}");
            }


            Actor[] resultActor = await _dao.GetAllItemsArrayAsync<Actor>();
            Film[] resultFilm = await _dao.GetAllItemsArrayAsync<Film>();
            Cast[] resultCast = await _dao.GetAllItemsArrayAsync<Cast>();

            await _daosqllite.WriteToServerAsync(resultActor);
            await _daosqllite.WriteToServerAsync(resultFilm);
            await _daosqllite.WriteToServerAsync(resultCast);

        }


        public async Task WriteSqlServer()
        {

            await DropTables();
            await CreateTables();



            Actor[] resultActor = await _daosqllite.GetAllItemsArrayAsync<Actor>();
            Film[] resultFilm = await _daosqllite.GetAllItemsArrayAsync<Film>();
            Cast[] resultCast = await _daosqllite.GetAllItemsArrayAsync<Cast>();

            await _dao.WriteToServerAsync(resultActor);
            await _dao.WriteToServerAsync(resultFilm);
            await _dao.WriteToServerAsync(resultCast);

        }



        public IQueryBuilder Query()
        {
            const string aliasCast = "cast";
            const string aliasFilm = "film";
            const string aliasActor = "actor";
            //var builder = _dao
            //             .NewQueryBuilder()
            //             .Select()
            //             .Star()
            //            .From()
            //            .Tables
            //            (
            //                FlatTable<Cast>.WithAlias(aliasCast),
            //                InnerJoin<Film>.WithAlias(aliasFilm),
            //                (cast, film) => cast.FilmId == film.Id
            //);

            var builder = _dao
                            .NewQueryBuilder()
                            .Select()
                            .AllFields<Film>(aliasFilm)
                            .From<Film>(aliasFilm)
                            .CustomSql
                            (
                                "WHERE EXISTS("
                            )
                            .Select()
                            .Star()
                            .From<Cast>(aliasCast)
                            .CustomSql
                            (
                                "WHERE " + aliasCast + ".FILMID = " + aliasFilm + ".ID)"
                            );



            return builder;
        }

        public async Task<IList<Actor>> GetYoungActors(int year, bool equalyear)
        {
            string aliasactor = "actors";
            Actor[] actors;

            var builder = _dao.NewQueryBuilder()
                             .Select()
                             .AllFields<Actor>(aliasactor)
                             .From<Actor>(aliasactor);


            if (equalyear)
            {
                builder = builder.Where<Actor>(aliasactor, a => a.ActorYear, WhereOperator.GreaterEqualThan, year);
                actors = await _dao.GetItemsArrayAsync<Actor>(builder);
            }

            else
            {
                builder = builder.Where<Actor>(aliasactor, a => a.ActorYear, WhereOperator.GreaterThan, year);
                actors = await _dao.GetItemsArrayAsync<Actor>(builder);
            }





            return actors.ToEmptyListIfNull();
        }

        public async Task<IList<Actor>> GetActorsInList(List<int> ActorId)
        {
            Actor[] actors = await _dao.GetItemsArrayByExampleAsync<Actor>
                            (
                                    a => a.Id.IsIn(ActorId)
                            )
                            .ConfigureAwait(false);

            return actors.ToEmptyListIfNull();

        }

        public async Task<(Actor, string, Cast, string)[]> GetTuple()
        {
            string aliasactor = "actor";
            string aliascast = "cast";

            try
            {
                var query = await _dao.NewQueryBuilder()
                            .Select()
                            .AllFields<Actor>(aliasactor).Comma()
                            .Field<Actor>(aliasactor, a => a.ActorName, "ACTORNAME").Comma()
                            .AllFields<Cast>(aliascast).Comma()
                            .Field<Cast>(aliascast, c => c.Role, "ACTORROLE")
                            .From()
                            .Tables
                            (
                                FlatTable<Actor>.WithAlias(aliasactor),
                                InnerJoin<Cast>.WithAlias(aliascast),
                                (a, c) => a.Id == c.ActorId
                            )
                            .OrderBy<Actor>(aliasactor, a => a.Id)
                            .GetItemsArrayAsync<(Actor Actor, string Actorname, Cast Cast, string Role)>()
                            .ConfigureAwait(false);

                return query;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return null;

        }

        public async Task<string[]> GetActorNameByAge(int age) 
        {
            const string aliasActor = "actor";

            var builder =
                _dao
                  .NewQueryBuilder()
                  .Select()
                  .Field<Actor>(aliasActor, a => a.ActorName)
                  .From<Actor>(aliasActor)
                  .Where<Actor>(aliasActor, a => a.ActorYear, WhereOperator.LessThan, age);

            string[] result = await _dao.GetItemsArrayAsync<string>(builder);

            return result.ToEmptyArrayIfNull();
        }

        public async Task<Actor[]> GetInfoActors(int age) 
        {
            const string aliasActor = "actor";

            var builder =
                 _dao
                   .NewQueryBuilder()
                   .Select()
                   .Field<Actor>(aliasActor, a => a.ActorName).Comma()
                   .Field<Actor>(aliasActor, a => a.ActorLastName)
                   .From<Actor>(aliasActor)
                   .Where<Actor>(aliasActor, a => a.ActorYear, WhereOperator.LessThan, age);


            var resultTuple = await _dao.GetItemsArrayAsync<Actor>(builder);

            return resultTuple.ToEmptyArrayIfNull();
        }

    }
}