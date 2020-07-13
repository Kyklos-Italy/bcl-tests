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

namespace Kyklos.Kernel.Data.SqlServer.Test.NetFramework
{
    public class SqlServerDatsyncTest : BaseDatasyncTest
    {
        protected override string Schema => "dbo";
        protected override string ProviderName => "SqlServer";

        private void Setup()
        {
            SetupCoreAsync().Wait();
        }

        public SqlServerDatsyncTest() : base(XUnitTestSupport.NetPlatformType.NETFRAMEWORK)
        {
            Setup();
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
            string createSequenceScript = @"CREATE SEQUENCE [dbo].[my_sequence]
                                           INCREMENT BY 1
                                           MINVALUE 1 MAXVALUE 1000
                                           START WITH 1";

            string dropSequenceScript = "DROP SEQUENCE [dbo].[my_sequence]";

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
            CheckDbProviderNameShouldBeCore("SqlServer");
        }


        [Fact]
        public void SqlInnerJoinShouldBe()
        {
            string sqlJoin = "dbo.RESULTS r inner join dbo.DAYS d on (r.DAY_ID = d.DAY_ID)" +
                              " inner join dbo.TEAMS ht on (r.HOME_TEAM_ID = ht.TEAM_ID)" +
                              " inner join dbo.TEAMS vt on (r.VISITOR_TEAM_ID = vt.TEAM_ID)";

            SqlInnerJoinShouldBeCore(sqlJoin);
        }


        [Fact]
        public void SqlLeftJoinShouldBe()
        {
            string sqlJoin = "dbo.RESULTS r left join dbo.DAYS d on (r.DAY_ID = d.DAY_ID)" +
                              " left join dbo.TEAMS ht on (r.HOME_TEAM_ID = ht.TEAM_ID)" +
                              " left join dbo.TEAMS vt on (r.VISITOR_TEAM_ID = vt.TEAM_ID)";

            SqlLeftJoinShouldBeCore(sqlJoin);
        }


        [Fact]
        public void SqlRightJoinShouldBe()
        {
            string sqlJoin = "dbo.RESULTS r right join dbo.DAYS d on (r.DAY_ID = d.DAY_ID)" +
                              " right join dbo.TEAMS ht on (r.HOME_TEAM_ID = ht.TEAM_ID)" +
                              " right join dbo.TEAMS vt on (r.VISITOR_TEAM_ID = vt.TEAM_ID)";

            SqlRightJoinShouldBeCore(sqlJoin);
        }


        [Fact]
        public void SqlFullOuterJoinShouldBe()
        {
            string sqlJoin = "dbo.RESULTS r full outer join dbo.DAYS d on (r.DAY_ID = d.DAY_ID)" +
                              " full outer join dbo.TEAMS ht on (r.HOME_TEAM_ID = ht.TEAM_ID)" +
                              " full outer join dbo.TEAMS vt on (r.VISITOR_TEAM_ID = vt.TEAM_ID)";

            SqlFullOuterJoinShouldBeCore(sqlJoin);
        }


        [Fact]
        public void SqlInnerJoinWithEscapeShouldBeCore()
        {
            string sqlJoin = "[dbo].[RESULTS] r inner join [dbo].[DAYS] d on (r.[DAY_ID] = d.[DAY_ID])" +
                              " inner join [dbo].[TEAMS] ht on (r.[HOME_TEAM_ID] = ht.[TEAM_ID])" +
                              " inner join [dbo].[TEAMS] vt on (r.[VISITOR_TEAM_ID] = vt.[TEAM_ID])";

            SqlInnerJoinShouldBeCore(sqlJoin, false);
        }


        [Fact]
        public void SqlLeftJoinWithEscapeShouldBeCore()
        {
            string sqlJoin = "[dbo].[RESULTS] r left join [dbo].[DAYS] d on (r.[DAY_ID] = d.[DAY_ID])" +
                              " left join [dbo].[TEAMS] ht on (r.[HOME_TEAM_ID] = ht.[TEAM_ID])" +
                              " left join [dbo].[TEAMS] vt on (r.[VISITOR_TEAM_ID] = vt.[TEAM_ID])";

            SqlLeftJoinShouldBeCore(sqlJoin, false);
        }


        [Fact]
        public void SqlRightJoinWithEscapeShouldBeCore()
        {
            string sqlJoin = "[dbo].[RESULTS] r right join [dbo].[DAYS] d on (r.[DAY_ID] = d.[DAY_ID])" +
                              " right join [dbo].[TEAMS] ht on (r.[HOME_TEAM_ID] = ht.[TEAM_ID])" +
                              " right join [dbo].[TEAMS] vt on (r.[VISITOR_TEAM_ID] = vt.[TEAM_ID])";

            SqlRightJoinShouldBeCore(sqlJoin, false);
        }


        [Fact]
        public void SqlFullOuterJoinWithEscapeShouldBeCore()
        {
            string sqlJoin = "[dbo].[RESULTS] r full outer join [dbo].[DAYS] d on (r.[DAY_ID] = d.[DAY_ID])" +
                              " full outer join [dbo].[TEAMS] ht on (r.[HOME_TEAM_ID] = ht.[TEAM_ID])" +
                              " full outer join [dbo].[TEAMS] vt on (r.[VISITOR_TEAM_ID] = vt.[TEAM_ID])";

            SqlFullOuterJoinShouldBeCore(sqlJoin, false);
        }


        [Fact]
        public void GetNextValueForSequenceShouldBe()
        {
            GetNextValueForSequenceShouldBeCore(1L, "[dbo].[my_sequence]").Wait();
        }


        [Fact]
        public void GetRangeValuesForSequenceShouldBe()
        {
            long[] range = new long[]
            {
                1L, 2L, 3L, 4L, 5L
            };

            GetRangeValuesForSequenceShouldBeCore(range, "[dbo].[my_sequence]").Wait();
        }


        [Fact]
        public void GetResultTableMetadataFromQueryShouldBe()
        {
            string sql = "SELECT r.GOALS_HOME_TEAM, r.GOALS_VISITOR_TEAM FROM RESULTS r";

            GetResultTableMetadataFromQueryShouldBeCore(sql).Wait();
        }


        [Fact]
        public  void FillDayDataTableShouldBe()
        {
            string sql = "SELECT d.* FROM DAYS d";

            FillDayDataTableShouldBeCore(sql).Wait();
        }


        [Fact]
        public void GetScriptForCreateTableShouldBe()
        {
            string sql = "create table [dbo].[TEAMS] ([TEAM_ID] nvarchar(50) not null, [NAME] nvarchar(200) not null, [CITY] nvarchar(150) not null, [PRESIDENT] nvarchar(120) not null, constraint TEAMS_pk primary key ([TEAM_ID]))";

            GetScriptForCreateTableShouldBeCore(sql, false, typeof(Team));
        }


        [Fact]
        public void GetScriptForDropTableShouldBe()
        {
            string sql = "drop table [dbo].[RESULTS]";

            GetScriptForDropTableShouldBeCore(sql, "RESULTS");
        }


        [Fact]
        public void GetScriptsForForeignKeyShouldBe()
        {
            string[] sql = new string[]
            {
                "alter table [dbo].[RESULTS] add constraint [HOME_TEAM_REF] foreign key ([HOME_TEAM_ID]) references [dbo].[TEAMS]([TEAM_ID])",
                "alter table [dbo].[RESULTS] add constraint [VIS_TEAM_REF] foreign key ([VISITOR_TEAM_ID]) references [dbo].[TEAMS]([TEAM_ID])",
                "alter table [dbo].[RESULTS] add constraint [DAY_REF] foreign key ([DAY_ID]) references [dbo].[DAYS]([DAY_ID])"
            };

            GetScriptsForForeignKeyShouldBeCore(sql, typeof(Result));
        }


        [Fact]
        public void GetScriptForUniqueConstraintShouldBe()
        {
            string[] sql = new string[]
            {
                "create unique index UNIQUE_DAY on [dbo].[DAYS] ([DAY_NUMBER])"
            };

            GetScriptForUniqueConstraintShouldBeCore(sql, typeof(Day));
        }


        [Fact]
        public void GetScriptForCreateNonUniqueIndexShouldBe()
        {
            string[] sql = new string[]
            {
                "create index TEAM_INDEX on [dbo].[TEAMS] ([NAME], [CITY])"
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
            await SelectDayDatesAsStringsShouldBeCore("yyyy-MM-dd hh:mm:ss");
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
            bool actualBool = ContainsEscapeShouldBeCore('[', myDao, true);
            Assert.False(actualBool);
        }


        [Fact]
        public void NotIgnoreDaoEscapeShouldBe()
        {
            IAsyncDao myDao = AsyncDaoFactory.CreateAsyncDao(ConnectionString, ProviderName, Schema, ignoreEscape: true);
            bool actualBool = ContainsEscapeShouldBeCore('[', myDao, false);
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
        public async void CountAllResultsAfterFourAreDeletedShouldBe2()
        {
            await CountAllResultsAfterFourAreDeletedShouldBe1Core();
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
        public async Task ReadCommittedShouldReadPhantomData()
        {

            Team phantomTeam = new Team { TeamId = "1000", City = "Ghost", Name = "Phantom", President = "LittleCheese" };
            var existingTeams = await Dao.GetAllItemsArrayAsync<Team>();

            var t = Dao
                .DoInTransactionAsync
                (
                    async tDao =>
                    {
                        await tDao.InsertEntityAsync(phantomTeam).ConfigureAwait(false);
                        await Task.Delay(TimeSpan.FromSeconds(10));
                        throw new Exception("Do rollback");
                    }
                );

            await Task.Delay(TimeSpan.FromSeconds(3));
            Assert.False(t.IsCompleted);
            var data2 = await Dao.GetAllItemsArrayAsync<Team>(isolationLevel: System.Data.IsolationLevel.ReadUncommitted);
            Assert.Equal(existingTeams.Length + 1, data2.Length);
        }

        [Fact]
        public async Task ReadCommitedNotShouldReadPhantomData()
        {

            Team phantomTeam = new Team { TeamId = "1000", City = "Ghost", Name = "Phantom", President = "LittleCheese" };
            var existingTeams = await Dao.GetAllItemsArrayAsync<Team>();

            var t = Dao
                .DoInTransactionAsync
                (
                    async tDao =>
                    {
                        await tDao.InsertEntityAsync(phantomTeam).ConfigureAwait(false);
                        throw new Exception("Do rollback");
                    }
                );

            await Task.Delay(TimeSpan.FromSeconds(3));
            Assert.True(t.IsCompleted);
            var data3 = await Dao.GetAllItemsArrayAsync<Team>();
            Assert.Equal(data3.Length, existingTeams.Length);
        }

        [Fact]
        public async Task ReadCommitedShouldReadEditedPresident()
        {
            var existingTeams = await Dao.GetAllItemsArrayAsync<Team>();
            var fiorentinaTeamToUpdate = new Team { TeamId = "idFio", Name = "Fiorentina", City = "Florence", President = "Commisso" };

            var t = Dao
                .DoInTransactionAsync
                (
                    async tDao =>
                    {
                        var updateBuilder = tDao.NewUpdateTableBuilder<Team>().Set(x => x.President, x => fiorentinaTeamToUpdate.President).Where(x => x.TeamId == fiorentinaTeamToUpdate.TeamId);
                        var affected = await tDao.UpdateTableAsync(updateBuilder).ConfigureAwait(false);
                        Assert.Equal(1, affected);
                        await Task.Delay(TimeSpan.FromSeconds(10));
                        throw new Exception("Do rollback");
                    }
                );


            await Task.Delay(TimeSpan.FromSeconds(3));
            Assert.False(t.IsCompleted);
            var data2 = await Dao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdate.TeamId, isolationLevel: System.Data.IsolationLevel.ReadUncommitted);
            Assert.Equal(fiorentinaTeamToUpdate.President, data2.President);
        }

        [Fact]
        public async Task ReadCommitedNotShouldReadEditedPresident()
        {
            var existingTeams = await Dao.GetAllItemsArrayAsync<Team>();
            var fiorentinaTeamToUpdate = new Team { TeamId = "idFio", Name = "Fiorentina", City = "Florence", President = "Commisso" };
            var existingFiorentinaTeam = await Dao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdate.TeamId).ConfigureAwait(false);

            int affected = 0;
            var t = Dao
                .DoInTransactionAsync
                (
                    async tDao =>
                    {
                        var updateBuilder = tDao.NewUpdateTableBuilder<Team>().Set(x => x.President, x => fiorentinaTeamToUpdate.President).Where(x => x.TeamId == fiorentinaTeamToUpdate.TeamId);
                        affected = await tDao.UpdateTableAsync(updateBuilder).ConfigureAwait(false);
                        Assert.Equal(1, affected);
                        throw new Exception("Do rollback");
                    }
                );

            await Task.Delay(TimeSpan.FromSeconds(3));
            Assert.True(t.IsCompleted);
            var data2 = await Dao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdate.TeamId).ConfigureAwait(false);
            Assert.Equal(existingFiorentinaTeam.President, data2.President);
        }

        [Fact]
        public async Task DeleteCommitedShouldDeletePhantomData()
        {
            Team phantomTeam = new Team { TeamId = "1000", City = "Ghost", Name = "Phantom", President = "LittleCheese" };

            var affected = await Dao.InsertEntityAsync(phantomTeam).ConfigureAwait(false);
            Assert.Equal(1, affected);

            var existingTeams = await Dao.GetAllItemsArrayAsync<Team>();

            var t = Dao
                .DoInTransactionAsync
                (
                    async tDao =>
                    {
                        affected = await tDao.DeleteEntityAsync(phantomTeam).ConfigureAwait(false);
                        await Task.Delay(TimeSpan.FromSeconds(10));
                        Assert.Equal(1, affected);
                        throw new Exception("Do rollback");
                    }
                );

            await Task.Delay(TimeSpan.FromSeconds(3));
            Assert.False(t.IsCompleted);
            var data2 = await Dao.GetAllItemsArrayAsync<Team>(isolationLevel: System.Data.IsolationLevel.ReadUncommitted);

            Assert.Equal(existingTeams.Length - 1, data2.Length);
        }

        [Fact]
        public async Task DeleteCommitedNotShouldDeletePhantomData()
        {
            Team phantomTeam = new Team { TeamId = "1000", City = "Ghost", Name = "Phantom", President = "LittleCheese" };

            var affected = await Dao.InsertEntityAsync(phantomTeam).ConfigureAwait(false);
            Assert.Equal(1, affected);

            var existingTeams = await Dao.GetAllItemsArrayAsync<Team>();

            var t = Dao
                .DoInTransactionAsync
                (
                    async tDao =>
                    {
                        affected = await tDao.DeleteEntityAsync(phantomTeam).ConfigureAwait(false);
                        Assert.Equal(1, affected);
                        throw new Exception("Do rollback");
                    }
                );

            await Task.Delay(TimeSpan.FromSeconds(3));
            Assert.True(t.IsCompleted);
            var data3 = await Dao.GetAllItemsArrayAsync<Team>();

            Assert.Equal(existingTeams.Length, data3.Length);
        }
    }
}
