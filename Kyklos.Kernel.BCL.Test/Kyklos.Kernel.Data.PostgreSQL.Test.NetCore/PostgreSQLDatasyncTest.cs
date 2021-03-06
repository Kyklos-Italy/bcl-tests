﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Kyklos.Kernel.Core.Exceptions;
using Kyklos.Kernel.Data.Async;
using Kyklos.Kernel.Data.Async.Support;
using Kyklos.Kernel.Data.Test;
using Kyklos.Kernel.Data.Test.Entities;
using Npgsql;
using Xunit;
using Xunit.Sdk;
using FluentAssertions;
using System.Data;

namespace Kyklos.Kernel.Data.PostgreSQL.Test.NetCore
{
    public class PostgreSQLDatasyncTest : BaseDatasyncTest
    {
        protected override string Schema => "public";
        protected override string ProviderName => "PostgreSQL";

        private void Setup()
        {
            SetupCoreAsync().Wait();
        }

        public PostgreSQLDatasyncTest() : base(XUnitTestSupport.NetPlatformType.NETCORE)
        {
            //JsonConvert.DefaultSettings =
            //    () =>
            //        new JsonSerializerSettings
            //        {
            //            Formatting = Newtonsoft.Json.Formatting.None,
            //            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            //        };

            SetupCoreAsync().Wait();

            //Setup();
        }

        //private async Task SetupCore()
        //{
        //    await PrepareDB().ConfigureAwait(false);
        //    await GenerateScriptsForCreateAndDropSequence().ConfigureAwait(false);
        //    await AddTeams().ConfigureAwait(false);
        //    await AddDays().ConfigureAwait(false);
        //    await AddResults().ConfigureAwait(false);
        //    await AddMembers().ConfigureAwait(false);
        //    await AddJobs().ConfigureAwait(false);
        //    await AddReasons().ConfigureAwait(false);
        //    await AddJobTimes().ConfigureAwait(false);
        //}

        private async Task ReplaceDuplicateKey(IAsyncDao tDao, Day newDay, string newKey)
        {
            newDay.DayId = newKey;
            await tDao.InsertEntityAsync(newDay).ConfigureAwait(false);
        }


        protected override async Task GenerateScriptsForCreateAndDropSequence()
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


        //FillDayDataTableShouldBe()


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
            await CountAllResultsShouldBeN(6, Dao).ConfigureAwait(false);
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

        //CountAllResultsAfterFourAreDeletedShouldBe2()

        //UpdateGoalsHomeTeamShouldBe3()

        //UpsertTeamsShouldBe5Teams()

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

        //DeleteResultByIdShouldBeIdRes4()

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

        //InsertAndDeleteAResultInTransactionShouldBeIdRes7()

        //InsertDuplicateDayKeyInTransactionWithCorrectionShouldBeIdDay1()

        //InsertTwoDaysInTwoTransactionsShouldBeIdDay4IdDay1()



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
        public async Task CheckIfPKIndexIsAlsoUniqueShouldBeResults()
        {
            await CheckIfPKIndexIsAlsoUniqueShouldBeResultsCore().ConfigureAwait(false);
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

        [Fact]
        public async Task SelectMemberByIdAndPasswordShouldBe218()
        {
            string memberName = "LCasini";
            string password = "202cb962ac59075b964b07152d234b70";
            await SelectMemberByIdAndPasswordShouldBeOneMember(memberName, password).ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateHoursByJobTimeInJOB_TIMESshouldBe7()
        {
            JobTime jobTimeToUpdate =
            new JobTime
            {
                JobId = 2,
                MemberId = 213,
                DateOfWork = PrefixDate,
                AmountTimeToInvoice = null,
                FreeAmountTime = null,
                TimeNote = null,
                ReasonId = 4,
                Hours = 3,
            };
            await UpdateHoursByJobTimeInJOB_TIMEShouldBeInt(4, jobTimeToUpdate).ConfigureAwait(false);
        }
        [Fact]
        public async Task SelectJobTimesOfTheDayByDateOfWorkAndId218ShuoldBeTwoJobTimesOfTheDay()
        {
            await SelectJobTimesOfTheDayByDateOfWorkAndId218ShuoldBe().ConfigureAwait(false);
        }

        [Fact]
        public async Task SumHoursOfJobTimeAggregateByMemberIdAndDateOfWorkShuoldBe6()
        {
            await SumHoursOfJobTimeAggregateByMemberIdAndDateOfWorkShuoldBeCore().ConfigureAwait(false);
        }

        [Fact]
        public async Task CheckJobTimeExistShuoldTrue()
        {
            await CheckJobTimeExistShuoldBeCore().ConfigureAwait(false);
        }

        [Fact]
        public async Task ReaderCommittedNotReadDirtyData()
        {
            await DirtyRead(IsolationLevel.ReadCommitted);
        }

        [Fact]
        public async Task RepeatableReadNotReadDirtyData()
        {
            await DirtyRead(IsolationLevel.RepeatableRead);
        }

        [Fact]
        public async Task SerializableNotReadDirtyData()
        {
            await DirtyRead(IsolationLevel.Serializable);
        }

        [Fact]
        public async Task ReadCommittedShouldReadUpdatedRecord()
        {
            await RepeatableRead(IsolationLevel.ReadCommitted);
        }

        [Fact]
        public async Task RepeatableReaderShouldReturnSameResultAfeterUpdateRecord()
        {
            await RepeatableRead(IsolationLevel.RepeatableRead);
        }

        [Fact]
        public async Task SerializableShouldReturnSameResultAfeterUpdateRecord()
        {
            await RepeatableRead(IsolationLevel.Serializable);
        }

        [Fact]
        public async Task ReadCommittedShouldAllowDueConcurrentUpdateOnSameRow()
        {
            await UpdateOnSameRow(IsolationLevel.ReadCommitted);
        }

        [Fact]
        public void RepeatableReaderShouldNotAllowUpdateOnSameRow()
        {
            string postgresExpotionCode = "40001";
            Func<Task> function = this.Awaiting(x => x.UpdateOnSameRow(IsolationLevel.RepeatableRead));
            Func<Task> action = function;
            action.Should().Throw<PostgresException>().Where(x => x.SqlState == postgresExpotionCode);
        }

        [Fact]
        public void SerializableShouldNotAllowUpdateOnSameRow()
        {
            string postgresExpotionCode = "40001";
            Func<Task> function = this.Awaiting(x => x.UpdateOnSameRow(IsolationLevel.Serializable));
            Func<Task> action = function;
            action.Should().Throw<PostgresException>().Where(x => x.SqlState == postgresExpotionCode);
        }

        #region Private method for Isolation Level

        private async Task DirtyRead(IsolationLevel isolationLevel)
        {
            Team phantomTeam = new Team { TeamId = "1000", City = "Ghost", Name = "Phantom", President = "LittleCheese" };

            var existingTeams = await Dao.GetAllItemsArrayAsync<Team>();
            var affectedRow = 0;

            var t1 = Dao
                .DoInTransactionAsync
                (
                    async tDao =>
                    {
                        await tDao.InsertEntityAsync(phantomTeam, isolationLevel: System.Data.IsolationLevel.Serializable).ConfigureAwait(false);
                        affectedRow.Should().Be(1);
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        throw new Exception("Do rollback");
                    },
                    isolationLevel: isolationLevel
                );

            t1.IsCompleted.Should().BeFalse();
            var allItems = await Dao.GetAllItemsArrayAsync<Team>();
            allItems.Length.Should().Be(existingTeams.Length);
        }

        private async Task RepeatableRead(IsolationLevel isolationLevel)
        {
            Team fiorentinaTeamToUpdateInT2 = new Team { TeamId = "idFio", Name = "Fiorentina", City = "Florence", President = "Commisso" };
            Team fiorentinaTeamAfterT2 = new Team();
            Team fiorentinaTeamInT1 = new Team();

            Team existingFiorentinaTeam = await Dao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdateInT2.TeamId);

            var t1 = Dao
                .DoInTransactionAsync
                (
                    async tDao =>
                    {
                        fiorentinaTeamInT1 = await tDao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdateInT2.TeamId).ConfigureAwait(false);
                        fiorentinaTeamInT1.President.Should().Be(existingFiorentinaTeam.President);
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        fiorentinaTeamAfterT2 = await tDao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdateInT2.TeamId).ConfigureAwait(false);
                    },
                    isolationLevel: isolationLevel
                );

            var updateBuilder = Dao.NewUpdateTableBuilder<Team>().Set(x => x.President, x => fiorentinaTeamToUpdateInT2.President).Where(x => x.TeamId == fiorentinaTeamToUpdateInT2.TeamId);
            var affected = await Dao.UpdateTableAsync(updateBuilder).ConfigureAwait(false);
            var fiorentinaTeamAfterAllTCompleted = await Dao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdateInT2.TeamId);
            await t1;

            if (isolationLevel == IsolationLevel.ReadCommitted)
            {
                fiorentinaTeamAfterT2.President.Should().Be(fiorentinaTeamToUpdateInT2.President);
                return;
            }

            fiorentinaTeamAfterT2.President.Should().Be(fiorentinaTeamInT1.President);
        }

        private async Task UpdateOnSameRow(IsolationLevel isolationLevel)
        {
            Team fiorentinaTeamToUpdateInT1 = new Team { TeamId = "idFio", Name = "Fiorentina", City = "Florence", President = "Commisso" };
            Team fiorentinaTeamToUpdateInT2 = new Team { TeamId = "idFio", Name = "Fiorentina", City = "Florence", President = "Barone" };

            var existingFiorentinaTeam = await Dao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdateInT1.TeamId);

            var t1 = Dao
            .DoInTransactionAsync
            (
                async tDao =>
                {
                    await tDao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdateInT1.TeamId);
                    var updateBuilderAsync = tDao.NewUpdateTableBuilder<Team>().Set(x => x.President, x => fiorentinaTeamToUpdateInT1.President).Where(x => x.TeamId == fiorentinaTeamToUpdateInT1.TeamId);
                    await tDao.UpdateTableAsync(updateBuilderAsync).ConfigureAwait(false);
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            );

            var fiorentinaTeamAfterUpdateT1 = await Dao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdateInT2.TeamId);
            fiorentinaTeamAfterUpdateT1.President.Should().Be(existingFiorentinaTeam.President);

            t1.IsCompleted.Should().BeFalse();

            var updateBuilder = Dao.NewUpdateTableBuilder<Team>().Set(x => x.President, x => fiorentinaTeamToUpdateInT2.President).Where(x => x.TeamId == fiorentinaTeamToUpdateInT2.TeamId);
            await Dao.UpdateTableAsync(updateBuilder, isolationLevel: isolationLevel).ConfigureAwait(false);

            t1.IsCompleted.Should().BeTrue();

            Team fiorentinaTeamAfterUpdate = await Dao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdateInT2.TeamId);

            fiorentinaTeamAfterUpdate.President.Should().Be(fiorentinaTeamToUpdateInT2.President);
        }

        #endregion
    }
}
