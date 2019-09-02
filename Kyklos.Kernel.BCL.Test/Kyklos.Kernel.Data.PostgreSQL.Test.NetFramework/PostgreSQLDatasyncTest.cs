using System;
using System.Threading;
using System.Threading.Tasks;
using Kyklos.Kernel.Core.Exceptions;
using Kyklos.Kernel.Data.Async;
using Kyklos.Kernel.Data.Async.Support;
using Kyklos.Kernel.Data.Test;
using Kyklos.Kernel.Data.Test.Entities;
using Xunit;
using Xunit.Sdk;

namespace Kyklos.Kernel.Data.PostgreSQL.Test.NetFramework
{
    public class PostgreSQLDatasyncTest : BaseDatasyncTest
    {
        protected override string Schema => "public";
        protected override string ConnectionString =>ConnectionStringsProvider.GetConnectionStringProviderList("PostgreSql");

        protected override string ProviderName => "PostgreSQL";

        private void Setup()
        {
            SetupCore().Wait();
        }

        public PostgreSQLDatasyncTest() : base(XUnitTestSupport.NetPlatformType.NETFRAMEWORK)
        {
            Setup();
        }

        private async Task SetupCore()
        {
            await PrepareDB().ConfigureAwait(false);
            await GenerateScriptsForCreateAndDropSequence().ConfigureAwait(false);
            await AddTeams().ConfigureAwait(false);
            await AddDays().ConfigureAwait(false);
            await AddResults().ConfigureAwait(false);
        }

        private async Task ReplaceDuplicateKey(IAsyncDao tDao, Day newDay, string newKey)
        {
            newDay.DayId = newKey;
            await tDao.InsertEntityAsync(newDay).ConfigureAwait(false);
        }


        private async Task GenerateScriptsForCreateAndDropSequence()
        {
            string createSequenceScript = @"CREATE SEQUENCE ""my_sequence""
                                           INCREMENT BY 1
                                           MINVALUE 1 MAXVALUE 1000
                                           START WITH 1";

            string dropSequenceScript = @"DROP SEQUENCE ""my_sequence""";

            await PrepareSequence(createSequenceScript, dropSequenceScript).ConfigureAwait(false);
        }


        [Fact]
        public void CheckIfDbSupportsValuesForFastInConditionShouldBe()
        {
            CheckIfDbSupportsValuesForFastInConditionShouldBeCore(true);
        }


        [Fact]
        public void CheckDbProviderNameShouldBe()
        {
            CheckDbProviderNameShouldBeCore("PostgreSQL");
        }


        [Fact]
        public void SqlInnerJoinShouldBe()
        {
            string sqlJoin = "public.RESULTS r inner join public.DAYS d on (r.DAY_ID = d.DAY_ID)" +
                              " inner join public.TEAMS ht on (r.HOME_TEAM_ID = ht.TEAM_ID)" +
                              " inner join public.TEAMS vt on (r.VISITOR_TEAM_ID = vt.TEAM_ID)";

            SqlInnerJoinShouldBeCore(sqlJoin);
        }


        [Fact]
        public void SqlLeftJoinShouldBe()
        {
            string sqlJoin = "public.RESULTS r left join public.DAYS d on (r.DAY_ID = d.DAY_ID)" +
                              " left join public.TEAMS ht on (r.HOME_TEAM_ID = ht.TEAM_ID)" +
                              " left join public.TEAMS vt on (r.VISITOR_TEAM_ID = vt.TEAM_ID)";

            SqlLeftJoinShouldBeCore(sqlJoin);
        }


        [Fact]
        public void SqlRightJoinShouldBe()
        {
            string sqlJoin = "public.RESULTS r right join public.DAYS d on (r.DAY_ID = d.DAY_ID)" +
                              " right join public.TEAMS ht on (r.HOME_TEAM_ID = ht.TEAM_ID)" +
                              " right join public.TEAMS vt on (r.VISITOR_TEAM_ID = vt.TEAM_ID)";

            SqlRightJoinShouldBeCore(sqlJoin);
        }


        [Fact]
        public void SqlFullOuterJoinShouldBe()
        {
            string sqlJoin = "public.RESULTS r full outer join public.DAYS d on (r.DAY_ID = d.DAY_ID)" +
                              " full outer join public.TEAMS ht on (r.HOME_TEAM_ID = ht.TEAM_ID)" +
                              " full outer join public.TEAMS vt on (r.VISITOR_TEAM_ID = vt.TEAM_ID)";

            SqlFullOuterJoinShouldBeCore(sqlJoin);
        }


        [Fact]
        public void SqlInnerJoinWithEscapeShouldBeCore()
        {
            string sqlJoin = @"""public"".""RESULTS"" r inner join ""public"".""DAYS"" d on (r.""DAY_ID"" = d.""DAY_ID"")" +
                              @" inner join ""public"".""TEAMS"" ht on (r.""HOME_TEAM_ID"" = ht.""TEAM_ID"")" +
                              @" inner join ""public"".""TEAMS"" vt on (r.""VISITOR_TEAM_ID"" = vt.""TEAM_ID"")";

            SqlInnerJoinShouldBeCore(sqlJoin, false);
        }


        [Fact]
        public void SqlLeftJoinWithEscapeShouldBeCore()
        {
            string sqlJoin = @"""public"".""RESULTS"" r left join ""public"".""DAYS"" d on (r.""DAY_ID"" = d.""DAY_ID"")" +
                              @" left join ""public"".""TEAMS"" ht on (r.""HOME_TEAM_ID"" = ht.""TEAM_ID"")" +
                              @" left join ""public"".""TEAMS"" vt on (r.""VISITOR_TEAM_ID"" = vt.""TEAM_ID"")";

            SqlLeftJoinShouldBeCore(sqlJoin, false);
        }


        [Fact]
        public void SqlRightJoinWithEscapeShouldBeCore()
        {
            string sqlJoin = @"""public"".""RESULTS"" r right join ""public"".""DAYS"" d on (r.""DAY_ID"" = d.""DAY_ID"")" +
                              @" right join ""public"".""TEAMS"" ht on (r.""HOME_TEAM_ID"" = ht.""TEAM_ID"")" +
                              @" right join ""public"".""TEAMS"" vt on (r.""VISITOR_TEAM_ID"" = vt.""TEAM_ID"")";

            SqlRightJoinShouldBeCore(sqlJoin, false);
        }


        [Fact]
        public void SqlFullOuterJoinWithEscapeShouldBeCore()
        {
            string sqlJoin = @"""public"".""RESULTS"" r full outer join ""public"".""DAYS"" d on (r.""DAY_ID"" = d.""DAY_ID"")" +
                              @" full outer join ""public"".""TEAMS"" ht on (r.""HOME_TEAM_ID"" = ht.""TEAM_ID"")" +
                              @" full outer join ""public"".""TEAMS"" vt on (r.""VISITOR_TEAM_ID"" = vt.""TEAM_ID"")";

            SqlFullOuterJoinShouldBeCore(sqlJoin, false);
        }


        [Fact]
        public void GetNextValueForSequenceShouldBe()
        {
            GetNextValueForSequenceShouldBeCore(1L, @"""my_sequence""").Wait();
        }


        [Fact]
        public void GetRangeValuesForSequenceShouldBe()
        {
            long[] range = new long[]
            {
                1L, 2L, 3L, 4L, 5L
            };

            GetRangeValuesForSequenceShouldBeCore(range, @"""my_sequence""").Wait();
        }


        [Fact]
        public void GetResultTableMetadataFromQueryShouldBe()
        {
            string sql = @"SELECT r.""GOALS_HOME_TEAM"", r.""GOALS_VISITOR_TEAM"" FROM ""RESULTS"" r";

            GetResultTableMetadataFromQueryShouldBeCore(sql).Wait();
        }


        [Fact]
        public async Task FillDayDataTableShouldBe()
        {
            string sql = @"SELECT d.* FROM ""DAYS"" d";

            await FillDayDataTableShouldBeCore(sql);
        }


        [Fact]
        public void GetScriptForCreateTableShouldBe()
        {
            string sql = @"create table ""public"".""TEAMS"" (""TEAM_ID"" varchar(50) not null, ""NAME"" varchar(200) not null, ""CITY"" varchar(150) not null, ""PRESIDENT"" varchar(120) not null, constraint TEAMS_pk primary key (""TEAM_ID""))";

            GetScriptForCreateTableShouldBeCore(sql, false, typeof(Team));
        }


        [Fact]
        public void GetScriptForDropTableShouldBe()
        {
            string sql = @"drop table ""public"".""RESULTS""";

            GetScriptForDropTableShouldBeCore(sql, "RESULTS");
        }


        [Fact]
        public void GetScriptsForForeignKeyShouldBe()
        {
            string[] sql = new string[]
            {
                @"alter table ""public"".""RESULTS"" add constraint ""HOME_TEAM_REF"" foreign key (""HOME_TEAM_ID"") references ""public"".""TEAMS""(""TEAM_ID"")",
                @"alter table ""public"".""RESULTS"" add constraint ""VIS_TEAM_REF"" foreign key (""VISITOR_TEAM_ID"") references ""public"".""TEAMS""(""TEAM_ID"")",
                @"alter table ""public"".""RESULTS"" add constraint ""DAY_REF"" foreign key (""DAY_ID"") references ""public"".""DAYS""(""DAY_ID"")"
            };

            GetScriptsForForeignKeyShouldBeCore(sql, typeof(Result));
        }


        [Fact]
        public void GetScriptForUniqueConstraintShouldBe()
        {
            string[] sql = new string[]
            {
                @"create unique index UNIQUE_DAY on ""public"".""DAYS"" (""DAY_NUMBER"")"
            };

            GetScriptForUniqueConstraintShouldBeCore(sql, typeof(Day));
        }


        [Fact]
        public void GetScriptForCreateNonUniqueIndexShouldBe()
        {
            string[] sql = new string[]
            {
                @"create index TEAM_INDEX on ""public"".""TEAMS"" (""NAME"", ""CITY"")"
            };

            GetScriptForCreateNonUniqueIndexShouldBeCore(sql, typeof(Team));
        }


        private async Task CreateIndexes()
        {
            string scriptIndex1 = $@"CREATE INDEX ""RESULT_DAY_INDEX"" ON ""{Schema}"".""RESULTS""(""DAY_ID"")";
            string scriptIndex2 = $@"CREATE INDEX ""RESULT_GOALS_INDEX"" ON ""{Schema}"".""RESULTS""(""GOALS_HOME_TEAM"", ""GOALS_VISITOR_TEAM"")";
            await ExecuteScript(scriptIndex1).ConfigureAwait(false);
            await ExecuteScript(scriptIndex2).ConfigureAwait(false);
        }


        [Fact]
        public async Task CancellationOfGetCompleteResultShouldBe()
        {
            await Assert
               .ThrowsAsync<TaskCanceledException>
               (
               async () =>
               {
                   try
                   {
                       using (CancellationTokenSource tokenSource = new CancellationTokenSource(100))
                       {
                           await CancellationOfGetCompleteResultShouldBeCore(20, tokenSource.Token).ConfigureAwait(false);
                       }
                   }
                   catch (Exception ex)
                   {
                       throw ex.UnwrapAggregateException();
                   }
               }
           )
           .ConfigureAwait(false);
        }


        [Fact]
        public async Task SelectDayDatesAsStringsShouldBeyyyy_MM_dd_HH_mm_ss()
        {
            await SelectDayDatesAsStringsShouldBeCore("yyyy-MM-dd HH:mm:ss");
        }


        [Fact]
        public async Task SelectDayDatesAsStringsWithIncorrectFormatShouldBe()
        {
            await Assert
               .ThrowsAsync<EqualException>
               (
               async () =>
               {
                   try
                   {
                       await SelectDayDatesAsStringsShouldBeCore("gfdgfd").ConfigureAwait(false);
                   }
                   catch (Exception ex)
                   {
                       throw ex.UnwrapAggregateException();
                   }
               }
           )
           .ConfigureAwait(false);
        }



        [Fact]
        public void IgnoreDaoEscapeShouldBe()
        {
            IAsyncDao myDao = AsyncDaoFactory.CreateAsyncDao(ConnectionString, ProviderName, Schema, ignoreEscape: true);
            bool actualBool = ContainsEscapeShouldBeCore('"', myDao, true);
            Assert.False(actualBool);
        }


        [Fact]
        public void NotIgnoreDaoEscapeShouldBe()
        {
            IAsyncDao myDao = AsyncDaoFactory.CreateAsyncDao(ConnectionString, ProviderName, Schema, ignoreEscape: true);
            bool actualBool = ContainsEscapeShouldBeCore('"', myDao, false);
            Assert.True(actualBool);
        }

        [Fact]
        public async Task CountAllTeamsShouldBe4()
        {
            await CountAllTeamsShouldBe4Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task CountAllDaysShouldBe3()
        {
            await CountAllDaysShouldBe3Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task CountAllResultsShouldBe6()
        {
            await CountAllResultsShouldBeN(6,Dao).ConfigureAwait(false);
        }

        [Fact]
        public async Task SelectResultsWhereResultIdIsIdRes4ShouldBe()
        {
            await SelectResultsWhereResultIdIsIdRes4ShouldBeCore().ConfigureAwait(false);
        }

        [Fact]
        public async Task SelectTeamsWhereGoalsVisitorTeamIsGreaterThan2ShouldBeJuveAndFiore()
        {
            await SelectTeamsWhereGoalsVisitorTeamIsGreaterThan2ShouldBeJuveAndFioreCore().ConfigureAwait(false);
        }

        [Fact]
        public async Task SelectHomeTeamsGoalsShouldBe4Teams()
        {
            await SelectHomeTeamsGoalsShouldBe4TeamsCore().ConfigureAwait(false);
        }

        [Fact]
        public async Task SelectTotalTeamsGoalsShouldBe4Teams()
        {
            await SelectTotalTeamsGoalsShouldBe4TeamsCore().ConfigureAwait(false);
        }

        [Fact]
        public async Task SelectHomeTeamsWhereConditionsAreMetShouldBe3Teams()
        {
            await SelectHomeTeamsWhereConditionsAreMetShouldBe3TeamsCore().ConfigureAwait(false);
        }

        [Fact]
        public async Task SelectTeamsNameLikeShouldBe3Teams()
        {
            await SelectTeamsNameLikeShouldBe3TeamsCore().ConfigureAwait(false);
        }

        [Fact]
        public void CountAllResultsAfterFourAreDeletedShouldBe2()
        {
            CountAllResultsAfterFourAreDeletedShouldBe1Core();
        }

        [Fact]
        public async Task UpdateGoalsHomeTeamShouldBe3()
        {
            await UpdateGoalsHomeTeamShouldBe3Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task UpsertTeamsShouldBe5Teams()
        {
            await UpsertTeamsShouldBe5TeamsCore().ConfigureAwait(false);
        }

        [Fact]
        public async Task SelectWithFastInConditionShouldGet2Teams()
        {
            await SelectWithFastInConditionShouldGet2TeamsCore().ConfigureAwait(false);
        }

        [Fact]
        public async Task SumOfGoalsHomeTeamShouldBe11()
        {
            await SumOfGoalsHomeTeamShouldBe11Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task SumOfGoalsVisitorTeamShouldBe12()
        {
            await SumOfGoalsVisitorTeamShouldBe12Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task SumOfTotalGoalsForResultShouldBeOK()
        {
            await SumOfTotalGoalsForResultShouldBeOKCore().ConfigureAwait(false);
        }

        [Fact]
        public async Task SumOfTotalGoalsShouldBe23()
        {
            await SumOfTotalGoalsShouldBe23Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task MaxOfGoalsHomeTeamShouldBe4()
        {
            await MaxOfGoalsHomeTeamShouldBe4Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task MaxOfGoalsVisitorTeamShouldBe3()
        {
            await MaxOfGoalsVisitorTeamShouldBe3Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task MaxOfTotalGoalsShouldBe7()
        {
            await MaxOfTotalGoalsShouldBe7Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task MinOfGoalsHomeTeamShouldBeZero()
        {
            await MinOfGoalsHomeTeamShouldBeZeroCore().ConfigureAwait(false);
        }

        [Fact]
        public async Task MinOfGoalsVisitorTeamShouldBe1()
        {
            await MinOfGoalsVisitorTeamShouldBe1Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task MinOfTotalGoalsShouldBe1()
        {
            await MinOfTotalGoalsShouldBe1Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task AvgOfGoalsHomeTeamShouldBe1Point83()
        {
            await AvgOfGoalsHomeTeamShouldBe1Point83Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task AvgOfGoalsVisitorTeamShouldBe2()
        {
            await AvgOfGoalsVisitorTeamShouldBe2Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task AvgOfTotalGoalsShouldBe3Point83()
        {
            await AvgOfTotalGoalsShouldBe3Point83Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task GetCompleteResultShouldBe6Results()
        {
            await GetCompleteResultShouldBe6ResultsCore().ConfigureAwait(false);
        }

        [Fact]
        public async Task SelectTheFirstTwoResultsShouldBeIdRes1IdRes2()
        {
            await SelectTheFirstTwoResultsShouldBeIdRes1IdRes2Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task SelectTheLastTwoResultsShouldBeIdRes5IdRes6()
        {
            await SelectTheLastTwoResultsShouldBeIdRes5IdRes6Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task SelectResultsWithMaxSumOfGoalsShouldBeIdRes3()
        {
            await SelectResultsWithMaxSumOfGoalsShouldBeIdRes3Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task SelectTheFirstDayWithTupleExceptionShouldBe()
        {
            await SelectTheFirstDayWithTupleExceptionShouldBeCore().ConfigureAwait(false);
        }

        [Fact]
        public async Task SelectResultsByIdWithDictionaryExceptionShouldBe()
        {
            await SelectResultsByIdWithDictionaryExceptionShouldBeCore().ConfigureAwait(false);
        }

        [Fact]
        public async Task SelectResultsByIdWithDictionaryShouldBeIdRes4()
        {
            await SelectResultsByIdWithDictionaryShouldBeIdRes4Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task SumOfTotalGoalsForResultWithDictionaryShouldBeIdRes1IdRes2IdRes3IdRes4IdRes5IdRes6()
        {
            await SumOfTotalGoalsForResultWithDictionaryShouldBeIdRes1IdRes2IdRes3IdRes4IdRes5IdRes6Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task SelectTeamFromConcatConditionShouldBeIdJuv()
        {
            await SelectTeamFromConcatConditionShouldBeIdJuvCore().ConfigureAwait(false);
        }

        [Fact]
        public async Task DeleteResultByIdShouldBeIdRes4()
        {
            await DeleteResultByIdShouldBeIdRes4Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task CheckIfADayExistsShouldBeIdDay1()
        {
            await CheckIfADayExistsShouldBeIdDay1Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task CheckIfTheErrorIsASyntaxErrorShouldBeTrue()
        {
            await CheckIfTheErrorIsASyntaxErrorShouldBeTrueCore().ConfigureAwait(false);
        }

        [Fact]
        public async Task CheckIfTheErrorIsADuplicateKeyErrorShouldBeIdDay1()
        {
            await CheckIfTheErrorIsADuplicateKeyErrorShouldBeIdDay1Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task GetTeamTableMetadataShouldBeFour()
        {
            await GetTeamTableMetadataShouldBeFourCore().ConfigureAwait(false);
        }

        [Fact]
        public async Task InsertAndDeleteAResultInTransactionShouldBeIdRes7()
        {
            await InsertAndDeleteAResultInTransactionShouldBeIdRes7Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task InsertDuplicateDayKeyInTransactionWithCorrectionShouldBeIdDay1()
        {
            await InsertDuplicateDayKeyInTransactionWithCorrectionShouldBeIdDay1Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task InsertTwoDaysInTwoTransactionsShouldBeIdDay4IdDay1()
        {
            await InsertTwoDaysInTwoTransactionsShouldBeIdDay4IdDay1Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task GetTableColumnNamesShouldBeSixStrings()
        {
            await GetTableColumnNamesShouldBeSixStringsCore().ConfigureAwait(false);
        }

        [Fact]
        public async Task GetEntityObjectNameShouldBeTeams()
        {
            await GetEntityObjectNameShouldBeTeamsCore().ConfigureAwait(false);
        }

        [Fact]
        public async Task GetResultsTotalPaginationNumberShouldBe6()
        {
            await GetResultsTotalPaginationNumberShouldBe6Core().ConfigureAwait(false);
        }

        [Fact]
        public void CheckIfPKIndexIsAlsoUniqueShouldBeResults()
        {
            CheckIfPKIndexIsAlsoUniqueShouldBeResultsCore();
        }

        [Fact]
        public async Task SelectResultsForADayShouldBeDay1()
        {
            await SelectResultsForADayShouldBeDay1Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task SelectResultsForADayWithInvertedTuplesShouldBeDay1()
        {
            await SelectResultsForADayWithInvertedTuplesShouldBeDay1Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task SelectResultsForADayWithIncompletedFieldsShouldBeDay1()
        {
            await SelectResultsForADayWithIncompletedFieldsShouldBeDay1Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task SelectDoubleFirstDayShouldBeDay1()
        {
            await SelectDoubleFirstDayShouldBeDay1Core().ConfigureAwait(false);
        }


    }
}
