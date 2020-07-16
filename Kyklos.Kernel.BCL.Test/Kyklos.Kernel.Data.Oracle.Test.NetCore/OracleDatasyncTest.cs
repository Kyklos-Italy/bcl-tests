using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kyklos.Kernel.Core.Exceptions;
using Kyklos.Kernel.Core.Support;
using Kyklos.Kernel.Data.Async;
using Kyklos.Kernel.Data.Async.SqlBuilders;
using Kyklos.Kernel.Data.Async.Support;
using Kyklos.Kernel.Data.Test;
using Kyklos.Kernel.Data.Test.Entities;
using Oracle.ManagedDataAccess.Client;
using Xunit;
using FluentAssertions;


namespace Kyklos.Kernel.Data.Oracle.Test.NetCore
{
    public class OracleDatasyncTest : BaseDatasyncTest
    {
        protected override string Schema => "RMX_MORATO_DEV";
        protected override string ProviderName => "Oracle";


        public OracleDatasyncTest() : base(XUnitTestSupport.NetPlatformType.NETCORE)
        {
            //JsonConvert.DefaultSettings = 
            //    () => 
            //        new JsonSerializerSettings
            //        {
            //            Formatting = Newtonsoft.Json.Formatting.None,
            //            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            //        };

            SetupCoreAsync().Wait();
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
            string createSequenceScript = @"CREATE SEQUENCE ""RMX_MORATO_DEV"".""my_sequence"" 
                                           START WITH 1
                                           INCREMENT BY 1
                                           MINVALUE 1 MAXVALUE 1000";

            string dropSequenceScript = @"DROP SEQUENCE ""RMX_MORATO_DEV"".""my_sequence""";

            await PrepareSequence(createSequenceScript, dropSequenceScript).ConfigureAwait(false);
        }


        [Fact]
        public void CheckIfDbSupportsValuesForFastInConditionShouldBe()
        {
            CheckIfDbSupportsValuesForFastInConditionShouldBeCore(false);
        }


        [Fact]
        public void CheckDbProviderNameShouldBe()
        {
            CheckDbProviderNameShouldBeCore("Oracle");
        }


        [Fact]
        public void SqlInnerJoinShouldBe()
        {
            string sqlJoin = "RMX_MORATO_DEV.RESULTS r inner join RMX_MORATO_DEV.DAYS d on (r.DAY_ID = d.DAY_ID)" +
                              " inner join RMX_MORATO_DEV.TEAMS ht on (r.HOME_TEAM_ID = ht.TEAM_ID)" +
                              " inner join RMX_MORATO_DEV.TEAMS vt on (r.VISITOR_TEAM_ID = vt.TEAM_ID)";

            SqlInnerJoinShouldBeCore(sqlJoin);
        }


        [Fact]
        public void SqlLeftJoinShouldBe()
        {
            string sqlJoin = "RMX_MORATO_DEV.RESULTS r left join RMX_MORATO_DEV.DAYS d on (r.DAY_ID = d.DAY_ID)" +
                              " left join RMX_MORATO_DEV.TEAMS ht on (r.HOME_TEAM_ID = ht.TEAM_ID)" +
                              " left join RMX_MORATO_DEV.TEAMS vt on (r.VISITOR_TEAM_ID = vt.TEAM_ID)";

            SqlLeftJoinShouldBeCore(sqlJoin);
        }


        [Fact]
        public void SqlRightJoinShouldBe()
        {
            string sqlJoin = "RMX_MORATO_DEV.RESULTS r right join RMX_MORATO_DEV.DAYS d on (r.DAY_ID = d.DAY_ID)" +
                              " right join RMX_MORATO_DEV.TEAMS ht on (r.HOME_TEAM_ID = ht.TEAM_ID)" +
                              " right join RMX_MORATO_DEV.TEAMS vt on (r.VISITOR_TEAM_ID = vt.TEAM_ID)";

            SqlRightJoinShouldBeCore(sqlJoin);
        }


        [Fact]
        public void SqlFullOuterJoinShouldBe()
        {
            string sqlJoin = "RMX_MORATO_DEV.RESULTS r full outer join RMX_MORATO_DEV.DAYS d on (r.DAY_ID = d.DAY_ID)" +
                              " full outer join RMX_MORATO_DEV.TEAMS ht on (r.HOME_TEAM_ID = ht.TEAM_ID)" +
                              " full outer join RMX_MORATO_DEV.TEAMS vt on (r.VISITOR_TEAM_ID = vt.TEAM_ID)";

            SqlFullOuterJoinShouldBeCore(sqlJoin);
        }


        [Fact]
        public void SqlInnerJoinWithEscapeShouldBe()
        {
            string sqlJoin = @"""RMX_MORATO_DEV"".""RESULTS"" r inner join ""RMX_MORATO_DEV"".""DAYS"" d on (r.""DAY_ID"" = d.""DAY_ID"")" +
                              @" inner join ""RMX_MORATO_DEV"".""TEAMS"" ht on (r.""HOME_TEAM_ID"" = ht.""TEAM_ID"")" +
                              @" inner join ""RMX_MORATO_DEV"".""TEAMS"" vt on (r.""VISITOR_TEAM_ID"" = vt.""TEAM_ID"")";

            SqlInnerJoinShouldBeCore(sqlJoin, false);
        }


        [Fact]
        public void SqlLeftJoinWithEscapeShouldBe()
        {
            string sqlJoin = @"""RMX_MORATO_DEV"".""RESULTS"" r left join ""RMX_MORATO_DEV"".""DAYS"" d on (r.""DAY_ID"" = d.""DAY_ID"")" +
                              @" left join ""RMX_MORATO_DEV"".""TEAMS"" ht on (r.""HOME_TEAM_ID"" = ht.""TEAM_ID"")" +
                              @" left join ""RMX_MORATO_DEV"".""TEAMS"" vt on (r.""VISITOR_TEAM_ID"" = vt.""TEAM_ID"")";

            SqlLeftJoinShouldBeCore(sqlJoin, false);
        }


        [Fact]
        public void SqlRightJoinWithEscapeShouldBeCore()
        {
            string sqlJoin = @"""RMX_MORATO_DEV"".""RESULTS"" r right join ""RMX_MORATO_DEV"".""DAYS"" d on (r.""DAY_ID"" = d.""DAY_ID"")" +
                              @" right join ""RMX_MORATO_DEV"".""TEAMS"" ht on (r.""HOME_TEAM_ID"" = ht.""TEAM_ID"")" +
                              @" right join ""RMX_MORATO_DEV"".""TEAMS"" vt on (r.""VISITOR_TEAM_ID"" = vt.""TEAM_ID"")";

            SqlRightJoinShouldBeCore(sqlJoin, false);
        }


        [Fact]
        public void SqlFullOuterJoinWithEscapeShouldBeCore()
        {
            string sqlJoin = @"""RMX_MORATO_DEV"".""RESULTS"" r full outer join ""RMX_MORATO_DEV"".""DAYS"" d on (r.""DAY_ID"" = d.""DAY_ID"")" +
                              @" full outer join ""RMX_MORATO_DEV"".""TEAMS"" ht on (r.""HOME_TEAM_ID"" = ht.""TEAM_ID"")" +
                              @" full outer join ""RMX_MORATO_DEV"".""TEAMS"" vt on (r.""VISITOR_TEAM_ID"" = vt.""TEAM_ID"")";

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
            string sql = "SELECT r.GOALS_HOME_TEAM, r.GOALS_VISITOR_TEAM FROM RESULTS r";

            GetResultTableMetadataFromQueryShouldBeCore(sql).Wait();
        }


        [Fact]
        public async Task FillDayDataTableShouldBe()
        {
            string sql = "SELECT d.* FROM DAYS d";

            await FillDayDataTableShouldBeCore(sql);
        }


        [Fact]
        public void GetScriptForCreateTableShouldBe()
        {
            string sql = @"create table ""RMX_MORATO_DEV"".""TEAMS"" (""TEAM_ID"" nvarchar2(50) not null, ""NAME"" nvarchar2(200) not null, ""CITY"" nvarchar2(150) not null, ""PRESIDENT"" nvarchar2(120) not null, constraint TEAMS_pk primary key (""TEAM_ID""))";

            GetScriptForCreateTableShouldBeCore(sql, false, typeof(Team));
        }


        [Fact]
        public void GetScriptForDropTableShouldBe()
        {
            string sql = @"drop table ""RMX_MORATO_DEV"".""RESULTS""";

            GetScriptForDropTableShouldBeCore(sql, "RESULTS");
        }


        [Fact]
        public void GetScriptsForForeignKeyShouldBe()
        {
            string[] sql = new string[]
            {
                @"alter table ""RMX_MORATO_DEV"".""RESULTS"" add (constraint ""HOME_TEAM_REF"" foreign key (""HOME_TEAM_ID"") references ""RMX_MORATO_DEV"".""TEAMS""(""TEAM_ID""))",
                @"alter table ""RMX_MORATO_DEV"".""RESULTS"" add (constraint ""VIS_TEAM_REF"" foreign key (""VISITOR_TEAM_ID"") references ""RMX_MORATO_DEV"".""TEAMS""(""TEAM_ID""))",
                @"alter table ""RMX_MORATO_DEV"".""RESULTS"" add (constraint ""DAY_REF"" foreign key (""DAY_ID"") references ""RMX_MORATO_DEV"".""DAYS""(""DAY_ID""))"
            };

            GetScriptsForForeignKeyShouldBeCore(sql, typeof(Result));
        }


        [Fact]
        public void GetScriptForUniqueConstraintShouldBe()
        {
            string[] sql = new string[]
            {
                @"create unique index UNIQUE_DAY on ""RMX_MORATO_DEV"".""DAYS"" (""DAY_NUMBER"")"
            };

            GetScriptForUniqueConstraintShouldBeCore(sql, typeof(Day));
        }


        [Fact]
        public void GetScriptForCreateNonUniqueIndexShouldBe()
        {
            string[] sql = new string[]
            {
                @"create index TEAM_INDEX on ""RMX_MORATO_DEV"".""TEAMS"" (""NAME"", ""CITY"")"
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
        public async Task SelectDayDatesAsStringsWithIncorrectFormatShouldThrowsException()
        {
            await Assert
                .ThrowsAsync<OracleException>
                (
                    async () =>
                    {
                        try
                        {
                            await SelectDayDatesAsStringsShouldBeInOracleFormat("sdffsdf");
                        }
                        catch (Exception ex)
                        {
                            throw ex.UnwrapAggregateException();
                        }
                    }
                );
        }


        [Fact]
        public async Task SelectDayDatesAsStringsShouldBeyyyy_MM_dd_H_m_s()
        {
            await SelectDayDatesAsStringsShouldBeInOracleFormat("yyyy-MM-dd H:m:s");
        }

        private async Task SelectDayDatesAsStringsShouldBeInOracleFormat(string dateFormat)
        {
            var expectedDatesStrings = new string[]
            {
                "2018-09-16 00:00:00",
                "2018-09-17 00:00:00",
                "2018-09-18 00:00:00"
            };

            var queryBuilder =
                Dao
                    .NewQueryBuilder()
                        .Select()
                            .Field<Day>("d", x => x.DayDate.ToString(dateFormat))
                        .From()
                            .Table<Day>("d")
                        .OrderBy<Day>("d", x => x.DayDate);
                            //.Field<Day>("d", x => x.DayDate);

            string c = queryBuilder.ToString();
            var actualDatesString = (await Dao.GetItemsAsync<string>(queryBuilder).ConfigureAwait(false)).ToArray();
            Assert.Equal(expectedDatesStrings, actualDatesString);
        }

        //
        //[Fact]
        //public async Task SelectDayDatesAsStringsWithIncorrectFormatShouldBe()
        //{
        //    await Assert
        //       .ThrowsAsync<EqualException>
        //       (
        //       async () =>
        //       {
        //           try
        //           {
        //               await SelectDayDatesAsStringsShouldBeCore("gfdgfd").ConfigureAwait(false);
        //           }
        //           catch (Exception ex)
        //           {
        //               throw ex.UnwrapAggregateException();
        //           }
        //       }
        //   )
        //   .ConfigureAwait(false);
        //}


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
        public async Task ReaderCommittedNotReadDirtyData()
        {
            Team phantomTeam = new Team { TeamId = "1000", City = "Ghost", Name = "Phantom", President = "LittleCheese" };

            var existingTeams = await Dao.GetAllItemsArrayAsync<Team>();
            var affectedRow = 0;

            var t = Dao
                .DoInTransactionAsync
                (
                    async tDao =>
                    {
                        await tDao.InsertEntityAsync(phantomTeam).ConfigureAwait(false);
                        Assert.Equal(1, affectedRow);
                        await Task.Delay(TimeSpan.FromSeconds(10));
                        throw new Exception("Do rollback");
                    }
                );

            await Task.Delay(TimeSpan.FromSeconds(3));
            Assert.True(t.IsCompleted);
            var data3 = await Dao.GetAllItemsArrayAsync<Team>();
            Assert.Equal(existingTeams.Length, data3.Length);
        }

        [Fact]
        public async Task ReaderCommittedNotLockTable()
        {
            var fiorentinaTeamToUpdate = new Team { TeamId = "idFio", Name = "Fiorentina", City = "Florence", President = "Commisso" };
            var milanTeamToUpdate = new Team { TeamId = "idMil", Name = "Milan", City = "Milano", President = "Scaroni" };

            var t = Dao
            .DoInTransactionAsync
            (
                async tDao =>
                {
                    var dataT1 = await tDao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdate.TeamId);
                    var updateBuilderAsync = tDao.NewUpdateTableBuilder<Team>().Set(x => x.President, x => fiorentinaTeamToUpdate.President).Where(x => x.TeamId == fiorentinaTeamToUpdate.TeamId);
                    await tDao.UpdateTableAsync(updateBuilderAsync).ConfigureAwait(false);
                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
            );

            var updateBuilder = Dao.NewUpdateTableBuilder<Team>().Set(x => x.President, x => milanTeamToUpdate.President).Where(x => x.TeamId == milanTeamToUpdate.TeamId);
            await Dao.UpdateTableAsync(updateBuilder).ConfigureAwait(false);

            await t;

            var fiorentinaTeamAfterUpdate = await Dao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdate.TeamId);
            Assert.Equal(fiorentinaTeamToUpdate.President, fiorentinaTeamAfterUpdate.President);

            var milanTeamAfterUpdate = await Dao.GetItemByExampleAsync<Team>(x => x.TeamId == milanTeamToUpdate.TeamId);
            Assert.Equal(milanTeamToUpdate.President, milanTeamAfterUpdate.President);
        }

        [Fact]
        public async Task ReaderCommittedWaitCommittedT1BeforeUpdateSameRow()
        {
            var fiorentinaTeamToUpdateInT1 = new Team { TeamId = "idFio", Name = "Fiorentina", City = "Florence", President = "Commisso" };
            var fiorentinaTeamToUpdateInT2 = new Team { TeamId = "idFio", Name = "Fiorentina", City = "Florence", President = "Barone" };
            var fiorentinaTeamAfterT2 = new Team();
            var fiorentinaTeamInT1 = new Team();

            var existingFiorentinaTeam = await Dao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdateInT1.TeamId);

            var t = Dao
            .DoInTransactionAsync
            (
                async tDao =>
                {
                    var dataT1 = await tDao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdateInT1.TeamId);
                    var updateBuilderAsync = tDao.NewUpdateTableBuilder<Team>().Set(x => x.President, x => fiorentinaTeamToUpdateInT1.President).Where(x => x.TeamId == fiorentinaTeamToUpdateInT1.TeamId);
                    await tDao.UpdateTableAsync(updateBuilderAsync).ConfigureAwait(false);
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            );

            var fiorentinaTeamAfterUpdateT1 = await Dao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdateInT2.TeamId);
            Assert.Equal(existingFiorentinaTeam.President, fiorentinaTeamAfterUpdateT1.President);
            Assert.False(t.IsCompleted);
            var updateBuilder = Dao.NewUpdateTableBuilder<Team>().Set(x => x.President, x => fiorentinaTeamToUpdateInT2.President).Where(x => x.TeamId == fiorentinaTeamToUpdateInT2.TeamId);
            await Dao.UpdateTableAsync(updateBuilder).ConfigureAwait(false);
            Assert.True(t.IsCompleted);
            var fiorentinaTeamAfterUpdate = await Dao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdateInT2.TeamId);
            Assert.Equal(fiorentinaTeamToUpdateInT2.President, fiorentinaTeamAfterUpdate.President);
        }

        [Fact]
        public async Task SerializableNotReadDirtyData()
        {
            Team phantomTeam = new Team { TeamId = "1000", City = "Ghost", Name = "Phantom", President = "LittleCheese" };

            var existingTeams = await Dao.GetAllItemsArrayAsync<Team>();
            var affectedRow = 0;

            var t = Dao
                .DoInTransactionAsync
                (
                    async tDao =>
                    {
                        await tDao.InsertEntityAsync(phantomTeam, isolationLevel: System.Data.IsolationLevel.Serializable).ConfigureAwait(false);
                        Assert.Equal(1, affectedRow);
                        await Task.Delay(TimeSpan.FromSeconds(15));
                        throw new Exception("Do rollback");
                    },
                    isolationLevel: System.Data.IsolationLevel.Serializable
                );

            await Task.Delay(TimeSpan.FromSeconds(3));
            Assert.True(t.IsCompleted);
            var allteams = await Dao.GetAllItemsArrayAsync<Team>();
            Assert.Equal(existingTeams.Length, allteams.Length);
        }

        [Fact]
        public async Task SerializableNotLockTable()
        {
            var fiorentinaTeamToUpdate = new Team { TeamId = "idFio", Name = "Fiorentina", City = "Florence", President = "Commisso" };
            var milanTeamToUpdate = new Team { TeamId = "idMil", Name = "Milan", City = "Milano", President = "Scaroni" };

            var t = Dao
            .DoInTransactionAsync
            (
                async tDao =>
                {
                    var dataT1 = await tDao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdate.TeamId);
                    var updateBuilderAsync = tDao.NewUpdateTableBuilder<Team>().Set(x => x.President, x => fiorentinaTeamToUpdate.President).Where(x => x.TeamId == fiorentinaTeamToUpdate.TeamId);
                    await tDao.UpdateTableAsync(updateBuilderAsync).ConfigureAwait(false);
                    await Task.Delay(TimeSpan.FromSeconds(10));
                },
                isolationLevel: System.Data.IsolationLevel.Serializable
            );

            var updateBuilder = Dao.NewUpdateTableBuilder<Team>().Set(x => x.President, x => milanTeamToUpdate.President).Where(x => x.TeamId == milanTeamToUpdate.TeamId);
            await Dao.UpdateTableAsync(updateBuilder).ConfigureAwait(false);

            await t;

            var fiorentinaTeamAfterUpdate = await Dao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdate.TeamId);
            Assert.Equal(fiorentinaTeamToUpdate.President, fiorentinaTeamAfterUpdate.President);

            var milanTeamAfterUpdate = await Dao.GetItemByExampleAsync<Team>(x => x.TeamId == milanTeamToUpdate.TeamId);
            Assert.Equal(milanTeamToUpdate.President, milanTeamAfterUpdate.President);
        }

        [Fact]
        public void SerializableShouldNotLostUpdatesOnSameRow()
        {
            var function = this.Awaiting(x => x.SerializableShouldNotAllowDueConcurrentUpdateOnSameRow());
            Func<Task> action = function;
            action.Should().Throw<OracleException>().Where(x => x.Number == 8177);
        }

        private async Task SerializableShouldNotAllowDueConcurrentUpdateOnSameRow()
        {
            var fiorentinaTeamToUpdateInT1 = new Team { TeamId = "idFio", Name = "Fiorentina", City = "Florence", President = "Commisso" };
            var fiorentinaTeamToUpdateInT2 = new Team { TeamId = "idFio", Name = "Fiorentina", City = "Florence", President = "Barone" };
            var fiorentinaTeamAfterT2 = new Team();
            var fiorentinaTeamInT1 = new Team();

            var existingFiorentinaTeam = await Dao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdateInT1.TeamId);

            var t = Dao
            .DoInTransactionAsync
            (
                async tDao =>
                {
                    var dataT1 = await tDao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdateInT1.TeamId);
                    var updateBuilderAsync = tDao.NewUpdateTableBuilder<Team>().Set(x => x.President, x => fiorentinaTeamToUpdateInT1.President).Where(x => x.TeamId == fiorentinaTeamToUpdateInT1.TeamId);
                    await tDao.UpdateTableAsync(updateBuilderAsync).ConfigureAwait(false);
                    await Task.Delay(TimeSpan.FromSeconds(15));
                }
            );

            var fiorentinaTeamAfterUpdateT1 = await Dao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdateInT2.TeamId);
            Assert.Equal(existingFiorentinaTeam.President, fiorentinaTeamAfterUpdateT1.President);
            Assert.False(t.IsCompleted);
            var updateBuilder = Dao.NewUpdateTableBuilder<Team>().Set(x => x.President, x => fiorentinaTeamToUpdateInT2.President).Where(x => x.TeamId == fiorentinaTeamToUpdateInT2.TeamId);
            await Dao.UpdateTableAsync(updateBuilder, isolationLevel: System.Data.IsolationLevel.Serializable).ConfigureAwait(false);
            Assert.True(t.IsCompleted);
            var fiorentinaTeamAfterUpdate = await Dao.GetItemByExampleAsync<Team>(x => x.TeamId == fiorentinaTeamToUpdateInT2.TeamId);
            Assert.Equal(fiorentinaTeamToUpdateInT2.President, fiorentinaTeamAfterUpdate.President);
        }   
    }
}
