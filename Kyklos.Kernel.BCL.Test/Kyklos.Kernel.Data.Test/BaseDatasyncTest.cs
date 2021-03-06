﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kyklos.Kernel.Core.Exceptions;
using Kyklos.Kernel.Core.Support;
using Kyklos.Kernel.Data.Async;
using Kyklos.Kernel.Data.Async.SqlBuilders;
using Kyklos.Kernel.Data.Async.Support;
using Kyklos.Kernel.Data.Entities;
using Kyklos.Kernel.Data.Entities.Optimization;
using Kyklos.Kernel.Data.Query;
using Kyklos.Kernel.Data.Support;
using Kyklos.Kernel.Data.Test.Entities;
using Xunit;
using Xunit.Sdk;
using XUnitTestSupport;

namespace Kyklos.Kernel.Data.Test
{
    public abstract class BaseDatasyncTest
    {
        protected TestNetPlatform NetPlatform { get; }

        protected abstract string Schema { get; }

        protected const int ThresholdForFastInCondition = 10;

        protected IAsyncDao Dao { get; }

        protected BaseDatasyncTest(NetPlatformType netPlatformType)
        {
            NetPlatform = new TestNetPlatform(netPlatformType);
            Dao = CreateAsyncDao(ConnectionString, ProviderName, Schema);
        }

        protected abstract string ProviderName { get; }

        protected virtual string ConnectionString => ConnectionStringsProvider.GetConnectionString(ProviderName);

        #region Initial data sets

        private Result[] InitialResults =
            new Result[]
            {
                new Result() { ResultId = "idRes1", HomeTeamId = "idFio", VisitorTeamId = "idJuv", GoalsHomeTeam = 1, GoalsVisitorTeam = 3, DayId = "idDay1" },
                new Result() { ResultId = "idRes2", HomeTeamId = "idMil", VisitorTeamId = "idInt", GoalsHomeTeam = 0, GoalsVisitorTeam = 1, DayId = "idDay1" },
                new Result() { ResultId = "idRes3", HomeTeamId = "idInt", VisitorTeamId = "idFio", GoalsHomeTeam = 4, GoalsVisitorTeam = 3, DayId = "idDay2" },
                new Result() { ResultId = "idRes4", HomeTeamId = "idJuv", VisitorTeamId = "idMil", GoalsHomeTeam = 2, GoalsVisitorTeam = 2, DayId = "idDay2" },
                new Result() { ResultId = "idRes5", HomeTeamId = "idFio", VisitorTeamId = "idInt", GoalsHomeTeam = 0, GoalsVisitorTeam = 2, DayId = "idDay3" },
                new Result() { ResultId = "idRes6", HomeTeamId = "idMil", VisitorTeamId = "idJuv", GoalsHomeTeam = 4, GoalsVisitorTeam = 1, DayId = "idDay3" }
            };

        private Team[] InitialTeams =
            new Team[]
            {
                    new Team { TeamId = "idFio", Name = "Fiorentina", City = "Florence", President = "Della Valle" },
                    new Team { TeamId = "idJuv", Name = "Juventus", City = "Turin", President = "Agnelli" },
                    new Team { TeamId = "idMil", Name = "Milan", City = "Milan", President = "Tizio cinese" },
                    new Team { TeamId = "idInt", Name = "Inter", City = "Milan", President = "Thoir" }
            };

        private Day[] InitialDays =
            new Day[]
            {
                new Day() { DayId = "idDay1", DayDate = new DateTime(2018, 09, 16), DayNumber = 1 },
                new Day() { DayId = "idDay2", DayDate = new DateTime(2018, 09, 17), DayNumber = 2 },
                new Day() { DayId = "idDay3", DayDate = new DateTime(2018, 09, 18), DayNumber = 3 }
            };

        protected static DateTime PrefixDate = new DateTime(2019, 6, 18);

        private Member[] InitialMembers =
            new Member[]
            {
                new Member()
                {
                    MemberId = 213,
                    MemberName = "SFiordi",
                    MemberDescription = "Stefano Fiordi",
                    MemberShortCode = "SFI",
                    IsMemberActive = true,
                    ProfileId = 2,
                    CompanyId = 1,
                    MemberNote = null,
                    CanBeOwner = false,
                    Password = "202cb962ac59075b964b07152d234b70",
                    AnalyticCost = 19.7m,
                    ToBeTraced = true,
                    DailyHours = 8,
                    ReconcileValuationTypeId = 11
                },
                new Member()
                {
                    MemberId = 224,
                    MemberName = "TCostanzo",
                    MemberDescription = "Tommaso Costanzo",
                    MemberShortCode = "TCO",
                    IsMemberActive = true,
                    ProfileId = 2,
                    CompanyId = 1,
                    MemberNote = null,
                    CanBeOwner = false,
                    Password = "5f4dcc3b5aa765d61d8327deb882cf99",
                    AnalyticCost = 19.7m,
                    ToBeTraced = true,
                    DailyHours = 8,
                    ReconcileValuationTypeId = 11
                },
                new Member()
                {
                    MemberId = 218,
                    MemberName = "LCasini",
                    MemberDescription = "Leonardo Casini",
                    MemberShortCode = "LCA",
                    IsMemberActive = true,
                    ProfileId = 2,
                    CompanyId = 1,
                    MemberNote = null,
                    CanBeOwner = false,
                    Password = "202cb962ac59075b964b07152d234b70",
                    AnalyticCost = 19.7m,
                    ToBeTraced = true,
                    DailyHours = 8,
                    ReconcileValuationTypeId = 9
                }
            };

        private Job[] InitialJobs =
            new Job[]
            {
                new Job
                {
                    JobId = 1,
                    JobName = "AutoReceiving",
                    ShortDesc = "Auto Receiving suite",
                    LongDesc = "Auto Receiving suite",
                    IsJobActive = false,
                    CustomerId = 2,
                    StateId = 1,
                    DivisionId = 1,
                    CategoryId = 1,
                    OwnerId = 14,
                    ExpectedReleaseDate = PrefixDate,
                    ConfirmedReleaseDate = PrefixDate,
                    JobNote = "",
                    CustomerOrderRef = "",
                    InternalOfferCode = "028GVO2008",
                    IdFather = null,
                    ProductId = 20,
                    BusinessTeamId = 11,
                    InvCompanyId = 1,
                    TipologyId = 1,
                    SubTipologyId = 15,
                    ProgressPerc = null
                },
                new Job
                {
                    JobId = 2,
                    JobName = "DTCO for Guess",
                    ShortDesc = "DTCO for Guess",
                    LongDesc = "Implementation of DTCO for Guess",
                    IsJobActive = false,
                    CustomerId = 3,
                    StateId = 1,
                    DivisionId = 1,
                    CategoryId = 1,
                    OwnerId = 14,
                    ExpectedReleaseDate = PrefixDate,
                    ConfirmedReleaseDate = PrefixDate,
                    JobNote = "",
                    CustomerOrderRef = "",
                    InternalOfferCode = "03FDE2008",
                    IdFather = null,
                    ProductId = 20,
                    BusinessTeamId = 11,
                    InvCompanyId = 1,
                    TipologyId = 1,
                    SubTipologyId = 15,
                    ProgressPerc = null
                },
                new Job
                {
                    JobId = 3,
                    JobName = "DTCO as Product",
                    ShortDesc = "Develop DTCO as Kyklos Group product",
                    LongDesc = "Develop DTCO as Kyklos Group product",
                    IsJobActive = false,
                    CustomerId = 8,
                    StateId = 6,
                    DivisionId = 1,
                    CategoryId = 10,
                    OwnerId = 14,
                    ExpectedReleaseDate = PrefixDate,
                    ConfirmedReleaseDate = PrefixDate,
                    JobNote = "",
                    CustomerOrderRef = "",
                    InternalOfferCode = "",
                    IdFather = null,
                    ProductId = 20,
                    BusinessTeamId = 11,
                    InvCompanyId = 1,
                    TipologyId = 1,
                    SubTipologyId = 15,
                    ProgressPerc = null
                },
                                new Job
                {
                    JobId = 4,
                    JobName = "NausSys for La MDS Yacht Ser",
                    ShortDesc = "NausSys for La MDS Yacht Ser",
                    LongDesc = "NausSys for La MDS Yacht Ser",
                    IsJobActive = false,
                    CustomerId = 20,
                    StateId = 1,
                    DivisionId = 1,
                    CategoryId = 10,
                    OwnerId = 52,
                    ExpectedReleaseDate = PrefixDate,
                    ConfirmedReleaseDate = PrefixDate,
                    JobNote = "",
                    CustomerOrderRef = "",
                    InternalOfferCode = "01FDE07REVDEF",
                    IdFather = null,
                    ProductId = 20,
                    BusinessTeamId = 11,
                    InvCompanyId = 1,
                    TipologyId = 1,
                    SubTipologyId = 15,
                    ProgressPerc = null
                },

            };

        private Reason[] InitialReasons =
            new Reason[]
            {
                new Reason
                {
                    ReasonCode = "DEV",
                    ReasonDesc = "Development",
                    ReasonId = 3,
                    ReconcileValuationTypeId = 11
                },
                new Reason
                {
                    ReasonCode = "OFF",
                    ReasonDesc = "Offer definition",
                    ReasonId = 4,
                    ReconcileValuationTypeId = 9
                },
                new Reason
                {
                    ReasonCode = "PJM",
                    ReasonDesc = "Project Management",
                    ReasonId = 6,
                    ReconcileValuationTypeId = 9
                },
                new Reason
                {
                    ReasonCode = "ARC",
                    ReasonDesc = "Architecture",
                    ReasonId = 7,
                    ReconcileValuationTypeId = 10
                },
                new Reason
                {
                    ReasonCode = "LEA",
                    ReasonDesc = "Learning Free",
                    ReasonId = 8,
                    ReconcileValuationTypeId = 9
                },
            };

        private JobTime[] InitialJobTimes =
            new JobTime[]
            {
                new JobTime
                {
                    JobId = 1,
                    MemberId = 213,
                    DateOfWork = PrefixDate,
                    AmountTimeToInvoice = null,
                    FreeAmountTime = null,
                    TimeNote = null,
                    ReasonId = 3,
                    Hours = 3,
                },
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
                },
                new JobTime
                {
                    JobId = 3,
                    MemberId = 218,
                    DateOfWork = PrefixDate,
                    AmountTimeToInvoice = null,
                    FreeAmountTime = null,
                    TimeNote = null,
                    ReasonId = 6,
                    Hours = 3,
                },
                new JobTime
                {
                    JobId = 4,
                    MemberId = 218,
                    DateOfWork = PrefixDate,
                    AmountTimeToInvoice = null,
                    FreeAmountTime = null,
                    TimeNote = null,
                    ReasonId = 7,
                    Hours = 3,
                },
                new JobTime
                {
                    JobId = 1,
                    MemberId = 224,
                    DateOfWork = PrefixDate,
                    AmountTimeToInvoice = null,
                    FreeAmountTime = null,
                    TimeNote = null,
                    ReasonId = 8,
                    Hours = 3,
                },
                new JobTime
                {
                    JobId = 4,
                    MemberId = 224,
                    DateOfWork = PrefixDate,
                    AmountTimeToInvoice = null,
                    FreeAmountTime = null,
                    TimeNote = null,
                    ReasonId = 3,
                    Hours = 3,
                },
            };

        #endregion

        protected virtual async Task SetupCoreAsync()
        {
            await PrepareDB();
            await GenerateScriptsForCreateAndDropSequence().ConfigureAwait(false);
            await AddTeams();
            await AddDays();
            await AddResults();
            await AddMembers().ConfigureAwait(false);
            await AddJobs().ConfigureAwait(false);
            await AddReasons().ConfigureAwait(false);
            await AddJobTimes().ConfigureAwait(false);
        }

        protected abstract Task GenerateScriptsForCreateAndDropSequence();

        protected virtual IAsyncDao CreateAsyncDao(string connectionString, string providerName, string schema, bool ignoreEscape = false)
        {
            return
                AsyncDaoFactory
                .CreateAsyncDao
                (
                    connectionString: connectionString,
                    providerName: providerName,
                    schema: schema,
                    ignoreEscape: ignoreEscape
                );
        }

        protected virtual async Task AddTeams()
        {
            await Dao.WriteToServerAsync(InitialTeams).ConfigureAwait(false);
        }

        protected virtual async Task AddDays()
        {
            await Dao.WriteToServerAsync(InitialDays).ConfigureAwait(false);
        }

        protected virtual async Task AddResults()
        {
            await Dao.WriteToServerAsync(InitialResults).ConfigureAwait(false);
        }
        protected virtual async Task AddMembers()
        {
            await Dao.WriteToServerAsync(InitialMembers).ConfigureAwait(false);
        }
        protected virtual async Task AddReasons()
        {
            await Dao.WriteToServerAsync(InitialReasons).ConfigureAwait(false);
        }
        protected virtual async Task AddJobs()
        {
            await Dao.WriteToServerAsync(InitialJobs).ConfigureAwait(false);
        }
        protected virtual async Task AddJobTimes()
        {
            await Dao.WriteToServerAsync(InitialJobTimes).ConfigureAwait(false);
        }
        protected async Task CountAllTeamsShouldBe4Core()
        {
            int n = 4;
            var queryBuilder =
                Dao
                    .NewQueryBuilder()
                        .Select()
                            .Count<Team>("t", x => x.TeamId)
                        .From()
                            .Table<Team>("t");

            var actualValue = (await Dao.GetItemsAsync<int>(queryBuilder).ConfigureAwait(false)).FirstOrDefault();
            Assert.Equal(n, actualValue);
        }

        protected async Task CountAllDaysShouldBe3Core()
        {
            int n = 3;
            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Count<Day>("d", x => x.DayId)
                .From()
                .Table<Day>("d");

            var actualValue = (await Dao.GetItemsAsync<int>(queryBuilder).ConfigureAwait(false)).FirstOrDefault();
            Assert.Equal(n, actualValue);
        }

        public Task CountAllResultsShouldBe6Core()
        {
            return CountAllResultsShouldBeN(6, Dao);
        }

        protected async Task CountAllResultsShouldBeN(int n, IAsyncDao tDao)
        {
            var queryBuilder =
                tDao
                .NewQueryBuilder()
                .Select()
                .Count<Result>("r", x => x.ResultId)
                .From()
                .Table<Result>("r");

            var actualValue = await tDao.ExecuteScalarAsync<int>(queryBuilder).ConfigureAwait(false);
            Assert.Equal(n, actualValue);
        }

        protected async Task DeleteFourResults(IAsyncDao tDao)
        {
            await tDao.DeleteEntityAsync(new Result() { ResultId = "idRes1", HomeTeamId = "idFio", VisitorTeamId = "idJuv", GoalsHomeTeam = 1, GoalsVisitorTeam = 3, DayId = "idDay1" }).ConfigureAwait(false);
            await tDao.DeleteEntityAsync(new Result() { ResultId = "idRes3", HomeTeamId = "idFio", VisitorTeamId = "idMil", GoalsHomeTeam = 4, GoalsVisitorTeam = 3, DayId = "idDay2" }).ConfigureAwait(false);
            await tDao.DeleteEntityAsync(new Result() { ResultId = "idRes5", HomeTeamId = "idFio", VisitorTeamId = "idInt", GoalsHomeTeam = 0, GoalsVisitorTeam = 2, DayId = "idDay3" }).ConfigureAwait(false);
            await tDao.DeleteEntityAsync(new Result() { ResultId = "idRes6", HomeTeamId = "idJuv", VisitorTeamId = "idMil", GoalsHomeTeam = 4, GoalsVisitorTeam = 1, DayId = "idDay3" }).ConfigureAwait(false);
        }

        protected async Task SumOfGoalsHomeTeamShouldBeN(int expectedSum)
        {
            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Sum<Result>("r", x => x.GoalsHomeTeam)
                .From()
                .Table<Result>("r");

            var actualValue = await Dao.ExecuteScalarAsync<int>(queryBuilder).ConfigureAwait(false);
            Assert.Equal(expectedSum, actualValue);
        }

        protected async Task SumOfGoalsVisitorTeamShouldBeN(int expectedSum)
        {
            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Sum<Result>("r", x => x.GoalsVisitorTeam)
                .From()
                .Table<Result>("r");

            var actualValue = await Dao.ExecuteScalarAsync<int>(queryBuilder).ConfigureAwait(false);
            Assert.Equal(expectedSum, actualValue);
        }

        public void GetTableNamesForSchemaShouldBeRESULTS()
        {
            var expectedValue =
                new string[]
                {
                    "RESULTS"
                };

            var actualValue =
                Dao
                .GetTableNamesForSchema(Schema, x => string.Equals(x, "RESULTS", StringComparison.InvariantCultureIgnoreCase))
                .ToArray();

            Assert.Equal(expectedValue.ToArray(), actualValue.ToArray(), StringComparer.InvariantCultureIgnoreCase);
        }

        protected async Task SelectTheFirstDayWithTupleExceptionShouldBeTwoResults()
        {
            Tuple<int, DateTime> expectedDay = new Tuple<int, DateTime>(1, new DateTime(2018, 09, 04));

            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Fields<Day>("d", x => x.DayId, x => x.DayDate)
                .From()
                .Table<Day>("d")
                .WithPagination(null, 1, true);

            var actualDay = (await Dao.GetItemsAsync<Tuple<int, DateTime>>(queryBuilder).ConfigureAwait(false)).FirstOrDefault();
            Assert.Equal(expectedDay, actualDay);
        }

        protected async Task SelectResultsByIdWithDictionaryExceptionShouldBeIdRes4()
        {
            Result result = null;

            IDictionary<string, object> expectedValue =
                new Dictionary<string, object>
                {
                    { result.GetFieldName<Result>(x => x.ResultId), "idRes4" },
                    { result.GetFieldName<Result>(x => x.HomeTeamId), "idJuv" },
                    { result.GetFieldName<Result>(x => x.VisitorTeamId), "idMil" },
                    { result.GetFieldName<Result>(x => x.GoalsHomeTeam), 2 },
                    { result.GetFieldName<Result>(x => x.GoalsVisitorTeam), 2 },
                    { result.GetFieldName<Result>(x => x.DayId), "idDay2" }
                };

            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Star("r")
                .From()
                .Table<Result>("r")
                .Where<Result>("r", x => x.ResultId == "idRes4");

            var actualValue = (await Dao.GetItemsAsync<IDictionary<int, char>>(queryBuilder).ConfigureAwait(false)).FirstOrDefault();
            Assert.Null(actualValue);
        }

        protected void CheckIfDbSupportsValuesForFastInConditionShouldBeCore(bool expectedBool)
        {
            bool actualBool = Dao.DbProviderHelper.SupportsSqlValues;
            Assert.Equal(expectedBool, actualBool);
        }

        protected void CheckDbProviderNameShouldBeCore(string expectedName)
        {
            string actualName = Dao.DbProviderHelper.DbProviderName;
            Assert.Equal(expectedName, actualName);
        }

        protected void SqlInnerJoinShouldBeCore(string sqlExpectedJoin, bool ignoreEscape = true)
        {
            var queryBuilder =
                Dao
                .NewQueryBuilder(ignoreEscape: ignoreEscape)
                .Select()
                .Star()
                .From()
                .Tables
                (
                    FlatTable<Result>.WithAlias("r"),
                    InnerJoin<Day>.WithAlias("d"), (r, d) => r.DayId == d.DayId,
                    InnerJoin<Team>.WithAlias("ht"), (r, d, ht) => r.HomeTeamId == ht.TeamId,
                    InnerJoin<Team>.WithAlias("vt"), (r, d, ht, vt) => r.VisitorTeamId == vt.TeamId
                );

            var sqlActualJoin = queryBuilder.BuildSqlTextWithParameters().SqlText;
            Assert.Contains(sqlExpectedJoin, sqlActualJoin);
        }

        protected void SqlLeftJoinShouldBeCore(string sqlExpectedJoin, bool ignoreEscape = true)
        {
            var queryBuilder =
                Dao
                .NewQueryBuilder(ignoreEscape: ignoreEscape)
                .Select()
                .Star()
                .From()
                .Tables
                (
                    FlatTable<Result>.WithAlias("r"),
                    LeftJoin<Day>.WithAlias("d"), (r, d) => r.DayId == d.DayId,
                    LeftJoin<Team>.WithAlias("ht"), (r, d, ht) => r.HomeTeamId == ht.TeamId,
                    LeftJoin<Team>.WithAlias("vt"), (r, d, ht, vt) => r.VisitorTeamId == vt.TeamId
                );

            var sqlActualJoin = queryBuilder.BuildSqlTextWithParameters().SqlText;
            Assert.Contains(sqlExpectedJoin, sqlActualJoin);
        }

        protected void SqlRightJoinShouldBeCore(string sqlExpectedJoin, bool ignoreEscape = true)
        {
            var queryBuilder =
                Dao
                .NewQueryBuilder(ignoreEscape: ignoreEscape)
                .Select()
                .Star()
                .From()
                .Tables
                (
                    FlatTable<Result>.WithAlias("r"),
                    RightJoin<Day>.WithAlias("d"), (r, d) => r.DayId == d.DayId,
                    RightJoin<Team>.WithAlias("ht"), (r, d, ht) => r.HomeTeamId == ht.TeamId,
                    RightJoin<Team>.WithAlias("vt"), (r, d, ht, vt) => r.VisitorTeamId == vt.TeamId
                );

            var sqlActualJoin = queryBuilder.BuildSqlTextWithParameters().SqlText;
            Assert.Contains(sqlExpectedJoin, sqlActualJoin);
        }

        protected void SqlFullOuterJoinShouldBeCore(string sqlExpectedJoin, bool ignoreEscape = true)
        {
            var queryBuilder =
                Dao
                .NewQueryBuilder(ignoreEscape: ignoreEscape)
                .Select()
                .Star()
                .From()
                .Tables
                (
                    FlatTable<Result>.WithAlias("r"),
                    FullOuterJoin<Day>.WithAlias("d"), (r, d) => r.DayId == d.DayId,
                    FullOuterJoin<Team>.WithAlias("ht"), (r, d, ht) => r.HomeTeamId == ht.TeamId,
                    FullOuterJoin<Team>.WithAlias("vt"), (r, d, ht, vt) => r.VisitorTeamId == vt.TeamId
                );

            var sqlActualJoin = queryBuilder.BuildSqlTextWithParameters();
            Assert.Contains(sqlExpectedJoin, sqlActualJoin.SqlText);
        }

        protected async Task GetNextValueForSequenceShouldBeCore(long expectedValue, string sequenceName)
        {
            long actualValue = await Dao.GetNextValueForSequenceAsync(sequenceName).ConfigureAwait(false);
            Assert.Equal(expectedValue, actualValue);
        }

        protected async Task GetRangeValuesForSequenceShouldBeCore(long[] expectedRange, string sequenceName)
        {
            long[] actualRange = await
                Dao
                .GetRangeValuesForSequenceAsync(sequenceName, expectedRange.Length)
                .ConfigureAwait(false);
            Assert.Equal(expectedRange, actualRange);
        }

        protected async Task GetResultTableMetadataFromQueryShouldBeCore(string sql)
        {
            var actualMetadata = await Dao.GetMetadataForQueryAsync(sql);

            var metadataRows = actualMetadata.Select("ColumnName = 'GOALS_HOME_TEAM' OR ColumnName = 'GOALS_VISITOR_TEAM'");

            Assert.Equal(2, metadataRows.Count());
        }

        protected async Task FillDayDataTableShouldBeCore(string sql)
        {
            DataTable dayTable = new DataTable();

            await Dao.FillDataTableAsync(dayTable, sql).ConfigureAwait(false);

            var dataRows = dayTable.Select("DAY_ID = 'idDay1' OR DAY_ID = 'idDay2' OR DAY_ID = 'idDay3'");

            Assert.Equal(3, dataRows.Count());
        }

        private async Task ReplaceDuplicateKey(IAsyncDao tDao, Day newDay, string newKey)
        {
            newDay.DayId = newKey;
            await tDao.InsertEntityAsync(newDay).ConfigureAwait(false);
        }

        protected void GetScriptForCreateTableShouldBeCore(string expectedScript, bool generateForeignKeyConstraints, Type entityType)
        {
            string actualScript = Dao.GenerateSqlScriptForCreateTable(entityType, generateForeignKeyConstraints).ScriptText;
            Assert.Equal(expectedScript, actualScript);
        }

        protected void GetScriptForDropTableShouldBeCore(string expectedScript, string tableName)
        {
            string actualScript = Dao.GenerateSqlScriptForDropTable(tableName).ScriptText;
            Assert.Equal(expectedScript, actualScript);
        }

        protected void GetScriptsForForeignKeyShouldBeCore(string[] expectedScripts, Type entityType)
        {
            var actualScripts =
                Dao
                .GenerateSqlScriptsForForeignKeyConstraints(entityType)
                    .Select(x => x.ScriptText)
                        .ToArray();

            Assert.Equal(expectedScripts, actualScripts);
        }

        protected void GetScriptForUniqueConstraintShouldBeCore(string[] expectedScripts, Type entityType)
        {
            var actualScripts =
                Dao
                .GenerateSqlScriptsForCreateUniqueConstraints(entityType)
                .Select(x => x.ScriptText)
                .ToArray();

            Assert.Equal(expectedScripts, actualScripts);
        }

        protected void GetScriptForCreateNonUniqueIndexShouldBeCore(string[] expectedScripts, Type entityType)
        {
            var actualScripts =
                Dao
                .GenerateSqlScriptsForCreateIndexes(entityType)
                .Select(x => x.ScriptText)
                .ToArray();

            Assert.Equal(expectedScripts, actualScripts);
        }

        protected async Task CreateAndDropFKTableShouldBeCore(string createTable, string dropTable)
        {
            await ExecuteScript(createTable).ConfigureAwait(false);
            await ExecuteScript(dropTable).ConfigureAwait(false);
        }

        protected async Task CancellationOfGetCompleteResultShouldBeCore(int n, CancellationToken cancellationToken)
        {
            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                    .Field<Day>("d", x => x.DayNumber)
                    .Comma()
                    .Field<Team>("ht", x => x.Name)
                    .Comma()
                    .Field<Result>("r", x => x.GoalsHomeTeam)
                    .Comma()
                    .Case()
                        .When<Result>("r", x => x.GoalsHomeTeam > x.GoalsVisitorTeam)
                        .Then("'v'")
                        .When<Result>("r", x => x.GoalsHomeTeam == x.GoalsVisitorTeam)
                        .Then("'d'")
                        .Else("'l'")
                    .EndCase()
                    .Comma()
                    .Field<Team>("vt", x => x.Name)
                    .Comma()
                    .Field<Result>("r", x => x.GoalsVisitorTeam)
                    .Comma()
                    .Case()
                        .When<Result>("r", x => x.GoalsHomeTeam < x.GoalsVisitorTeam)
                        .Then("'v'")
                        .When<Result>("r", x => x.GoalsHomeTeam == x.GoalsVisitorTeam)
                        .Then("'d'")
                        .Else("'l'")
                    .EndCase()
                .From()
                    .Tables
                    (
                        FlatTable<Result>.WithAlias("r"),
                        InnerJoin<Day>.WithAlias("d"), (r, d) => r.DayId == d.DayId,
                        InnerJoin<Team>.WithAlias("ht"), (r, d, ht) => r.HomeTeamId == ht.TeamId,
                        InnerJoin<Team>.WithAlias("vt"), (r, d, ht, vt) => r.VisitorTeamId == vt.TeamId
                    )
                .OrderBy(1)
                .ThenBy<Team>("ht", x => x.Name);

            Stopwatch timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < n; i++)
            {
                var actualTuples = await
                    Dao
                    .GetItemsArrayAsync<Tuple<int, string, int, char, string, int, char>>(queryBuilder: queryBuilder, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            timer.Stop();
            double avgTime = timer.Elapsed.Milliseconds / n;
        }

        protected async Task SelectDayDatesAsStringsShouldBeCore(string dateFormat)
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

            var actualDatesString = (await Dao.GetItemsAsync<string>(queryBuilder).ConfigureAwait(false)).ToArray();
            Assert.Equal(expectedDatesStrings, actualDatesString);
        }

        protected void GetForeignKeysMetadataForTableShouldBeCore(FKConstraintDbItem[] expectedFK, string tableName, string schema = null)
        {
            var actualFK =
                Dao
                .GetForeignKeysMetadataForTableAsync(tableName, schema)
                .Result
                .OrderBy(x => x.ConstraintName)
                .ToArray();

            Assert.Equal(expectedFK, actualFK, new FKConstraintDbItemComparer());
        }

        protected void GetDbIndexesMetadataForTableShouldBeCore(IndexConstraintDbItem[] expectedIndexes, string tableName, string schema = null)
        {
            var actualIndexes =
                Dao
                .GetDbIndexesMetadataForTableAsync(tableName, schema)
                .Result
                .OrderBy(x => x.ConstraintName)
                .ThenBy(x => x.ReferencedField)
                .ToArray();

            Assert.Equal(expectedIndexes, actualIndexes, new IndexConstraintDbItemComparer());
        }

        protected void GetIndexesConstraintsForTableShouldBeCore(IndexConstraint[] expectedIndexes, string tableName, string schema = null)
        {
            var actualIndexes =
                Dao
                .GetIndexConstraintsForTableAsync(tableName, schema)
                .Result
                .OrderBy(x => x.ConstraintName)
                .ToArray();

            Assert.Equal(expectedIndexes.Length, actualIndexes.Length);
            for (int i = 0; i < expectedIndexes.Length; i++)
            {
                Assert.Equal(
                        expectedIndexes[i].ReferencedFields.ToArray().Length,
                        actualIndexes[i].ReferencedFields.ToArray().Length
                    );
            }
        }

        protected bool ContainsEscapeShouldBeCore(char escape, IAsyncDao myDao, bool ignoreEscape)
        {
            var queryBuilder =
                myDao.
                NewQueryBuilder(ignoreEscape: ignoreEscape)
                .Select()
                    .Star("r")
                .From()
                    .Table<Result>("r");

            var query = queryBuilder.BuildSqlTextWithParameters().SqlText;
            return query.Contains(escape);
        }

        protected async Task SelectResultsForADayWithInvertedTuplesShouldBeCore(string dayId)
        {
            object expectedResults =
                new Tuple<Result, Day>[]
                {
                    new Tuple<Result, Day>(new Result() { ResultId = "idRes1", HomeTeamId = "idFio", VisitorTeamId = "idJuv", GoalsHomeTeam = 1, GoalsVisitorTeam = 3, DayId = "idDay1" }, new Day() { DayId = "idDay1", DayDate = new DateTime(2018, 09, 16), DayNumber = 1 }),
                    new Tuple<Result, Day>(new Result() { ResultId = "idRes2", HomeTeamId = "idMil", VisitorTeamId = "idInt", GoalsHomeTeam = 0, GoalsVisitorTeam = 1, DayId = "idDay1" }, new Day() { DayId = "idDay1", DayDate = new DateTime(2018, 09, 16), DayNumber = 1 })
                }
                .OrderBy(x => x.Item1.ResultId)
                .ToArray();

            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Star("r")
                .Comma()
                .Star("d")
                .From()
                .Tables
                (
                    FlatTable<Result>.WithAlias("r"),
                    InnerJoin<Day>.WithAlias("d"), (r, d) => r.DayId == d.DayId
                )
                .Where<Result>("r", x => x.DayId == dayId);

            object actualResults = (await
                Dao
                .GetItemsAsync<Tuple<Day, Result>>(queryBuilder)
                .ConfigureAwait(false))
                .OrderBy(x => x.Item2.ResultId)
                .ToArray();

            Assert.Equal(expectedResults, actualResults);
        }

        protected async Task ExecuteScript(string sqlScript)
        {
            await Dao.ExecuteNonQueryAsync(sqlScript).ConfigureAwait(false);
        }

        protected async Task PrepareDB()
        {
            try
            {
                await DropTables().ConfigureAwait(false);
                await CreateTables().ConfigureAwait(false);
            }
            catch (Exception) { }
        }

        private async Task DropTables()
        {
            await Dao.DropTableAsync<Tbl4FastInStringFilter>(ignoreError: true).ConfigureAwait(false);
            await Dao.DropTableAsync<Tbl4FastInFilter>(ignoreError: true).ConfigureAwait(false);
            await Dao.DropTableAsync<Result>(ignoreError: true).ConfigureAwait(false);
            await Dao.DropTableAsync<Team>(ignoreError: true).ConfigureAwait(false);
            await Dao.DropTableAsync<Day>(ignoreError: true).ConfigureAwait(false);
            await Dao.DropTableAsync<Member>(ignoreError: true).ConfigureAwait(false);
            await Dao.DropTableAsync<Job>(ignoreError: true).ConfigureAwait(false);
            await Dao.DropTableAsync<Reason>(ignoreError: true).ConfigureAwait(false);
            await Dao.DropTableAsync<JobTime>(ignoreError: true).ConfigureAwait(false);
        }

        private async Task CreateTables()
        {
            var ddlResults = await
                Dao
                .ExecDDLScriptsForTablesAsync
                (
                    new Type[]
                    {
                        typeof(Day),
                        typeof(Team),
                        typeof(Result),
                        typeof(Member),
                        typeof(Job),
                        typeof(Reason),
                        typeof(JobTime),
                        typeof(Tbl4FastInFilter),
                        typeof(Tbl4FastInStringFilter)
                    }
                )
                .ConfigureAwait(false);
        }

        protected async Task PrepareSequence(string createSequenceScript, string dropSequenceScript)
        {
            try
            {
                await Dao.ExecuteNonQueryAsync(dropSequenceScript).ConfigureAwait(false);
                await Dao.ExecuteNonQueryAsync(createSequenceScript).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await Dao.ExecuteNonQueryAsync(createSequenceScript).ConfigureAwait(false);
            }
        }


        protected async Task SelectResultsWhereResultIdIsIdRes4ShouldBeCore()
        {
            var expectedValue =
                new Result[]
                {
                    new Result() { ResultId = "idRes1", HomeTeamId = "idFio", VisitorTeamId = "idJuv", GoalsHomeTeam = 1, GoalsVisitorTeam = 3, DayId = "idDay1" },
                    new Result() { ResultId = "idRes5", HomeTeamId = "idFio", VisitorTeamId = "idInt", GoalsHomeTeam = 0, GoalsVisitorTeam = 2, DayId = "idDay3" },
                }
                .OrderBy(x => x.ResultId)
                .ToArray();

            var queryBuilder =
                Dao
                .NewQueryBuilder()
                    .Select()
                        .Star()
                    .From()
                        .Table<Result>("R")
                    .Where<Result>("R", x => x.HomeTeamId == "idFio");

            var actualValue =
                (await Dao.GetItemsAsync<Result>(queryBuilder).ConfigureAwait(false))
                .OrderBy(x => x.ResultId)
                .ToArray();

            Assert.Equal(expectedValue, actualValue);
        }


        protected async Task SelectTeamsWhereGoalsVisitorTeamIsGreaterThan2ShouldBeJuveAndFioreCore()
        {
            var expectedValue =
                new string[]
                {
                    "Juventus",
                    "Fiorentina"
                }
                .OrderBy(x => x)
                .ToArray();

            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Distinct()
                .Field<Team>("T", x => x.Name)
                .From()
                .Table<Result>("R")
                .TablesJoin<Result, Team>
                (
                    "R",
                    InnerJoin<Team>.WithAlias("T"),
                    (R, T) => R.VisitorTeamId == T.TeamId
                )
                .Where<Result>("R", x => x.GoalsVisitorTeam > 2);

            var actualValue = (await Dao.GetItemsAsync<string>(queryBuilder).ConfigureAwait(false)).OrderBy(x => x).ToArray();
            Assert.Equal(expectedValue, actualValue);
        }


        protected async Task SelectHomeTeamsGoalsShouldBe4TeamsCore()
        {
            Tuple<string, int>[] expectedValue = new Tuple<string, int>[]
            {
                new Tuple<string, int>("Inter", 4),
                new Tuple<string, int>("Milan", 4),
                new Tuple<string, int>("Juventus", 2),
                new Tuple<string, int>("Fiorentina", 1)
            }.OrderBy(x => x.Item1).ToArray();

            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Field<Team>("T", x => x.Name)
                .Comma()
                .Sum<Result>("R", x => x.GoalsHomeTeam)
                .From()
                .Table<Result>("R")
                .TablesJoin<Result, Team>("R", InnerJoin<Team>.WithAlias("T"), (R, T) => R.HomeTeamId == T.TeamId)
                .GroupBy()
                .Fields<Team>("T", x => x.TeamId, x => x.Name)
                .OrderBy<Team>("T", x => x.Name, OrderByDirection.Descending);

            var actualValue = (await Dao.GetItemsAsync<Tuple<string, int>>(queryBuilder).ConfigureAwait(false)).OrderBy(x => x.Item1).ToArray();
            Assert.Equal(expectedValue, actualValue);
        }


        protected async Task SelectTotalTeamsGoalsShouldBe4TeamsCore()
        {
            Tuple<string, int>[] expectedValue =
                new Tuple<string, int>[]
                {
                    new Tuple<string, int>("Inter", 11),
                    new Tuple<string, int>("Milan", 8),
                    new Tuple<string, int>("Juventus", 8),
                    new Tuple<string, int>("Fiorentina", 7)
                }
                .OrderBy(x => x.Item1)
                .ToArray();

            var queryBuilder =
                    Dao
                        .NewQueryBuilder()
                            .Select()
                                .Field<Team>("T", x => x.Name)
                                .Comma()
                                .Sum<Result>("RH", x => x.GoalsHomeTeam)
                                .OperPlus()
                                .Sum<Result>("RV", x => x.GoalsVisitorTeam)
                            .From()
                                .Tables
                                (
                                    FlatTable<Team>.WithAlias("T"),
                                    LeftJoin<Result>.WithAlias("RH"), (T, RH) => T.TeamId == RH.HomeTeamId,
                                    LeftJoin<Result>.WithAlias("RV"), (T, RH, RV) => T.TeamId == RV.VisitorTeamId
                                )
                            .GroupBy()
                                .Fields<Team>("T", x => x.TeamId, x => x.Name)
                            .OrderBy<Team>("T", x => x.Name, OrderByDirection.Descending);

            var actualValue = (await Dao.GetItemsAsync<Tuple<string, int>>(queryBuilder).ConfigureAwait(false)).OrderBy(x => x.Item1).ToArray();
            Assert.Equal(expectedValue, actualValue);
        }


        protected async Task SelectHomeTeamsWhereConditionsAreMetShouldBe3TeamsCore()
        {
            Tuple<string, int>[] expectedValue = new
                Tuple<string, int>[]
                {
                    new Tuple<string, int>("Milan", 4),
                    new Tuple<string, int>("Fiorentina", 0),
                    new Tuple<string, int>("Milan", 0)
                }
                .OrderBy(x => x.Item2)
                .ThenByDescending(x => x.Item1)
                .ToArray();

            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Field<Team>("T", x => x.Name)
                .Comma()
                .Field<Result>("R", x => x.GoalsHomeTeam)
                .From()
                .Table<Result>("R")
                .TablesJoin<Result, Team>
                (
                    "R",
                    InnerJoin<Team>.WithAlias("T"),
                    (R, T) => R.HomeTeamId == T.TeamId
                )
                .Where()
                .OpenPar()
                    .Condition<Result>("R", x => x.GoalsHomeTeam > 2)
                    .And<Result>("R", x => x.GoalsVisitorTeam, WhereOperator.LessThan, 2)
                .ClosePar()
                .Or<Result>("R", x => x.GoalsHomeTeam, WhereOperator.EqualTo, 0)
                .OrderBy<Result>("R", x => x.GoalsHomeTeam, OrderByDirection.Ascending)
                .ThenBy<Team>("T", x => x.Name, OrderByDirection.Descending);

            var xx = queryBuilder.BuildSqlTextWithParameters();

            var actualValue = (await
                Dao
                .GetItemsAsync<Tuple<string, int>>(queryBuilder)
                .ConfigureAwait(false))
                .ToArray();

            Assert.Equal(expectedValue, actualValue);
        }

        protected async Task SelectTeamsNameLikeShouldBe3TeamsCore()
        {
            var expectedValues =
                new string[]
                {
                    "Fiorentina",
                    "Juventus",
                    "Inter"
                }
                .OrderBy(x => x)
                .ToArray();

            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Field<Team>("T", x => x.Name)
                .From()
                .Table<Team>("T")
                .Where<Team>("T", x => x.Name.Contains("nt"))
                .OrderBy("1");

            var actualValues = await
                Dao
                .GetItemsArrayAsync<string>(queryBuilder)
                .ConfigureAwait(false);

            Assert.Equal(expectedValues, actualValues);
        }


        protected async Task CountAllResultsAfterFourAreDeletedShouldBe1Core()
        {
            await Dao
            .DoInTransactionAsync
            (
                async tDao =>
                {
                    await DeleteFourResults(tDao);
                    await CountAllResultsShouldBeN(2, tDao);
                    throw new Exception();
                }
            )
            .ConfigureAwait(false);

        }

        protected async Task UpdateGoalsHomeTeamShouldBe3Core()
        {
            int n = 3;
            await Dao
            .DoInTransactionAsync
            (
                async tDao =>
                {
                    var updateTableBuilder =
                        tDao
                        .NewUpdateTableBuilder<Result>()
                        //.Set(x => x.GoalsVisitorTeam, x => x.GoalsVisitorTeam + 1)
                        .Set(x => x.GoalsHomeTeam, () => n)
                        .Where(x => x.ResultId == "idRes2");

                    var sql = updateTableBuilder.BuildSqlTextWithParameters();
                    int affected = await tDao.UpdateTableAsync(updateTableBuilder);

                    var actualValue = await
                    tDao.GetItemByExampleAsync<Result>(x => x.ResultId == "idRes2").ConfigureAwait(false);

                    Assert.Equal(n, actualValue.GoalsHomeTeam);
                    throw new Exception();
                },
                exc =>
                {
                    return Task.CompletedTask;
                }
            ).ConfigureAwait(false);


        }


        protected async Task UpsertTeamsShouldBe5TeamsCore()
        {
            Team[] expectedValues =
                new Team[]
                {
                    new Team { TeamId = "idFio", Name = "Fiorentina", City = "Florence", President = "Della Valle" },
                    new Team { TeamId = "idRom", Name = "Roma", City = "Rome", President = "Pallotta" },
                    new Team { TeamId = "idMil", Name = "Milan", City = "Milan", President = "Scaroni" },
                    new Team { TeamId = "idJuv", Name = "Juventus", City = "Turin", President = "Agnelli" },
                    new Team { TeamId = "idInt", Name = "Inter", City = "Milan", President = "Thoir" }
                }
                .OrderBy(x => x.TeamId)
                .ToArray();
            await Dao.DoInTransactionAsync
            (
                async tDao =>
                {
                    await Dao.UpsertEntitiesAsync(expectedValues).ConfigureAwait(false);
                    var actualValues = (await Dao.GetAllItemsArrayAsync<Team>().ConfigureAwait(false)).OrderBy(x => x.TeamId).ToArray();
                    Assert.Equal(expectedValues, actualValues);
                    throw new Exception();
                }
            ).ConfigureAwait(false);




        }


        protected async Task SelectWithFastInConditionShouldGet2TeamsCore()
        {
            var expectedValue =
                new string[]
                {
                    "Fiorentina",
                    "Inter"
                }
                .OrderBy(x => x)
                .ToArray();

            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Field<Team>("T", x => x.Name)
                .From()
                .Tables
                (
                    FlatTable<Team>.WithAlias("T"),
                    InnerJoin<Result>.WithAlias("R"), (T, R) => T.TeamId == R.HomeTeamId
                )
                .Where<Team>("T", x => x.Name, WhereOperator.Contains, "nt")
                .AndFastInCondition<Result>("R", x => x.ResultId, new string[] { "idRes1", "idRes3" }, 1)
                .OrderBy(1);

            var xx = queryBuilder.BuildSqlTextWithParameters();
            var actualValue = (await
                Dao
                .GetItemsAsync<string>(queryBuilder)
                .ConfigureAwait(false))
                .OrderBy(x => x)
                .ToArray();

            Assert.Equal(expectedValue, actualValue);
        }

        protected async Task SumOfGoalsHomeTeamShouldBe11Core()
        {
            await SumOfGoalsHomeTeamShouldBeN(11).ConfigureAwait(false);
        }

        protected async Task SumOfGoalsVisitorTeamShouldBe12Core()
        {
            await SumOfGoalsVisitorTeamShouldBeN(12).ConfigureAwait(false);
        }

        protected async Task SumOfTotalGoalsForResultShouldBeOKCore()
        {
            Tuple<string, int>[] expectedTuples =
                new Tuple<string, int>[]
                {
                    new Tuple<string, int> ("idRes1", 4),
                    new Tuple<string, int> ("idRes2", 1),
                    new Tuple<string, int> ("idRes3", 7),
                    new Tuple<string, int> ("idRes4", 4),
                    new Tuple<string, int> ("idRes5", 2),
                    new Tuple<string, int> ("idRes6", 5)
                }
                .OrderBy(x => x.Item1)
                .ToArray();

            var queryBuilder =
                Dao.
                NewQueryBuilder()
                .Select()
                .Field<Result>("r", x => x.ResultId)
                .Comma()
                .Sum<Result>("r", x => x.GoalsHomeTeam + x.GoalsVisitorTeam)
                .From()
                .Table<Result>("r")
                .GroupBy()
                .Field<Result>("r", x => x.ResultId);

            var actualTuples = (await
                Dao
                .GetItemsAsync<Tuple<string, int>>(queryBuilder)
                .ConfigureAwait(false))
                .OrderBy(x => x.Item1)
                .ToArray();

            Assert.Equal(expectedTuples, actualTuples);
        }

        protected async Task SumOfTotalGoalsShouldBe23Core()
        {
            int expectedValue = 23;
            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Sum<Result>("r", x => x.GoalsHomeTeam)
                .OperPlus()
                .Sum<Result>("r", x => x.GoalsVisitorTeam)
                .From()
                .Table<Result>("r");

            var actualValue = await
                Dao
                .ExecuteScalarAsync<int>(queryBuilder)
                .ConfigureAwait(false);

            Assert.Equal(expectedValue, actualValue);
        }

        protected async Task MaxOfGoalsHomeTeamShouldBe4Core()
        {
            int expectedValue = 4;
            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Max<Result>("r", x => x.GoalsHomeTeam)
                .From()
                .Table<Result>("r");

            var actualValue = await Dao.ExecuteScalarAsync<int>(queryBuilder).ConfigureAwait(false);
            Assert.Equal(expectedValue, actualValue);
        }

        protected async Task MaxOfGoalsVisitorTeamShouldBe3Core()
        {
            int expectedValue = 3;
            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Max<Result>("r", x => x.GoalsVisitorTeam)
                .From()
                .Table<Result>("r");

            var actualValue = await Dao.ExecuteScalarAsync<int>(queryBuilder).ConfigureAwait(false);
            Assert.Equal(expectedValue, actualValue);
        }

        protected async Task MaxOfTotalGoalsShouldBe7Core()
        {
            int expectedValue =
                InitialResults
                .Select(x => x.GoalsHomeTeam + x.GoalsVisitorTeam)
                .Max();

            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Max<Result>("r", x => x.GoalsHomeTeam + x.GoalsVisitorTeam)
                .From()
                .Table<Result>("r");

            var actualValue = await Dao.ExecuteScalarAsync<int>(queryBuilder).ConfigureAwait(false);
            Assert.Equal(expectedValue, actualValue);
        }

        protected async Task MinOfGoalsHomeTeamShouldBeZeroCore()
        {
            int expectedValue = 0;
            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Min<Result>("r", x => x.GoalsHomeTeam)
                .From()
                .Table<Result>("r");

            var actualValue = await Dao.ExecuteScalarAsync<int>(queryBuilder).ConfigureAwait(false);
            Assert.Equal(expectedValue, actualValue);
        }

        protected async Task MinOfGoalsVisitorTeamShouldBe1Core()
        {
            int expectedValue = 1;
            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Min<Result>("r", x => x.GoalsVisitorTeam)
                .From()
                .Table<Result>("r");

            var actualValue = await Dao.ExecuteScalarAsync<int>(queryBuilder).ConfigureAwait(false);
            Assert.Equal(expectedValue, actualValue);
        }

        protected async Task MinOfTotalGoalsShouldBe1Core()
        {
            int expectedValue = 1;
            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Min
                (
                    nqb =>
                        nqb
                        .Field<Result>("r", x => x.GoalsHomeTeam)
                        .OperPlus()
                        .Field<Result>("r", x => x.GoalsVisitorTeam)
                )
                .From()
                .Table<Result>("r");

            var actualValue = await Dao.ExecuteScalarAsync<int>(queryBuilder).ConfigureAwait(false);
            Assert.Equal(expectedValue, actualValue);
        }

        protected async Task AvgOfGoalsHomeTeamShouldBe1Point83Core()
        {
            double expectedValue = 1.83;
            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Function
                (
                    "cast",
                    nqb =>
                        nqb
                        .Avg<Result>("r", x => x.GoalsHomeTeam)
                        .CustomSql("as int")
                )
                .From()
                .Table<Result>("r");

            var actualValue = await Dao.ExecuteScalarAsync<int>(queryBuilder).ConfigureAwait(false);
            Assert.True(expectedValue - actualValue < 1);
        }

        protected async Task AvgOfGoalsVisitorTeamShouldBe2Core()
        {
            double expectedValue = 2;
            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Function
                (
                    "CAST",
                    nqb =>
                       nqb
                       .Avg<Result>("r", x => x.GoalsVisitorTeam)
                       .CustomSql("as int")
                )
                .From()
                .Table<Result>("r");

            var actualValue = await Dao.ExecuteScalarAsync<int>(queryBuilder).ConfigureAwait(false);
            Assert.True(expectedValue - actualValue < 1);
        }


        protected async Task AvgOfTotalGoalsShouldBe3Point83Core()
        {
            double expectedValue = 3.83;

            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Function
                (
                    "CAST",
                    nqb =>
                        nqb
                        .Avg<Result>
                        (
                            "r",
                            x => x.GoalsHomeTeam + x.GoalsVisitorTeam
                        )
                        .CustomSql("as int")
                )
                .From()
                .Table<Result>("r");

            var actualValue = await Dao.ExecuteScalarAsync<int>(queryBuilder).ConfigureAwait(false);
            Assert.True(expectedValue - actualValue < 1);
        }

        protected async Task GetCompleteResultShouldBe6ResultsCore()
        {
            Tuple<int, string, int, char, string, int, char>[] expectedTuples =
                new Tuple<int, string, int, char, string, int, char>[]
                {
                    new Tuple<int, string, int, char, string, int, char>( 1, "Fiorentina", 1, 'l', "Juventus", 3, 'v'),
                    new Tuple<int, string, int, char, string, int, char>( 1, "Milan", 0, 'l', "Inter", 1, 'v'),
                    new Tuple<int, string, int, char, string, int, char>( 2, "Inter", 4, 'v', "Fiorentina", 3, 'l'),
                    new Tuple<int, string, int, char, string, int, char>( 2, "Juventus", 2, 'd', "Milan", 2, 'd'),
                    new Tuple<int, string, int, char, string, int, char>( 3, "Fiorentina", 0, 'l', "Inter", 2, 'v'),
                    new Tuple<int, string, int, char, string, int, char>( 3, "Milan", 4, 'v', "Juventus", 1, 'l')
                }
                .OrderBy(x => x.Item1)
                .ThenBy(x => x.Item2)
                .ToArray();

            var queryBuilder =
                    Dao
                    .NewQueryBuilder()
                    .Select()
                    .Field<Day>("d", x => x.DayNumber)
                    .Comma()
                    .Field<Team>("ht", x => x.Name)
                    .Comma()
                    .Field<Result>("r", x => x.GoalsHomeTeam)
                    .Comma()
                    .Case()
                        .When<Result>("r", x => x.GoalsHomeTeam > x.GoalsVisitorTeam)
                        .Then("'v'")
                        .When<Result>("r", x => x.GoalsHomeTeam == x.GoalsVisitorTeam)
                        .Then("'d'")
                        .Else("'l'")
                    .EndCase()
                    .Comma()
                    .Field<Team>("vt", x => x.Name)
                    .Comma()
                    .Field<Result>("r", x => x.GoalsVisitorTeam)
                    .Comma()
                    .Case()
                        .When<Result>("r", x => x.GoalsHomeTeam < x.GoalsVisitorTeam)
                        .Then("'v'")
                        .When<Result>("r", x => x.GoalsHomeTeam == x.GoalsVisitorTeam)
                        .Then("'d'")
                        .Else("'l'")
                    .EndCase()
                    .From()
                    .Tables
                    (
                        FlatTable<Result>.WithAlias("r"),
                        InnerJoin<Day>.WithAlias("d"), (r, d) => r.DayId == d.DayId,
                        InnerJoin<Team>.WithAlias("ht"), (r, d, ht) => r.HomeTeamId == ht.TeamId,
                        InnerJoin<Team>.WithAlias("vt"), (r, d, ht, vt) => r.VisitorTeamId == vt.TeamId
                    )
                    .OrderBy<Day>("d", x => x.DayNumber)
                    .ThenBy<Team>("ht", x => x.Name);

            var actualTuples = await
                Dao
                .GetItemsArrayAsync<Tuple<int, string, int, char, string, int, char>>(queryBuilder)
                .ConfigureAwait(false);
            //.OrderBy(x => x.Item1).ThenBy(x => x.Item2).ToArray();

            Assert.Equal(expectedTuples, actualTuples);
        }


        protected async Task SelectTheFirstTwoResultsShouldBeIdRes1IdRes2Core()
        {
            Result[] expectedResults = new Result[]
            {
                new Result() { ResultId = "idRes1", HomeTeamId = "idFio", VisitorTeamId = "idJuv", GoalsHomeTeam = 1, GoalsVisitorTeam = 3, DayId = "idDay1" },
                new Result() { ResultId = "idRes2", HomeTeamId = "idMil", VisitorTeamId = "idInt", GoalsHomeTeam = 0, GoalsVisitorTeam = 1, DayId = "idDay1" }
            }.OrderBy(x => x.ResultId).ToArray();

            var queryBuilder =
                    Dao
                        .NewQueryBuilder()
                            .Select()
                                .Star("r")
                            .From()
                                .Table<Result>("r")
                            .OrderBy<Result>("r", x => x.ResultId)
                            .WithPagination(null, 2, false);

            var actualResults = (await Dao.GetItemsAsync<Result>(queryBuilder).ConfigureAwait(false)).OrderBy(x => x.ResultId).ToArray();
            Assert.Equal(expectedResults, actualResults);
        }


        protected async Task SelectTheLastTwoResultsShouldBeIdRes5IdRes6Core()
        {
            Result[] expectedResults = new Result[]
            {
                new Result() { ResultId = "idRes5", HomeTeamId = "idFio", VisitorTeamId = "idInt", GoalsHomeTeam = 0, GoalsVisitorTeam = 2, DayId = "idDay3" },
                new Result() { ResultId = "idRes6", HomeTeamId = "idMil", VisitorTeamId = "idJuv", GoalsHomeTeam = 4, GoalsVisitorTeam = 1, DayId = "idDay3" }
            }.OrderBy(x => x.ResultId).ToArray();

            var queryBuilder =
                    Dao
                        .NewQueryBuilder()
                            .Select()
                                .Star("r")
                            .From()
                                .Table<Result>("r")
                            .WithPagination(4, 2, true);

            var actualResults = (await Dao.GetItemsAsync<Result>(queryBuilder).ConfigureAwait(false)).OrderBy(x => x.ResultId).ToArray();
            Assert.Equal(expectedResults, actualResults);
        }


        protected async Task SelectResultsWithMaxSumOfGoalsShouldBeIdRes3Core()
        {
            Result expectedResult = new Result()
            {
                ResultId = "idRes3",
                HomeTeamId = "idInt",
                VisitorTeamId = "idFio",
                GoalsHomeTeam = 4,
                GoalsVisitorTeam = 3,
                DayId = "idDay2"
            };

            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Star("r")
                .From()
                .Table<Result>("r")
                .GroupBy()
                .Fields<Result>("r", x => x.ResultId, x => x.HomeTeamId, x => x.VisitorTeamId, x => x.GoalsHomeTeam, x => x.GoalsVisitorTeam, x => x.DayId)
                .Having()
                .OpenPar()
                    .Field<Result>("r", x => x.GoalsHomeTeam)
                    .OperPlus()
                    .Field<Result>("r", x => x.GoalsVisitorTeam)
                .ClosePar()
                .CustomSql("=")
                .OpenPar()
                    .Select()
                    .Max<Result>
                    (
                        "r",
                        x => x.GoalsHomeTeam + x.GoalsVisitorTeam
                    )
                    .From()
                    .Table<Result>("r")
                .ClosePar();

            var actualResult = (await Dao.GetItemsAsync<Result>(queryBuilder).ConfigureAwait(false)).FirstOrDefault();
            Assert.Equal(expectedResult, actualResult);
        }


        protected async Task SelectTheFirstDayWithTupleExceptionShouldBeCore()
        {
            await
                Assert
                .ThrowsAsync<FormatException>
                (
                    async () =>
                    {
                        try
                        {
                            await SelectTheFirstDayWithTupleExceptionShouldBeTwoResults().ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            throw ex.UnwrapAggregateException();
                        }
                    }
                )
                .ConfigureAwait(false);
        }


        protected async Task SelectResultsByIdWithDictionaryExceptionShouldBeCore()
        {
            await
                Assert
                .ThrowsAsync<NullReferenceException>
                (
                    async () =>
                    {
                        try
                        {
                            await SelectResultsByIdWithDictionaryExceptionShouldBeIdRes4();
                        }
                        catch (Exception ex)
                        {
                            throw ex.UnwrapAggregateException();
                        }
                    }
                );
        }


        protected async Task SelectResultsByIdWithDictionaryShouldBeIdRes4Core()
        {
            Result result = null;
            string goalsHomeTeamField = result.GetFieldName<Result>(x => x.GoalsHomeTeam);
            string goalsVisitorTeamField = result.GetFieldName<Result>(x => x.GoalsVisitorTeam);

            IDictionary<string, object> expectedValue = new Dictionary<string, object>
            {
                { result.GetFieldName<Result>(x => x.ResultId), "idRes4" },
                { result.GetFieldName<Result>(x => x.HomeTeamId), "idJuv" },
                { result.GetFieldName<Result>(x => x.VisitorTeamId), "idMil" },
                { result.GetFieldName<Result>(x => x.DayId), "idDay2" }
            };

            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Star("r")
                .From()
                .Table<Result>("r")
                .Where<Result>("r", x => x.ResultId == "idRes4");

            var actualValue = (await Dao.GetItemsAsync<IDictionary<string, object>>(queryBuilder).ConfigureAwait(false)).FirstOrDefault();

            var numberType = actualValue[goalsHomeTeamField].GetType().Name;

            if (numberType.Equals("Int32"))
            {
                expectedValue.Add(goalsHomeTeamField, 2);
                expectedValue.Add(goalsVisitorTeamField, 2);
            }
            else
            {
                expectedValue.Add(goalsHomeTeamField, 2L);
                expectedValue.Add(goalsVisitorTeamField, 2L);
            }

            Assert.Equal(expectedValue.OrderBy(x => x.Key).ToArray(), actualValue.OrderBy(x => x.Key).ToArray());
        }


        protected async Task SumOfTotalGoalsForResultWithDictionaryShouldBeIdRes1IdRes2IdRes3IdRes4IdRes5IdRes6Core()
        {
            Result result = null;
            string idResultFieldName = result.GetFieldName<Result>(x => x.ResultId);
            KeyValuePair<string, object>[] expectedTuples = null;

            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Field<Result>("r", x => x.ResultId)
                .Comma()
                .Sum
                (
                    nqb =>
                        nqb
                        .Field<Result>("r", x => x.GoalsHomeTeam)
                        .OperPlus()
                        .Field<Result>("r", x => x.GoalsVisitorTeam)
                )
                .As("sum")
                .From()
                .Table<Result>("r")
                .GroupBy()
                .Field<Result>("r", x => x.ResultId);

            var actualTuples = (await Dao.GetItemsAsync<IDictionary<string, object>>(queryBuilder).ConfigureAwait(false)).SelectMany(x => x).OrderBy(x => x.Key).ThenBy(x => x.Value).ToArray();

            var sumType = actualTuples[actualTuples.Length / 2].Value.GetType().Name;

            if (sumType.Equals("Int32"))
            {
                expectedTuples = new Dictionary<string, object>[]
                {
                    new Dictionary<string, object>()
                    {
                        { idResultFieldName, "idRes1" },
                        { "sum", 4 }
                    },
                    new Dictionary<string, object>()
                    {
                        { idResultFieldName, "idRes2" },
                        { "sum", 1 }
                    },
                    new Dictionary<string, object>()
                    {
                        { idResultFieldName, "idRes3" },
                        { "sum", 7 }
                    },
                    new Dictionary<string, object>()
                    {
                        { idResultFieldName, "idRes4" },
                        { "sum", 4 }
                    },
                    new Dictionary<string, object>()
                    {
                        { idResultFieldName, "idRes5" },
                        { "sum", 2 }
                    },
                    new Dictionary<string, object>()
                    {
                        { idResultFieldName, "idRes6" },
                        { "sum", 5 }
                    }
                }.SelectMany(x => x).OrderBy(x => x.Key).ThenBy(x => x.Value).ToArray();
            }
            else if (sumType.Equals("Int64"))
            {
                expectedTuples = new Dictionary<string, object>[]
                {
                    new Dictionary<string, object>()
                    {
                        { idResultFieldName, "idRes1" },
                        { "sum", 4L }
                    },
                    new Dictionary<string, object>()
                    {
                        { idResultFieldName, "idRes2" },
                        { "sum", 1L }
                    },
                    new Dictionary<string, object>()
                    {
                        { idResultFieldName, "idRes3" },
                        { "sum", 7L }
                    },
                    new Dictionary<string, object>()
                    {
                        { idResultFieldName, "idRes4" },
                        { "sum", 4L }
                    },
                    new Dictionary<string, object>()
                    {
                        { idResultFieldName, "idRes5" },
                        { "sum", 2L }
                    },
                    new Dictionary<string, object>()
                    {
                        { idResultFieldName, "idRes6" },
                        { "sum", 5L }
                    }
                }.SelectMany(x => x).OrderBy(x => x.Key).ThenBy(x => x.Value).ToArray();
            }
            else
            {
                expectedTuples = new Dictionary<string, object>[]
                {
                    new Dictionary<string, object>()
                    {
                        { idResultFieldName, "idRes1" },
                        { "sum", 4m }
                    },
                    new Dictionary<string, object>()
                    {
                        { idResultFieldName, "idRes2" },
                        { "sum", 1m }
                    },
                    new Dictionary<string, object>()
                    {
                        { idResultFieldName, "idRes3" },
                        { "sum", 7m }
                    },
                    new Dictionary<string, object>()
                    {
                        { idResultFieldName, "idRes4" },
                        { "sum", 4m }
                    },
                    new Dictionary<string, object>()
                    {
                        { idResultFieldName, "idRes5" },
                        { "sum", 2m }
                    },
                    new Dictionary<string, object>()
                    {
                        { idResultFieldName, "idRes6" },
                        { "sum", 5m }
                    }
                }.SelectMany(x => x).OrderBy(x => x.Key).ThenBy(x => x.Value).ToArray();
            }

            Assert.Equal(expectedTuples, actualTuples);
        }


        protected async Task SelectTeamFromConcatConditionShouldBeIdJuvCore()
        {
            Team expectedTeam =
                new Team
                {
                    TeamId = "idJuv",
                    Name = "Juventus",
                    City = "Turin",
                    President = "Agnelli"
                };

            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Star("t")
                .From()
                .Table<Team>("t")
                .Where()
                .Condition<Team>("t", x => SqlFunctions.StrConcat(x.Name, " - ", x.City) == "Juventus - Turin");

            var actualTeam = (await Dao.GetItemsAsync<Team>(queryBuilder).ConfigureAwait(false)).FirstOrDefault();
            Assert.Equal(expectedTeam, actualTeam);
        }


        protected async Task DeleteResultByIdShouldBeIdRes4Core()
        {
            string idResult = "idRes4";
            await Dao.DoInTransactionAsync
            (
               async tDao =>
               {
                   var expectedTeams =
                   InitialResults
                   .Where(x => x.ResultId != idResult)
                   .OrderBy(x => x.ResultId)
                   .ToArray();
                   int i = await tDao.DeleteByConditionAsync<Result>(x => x.ResultId == idResult);
                   var actualTeams = (await tDao.GetAllItemsArrayAsync<Result>().ConfigureAwait(false)).OrderBy(x => x.ResultId).ToArray();
                   Assert.Equal(expectedTeams, actualTeams);
                   throw new Exception();
               }
            ).ConfigureAwait(false);

        }


        protected async Task CheckIfADayExistsShouldBeIdDay1Core()
        {
            bool expectedBool = true;
            Day day = new Day()
            {
                DayId = "idDay1",
                DayDate = new DateTime(2018, 09, 16),
                DayNumber = 1
            };

            bool actualBool = await Dao.EntityExistsAsync(day).ConfigureAwait(false);
            Assert.Equal(expectedBool, actualBool);
        }


        protected async Task CheckIfTheErrorIsASyntaxErrorShouldBeTrueCore()
        {
            bool expectedBool = true;
            try
            {
                var queryBuilder =
                        Dao
                            .NewQueryBuilder()
                                .CustomSql("SELECTION * FROM RESULTS");

                var results = await Dao.GetItemsAsync<Result>(queryBuilder).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                bool actualBool = Dao.ErrorIsBadSqlGrammar(ex);
                Assert.Equal(expectedBool, actualBool);
            }
        }


        protected async Task CheckIfTheErrorIsADuplicateKeyErrorShouldBeIdDay1Core()
        {
            bool expectedBool = true;
            try
            {
                Day day = new Day()
                {
                    DayId = "idDay1",
                    DayDate = new DateTime(2018, 09, 16),
                    DayNumber = 1
                };

                await Dao.InsertEntityAsync(day).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                bool actualBool = Dao.ErrorIsDuplicateKey(ex);
                Assert.Equal(expectedBool, actualBool);
            }
        }


        protected async Task GetTeamTableMetadataShouldBeFourCore()
        {
            var actualMetadata = await Dao.GetMetadataForEntityAsync<Team>().ConfigureAwait(false);

            var metadataRows = actualMetadata.Select("ColumnName = 'TEAM_ID' OR ColumnName = 'NAME' OR ColumnName = 'CITY' OR ColumnName = 'PRESIDENT'");

            Assert.Equal(4, metadataRows.Count());
        }


        protected async Task InsertAndDeleteAResultInTransactionShouldBeIdRes7Core()
        {
            string newResId = "idRes7";
            Result newResult =
                new Result()
                {
                    ResultId = newResId,
                    HomeTeamId = "idMil",
                    VisitorTeamId = "idInt",
                    GoalsHomeTeam = 1,
                    GoalsVisitorTeam = 3,
                    DayId = "idDay1"
                };

            var expectedDays =
                InitialResults
                .Where(x => x.ResultId != "idRes2") // && x.ResultId != "idRes4")
                .Concat(newResult.AsArray())
                .OrderBy(x => x.ResultId)
                .ToArray();

            //    new Result[]
            //{
            //    new Result() { ResultId = "idRes1", HomeTeamId = "idFio", VisitorTeamId = "idJuv", GoalsHomeTeam = 1, GoalsVisitorTeam = 3, DayId = "idDay1" },
            //    new Result() { ResultId = "idRes3", HomeTeamId = "idInt", VisitorTeamId = "idFio", GoalsHomeTeam = 4, GoalsVisitorTeam = 3, DayId = "idDay2" },
            //    new Result() { ResultId = "idRes4", HomeTeamId = "idJuv", VisitorTeamId = "idMil", GoalsHomeTeam = 2, GoalsVisitorTeam = 2, DayId = "idDay2" },
            //    new Result() { ResultId = "idRes5", HomeTeamId = "idFio", VisitorTeamId = "idInt", GoalsHomeTeam = 0, GoalsVisitorTeam = 2, DayId = "idDay3" },
            //    new Result() { ResultId = "idRes6", HomeTeamId = "idMil", VisitorTeamId = "idJuv", GoalsHomeTeam = 4, GoalsVisitorTeam = 1, DayId = "idDay3" },
            //    newResult
            //}.OrderBy(x => x.ResultId).ToArray();

            await
                Dao
                .DoInTransactionAsync
                (
                    async tDao =>
                    {
                        await tDao.InsertEntityAsync(newResult).ConfigureAwait(false);
                        await tDao.DeleteByConditionAsync<Result>(x => x.ResultId == "idRes2");
                        var actualDays =
                                            tDao
                                            .GetAllItems<Result>()
                                            .OrderBy(x => x.ResultId)
                                            .ToArray();

                        Assert.Equal(expectedDays, actualDays);
                        throw new Exception();
                    }
                )
                .ConfigureAwait(false);
            Assert.Equal(InitialResults, Dao.GetAllItems<Result>().OrderBy(x => x.ResultId).ToArray());



        }


        protected async Task InsertDuplicateDayKeyInTransactionWithCorrectionShouldBeIdDay1Core()
        {
            Day excDay = new Day() { DayId = "idDay1", DayDate = new DateTime(2018, 09, 19), DayNumber = 4 };
            Day newDay = new Day() { DayId = "idDay4", DayDate = new DateTime(2018, 09, 19), DayNumber = 4 };

            await
                Dao
                .DoInTransactionAsync
                (
                    async tDao =>
                    {
                        await tDao.InsertEntityAsync(newDay).ConfigureAwait(false);
                        bool newDayExist = await tDao.EntityExistsAsync(newDay);
                        Assert.True(newDayExist);
                        await tDao.InsertEntityAsync(excDay).ConfigureAwait(false);
                    }

                //await ReplaceDuplicateKey(Dao, newDay, "idDay4").ConfigureAwait(false);
                //var actualDays = (
                //                     await Dao
                //                     .GetAllItemsArrayAsync<Day>()
                //                     .ConfigureAwait(false)
                //                  )
                //                 .OrderBy(x => x.DayId)
                //                 .ToArray();

                //Day[] expectedDays = expectedDays = new Day[]
                //{
                //    new Day() { DayId = "idDay1", DayDate = new DateTime(2018, 09, 16), DayNumber = 1 },
                //    new Day() { DayId = "idDay2", DayDate = new DateTime(2018, 09, 17), DayNumber = 2 },
                //    new Day() { DayId = "idDay3", DayDate = new DateTime(2018, 09, 18), DayNumber = 3 },
                //}
                //.OrderBy(x => x.DayId).ToArray();
                //Assert.Equal(expectedDays, actualDays);

                )
                .ConfigureAwait(false);
            bool newDayDontExist = await Dao.EntityExistsAsync(newDay);
            Assert.False(newDayDontExist);
        }


        protected async Task InsertTwoDaysInTwoTransactionsShouldBeIdDay4IdDay1Core()
        {
            Day newDay = new Day() { DayId = "idDay4", DayDate = new DateTime(2019, 2, 21), DayNumber = 4 };
            Day excDay = new Day() { DayId = "idDay1", DayDate = new DateTime(2018, 09, 19), DayNumber = 4 };
            //Day day5 = new Day() { DayId = "idDay1", DayDate = new DateTime(2018, 09, 20), DayNumber = 5 };

            await
                Dao
                .DoInTransactionAsync
                (
                    async tDao =>
                    {
                        //          await tDao.InsertEntityAsync(day6).ConfigureAwait(false);
                        await
                            tDao
                            .DoInTransactionAsync
                            (
                                async ttDao =>
                                {
                                    await ttDao.InsertEntityAsync(newDay).ConfigureAwait(false);
                                    bool newDayExist = await ttDao.EntityExistsAsync(newDay);
                                    Assert.True(newDayExist);
                                },
                                async ex =>
                                {
                                    //await ReplaceDuplicateKey(tDao, newDay, "idDay4").ConfigureAwait(false);
                                    bool newDayExist = await tDao.EntityExistsAsync(newDay);
                                    Assert.False(newDayExist);
                                }

                            );
                        //var actualDays = (
                        //await tDao.GetAllItemsArrayAsync<Day>()
                        //.ConfigureAwait(false)
                        //)
                        //.OrderBy(x => x.DayId)
                        //.ToArray();

                        //Day[] expectedDays =
                        //    InitialDays
                        //    .Concat
                        //    (
                        //        new Day[]
                        //        {
                        //            //new Day() { DayId = "idDay4", DayDate = new DateTime(2018,9,19), DayNumber = 4 },
                        //            new Day() {
                        //                        DayId = "idDay4",
                        //                        DayDate = newDay.DayDate,
                        //                        DayNumber = newDay.DayNumber
                        //                       }
                        //        }
                        //    )
                        //    .OrderBy(x => x.DayId)
                        //    .ToArray();

                        //Assert.Equal(expectedDays, actualDays);
                        //throw new Exception();
                    }

                )
                .ConfigureAwait(false);

            //var actualDays = (await Dao.GetAllItemsArrayAsync<Day>().ConfigureAwait(false)).OrderBy(x => x.DayId).ToArray();

            //Day[] expectedDays =
            //    InitialDays
            //    .Concat
            //    (
            //        new Day[]
            //        {
            //            //new Day() { DayId = "idDay4", DayDate = new DateTime(2018,9,19), DayNumber = 4 },
            //            new Day() { DayId = "idDay4", DayDate = day4.DayDate, DayNumber = day4.DayNumber }
            //        }
            //    )
            //    .OrderBy(x => x.DayId)
            //    .ToArray();

            //Assert.Equal(expectedDays, actualDays);
        }


        protected async Task GetTableColumnNamesShouldBeSixStringsCore()
        {
            string[] expectedNames = new string[]
            {
                "RESULT_ID", "HOME_TEAM_ID", "VISITOR_TEAM_ID", "GOALS_HOME_TEAM", "GOALS_VISITOR_TEAM", "DAY_ID"
            };
            string tableName = "RESULTS";
            var actualNames = (await Dao.GetColumnNamesForTableAsync(tableName).ConfigureAwait(false)).ToArray();
            Assert.Equal(expectedNames, actualNames);
        }


        protected async Task GetEntityObjectNameShouldBeTeamsCore()
        {
            string expectedName = "TEAMS";
            string actualName = (await Dao.CreateObjectDefinitionForTable(expectedName).ConfigureAwait(false)).EntityName;
            Assert.Equal(expectedName, actualName);
        }


        protected async Task GetResultsTotalPaginationNumberShouldBe6Core()
        {
            int? expectedNumber = 6;
            var queryBuilder =
                Dao
                .NewQueryBuilder()
                    .Select()
                        .Star("r")
                    .From()
                        .Table<Result>("r")
                    .WithPagination(null, 2, true);

            int? actualNumber = (await Dao.GetPaginatedItemsAsync<Result>(queryBuilder).ConfigureAwait(false)).PaginationTotalCounts;
            Assert.Equal(expectedNumber, actualNumber);
        }


        protected async Task CheckIfPKIndexIsAlsoUniqueShouldBeResultsCore()
        {
            string table = "RESULTS";
            string schema = Schema;

            var indexes =
                Dao
                .GetDbIndexesMetadataForTableAsync(table, schema)
                .Result
                .OrderBy(x => x.ConstraintName)
                .ToArray();

            for (int i = 0; i < indexes.Length; i++)
            {
                if (indexes[i].IsPrimaryKey)
                {
                    Assert.Equal(indexes[i].IsUnique, indexes[i].IsPrimaryKey);
                }
            }
        }


        protected async Task SelectResultsForADayShouldBeDay1Core()
        {
            string dayId = "idDay1";

            var expectedResults =
                new Tuple<Result, Day>[]
                {
                    new Tuple<Result, Day>(new Result() { ResultId = "idRes1", HomeTeamId = "idFio", VisitorTeamId = "idJuv", GoalsHomeTeam = 1, GoalsVisitorTeam = 3, DayId = "idDay1" }, new Day() { DayId = "idDay1", DayDate = new DateTime(2018, 09, 16), DayNumber = 1 }),
                    new Tuple<Result, Day>(new Result() { ResultId = "idRes2", HomeTeamId = "idMil", VisitorTeamId = "idInt", GoalsHomeTeam = 0, GoalsVisitorTeam = 1, DayId = "idDay1" }, new Day() { DayId = "idDay1", DayDate = new DateTime(2018, 09, 16), DayNumber = 1 })
                }
                .OrderBy(x => x.Item1.ResultId)
                .ToArray();

            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Star("r")
                .Comma()
                .Star("d")
                .From()
                .Tables
                (
                    FlatTable<Result>.WithAlias("r"),
                    InnerJoin<Day>.WithAlias("d"), (r, d) => r.DayId == d.DayId
                )
                .Where<Result>("r", x => x.DayId == dayId)
                .OrderBy<Result>("r", x => x.ResultId);

            var actualResults = await
                Dao
                .GetItemsArrayAsync<Tuple<Result, Day>>(queryBuilder)
                .ConfigureAwait(false);

            Assert.Equal(expectedResults, actualResults);
        }

        protected async Task SelectResultsForADayWithInvertedTuplesShouldBeDay1Core()
        {
            await
                Assert
                .ThrowsAsync<EqualException>
                (
                    async () =>
                    {
                        try
                        {
                            await SelectResultsForADayWithInvertedTuplesShouldBeCore("idDay1");
                        }
                        catch (Exception ex)
                        {
                            throw ex.UnwrapAggregateException();
                        }
                    }
                );
        }


        protected async Task SelectResultsForADayWithIncompletedFieldsShouldBeDay1Core()
        {
            string dayId = "idDay1";
            var expectedResults =
                new Tuple<Result, Day>[]
                {
                    new Tuple<Result, Day>(new Result() { ResultId = "idRes1", HomeTeamId = "idFio", VisitorTeamId = "idJuv", DayId = "idDay1" }, new Day() { DayId = "idDay1" }),
                    new Tuple<Result, Day>(new Result() { ResultId = "idRes2", HomeTeamId = "idMil", VisitorTeamId = "idInt", DayId = "idDay1" }, new Day() { DayId = "idDay1" })
                }
                .OrderBy(x => x.Item1.ResultId)
                .ToArray();

            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Field<Result>("r", x => x.ResultId)
                .Comma()
                .Field<Result>("r", x => x.HomeTeamId)
                .Comma()
                .Field<Result>("r", x => x.VisitorTeamId)
                .Comma()
                .Field<Result>("r", x => x.DayId)
                .Comma()
                .Field<Day>("d", x => x.DayId)
                .From()
                .Tables
                (
                    FlatTable<Result>.WithAlias("r"),
                    InnerJoin<Day>.WithAlias("d"), (r, d) => r.DayId == d.DayId
                )
                .Where<Result>("r", x => x.DayId == dayId);

            var actualResults = (await Dao.GetItemsAsync<Tuple<Result, Day>>(queryBuilder).ConfigureAwait(false)).OrderBy(x => x.Item1.ResultId).ToArray();

            Assert.Equal(expectedResults, actualResults);
        }

        protected async Task SelectDoubleFirstDayShouldBeDay1Core()
        {
            var expectedDays =
                new Tuple<Day, Day>
                (
                    new Day
                    {
                        DayId = "idDay1",
                        DayDate = new DateTime(2018, 09, 16),
                        DayNumber = 1
                    },
                    new Day
                    {
                        DayId = "idDay1",
                        DayDate = new DateTime(2018, 09, 16),
                        DayNumber = 1
                    }
                );

            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Star("d")
                .Comma()
                .Star("d")
                .From()
                .Table<Day>("d")
                .Where<Day>("d", x => x.DayId == "idDay1");

            var actualDays = (await Dao.GetItemsAsync<Tuple<Day, Day>>(queryBuilder).ConfigureAwait(false)).FirstOrDefault();

            Assert.Equal(expectedDays, actualDays);
        }

        protected async Task CountWithNestedQueryShouldBeTheSameCore()
        {
            int maxDays = 3;

            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .CountStar("X")
                .Comma()
                .NestedQuery
                (
                    nqb =>
                        nqb
                        .Select()
                        .CountStar()
                        .From()
                        .Table<Day>("d2")
                        .Where<Day>("d2", x => x.DayNumber <= maxDays)
                )
                .As("Y")
                .From()
                .Table<Day>("d")
                .Where<Day>("d", x => x.DayNumber <= maxDays);

            var actualDays = (await Dao.GetItemsAsync<Tuple<int, int>>(queryBuilder).ConfigureAwait(false)).FirstOrDefault();

            Assert.True(actualDays.Item1 == actualDays.Item2);
        }

        protected async Task SearchByDynamicCriteriaCore()
        {
            try
            {
                var queryBuilder =
                    Dao
                    .NewQueryBuilder()
                    .Select()
                    .AllFields<Day>("D")
                    .From()
                    .Table<Day>("D")
                    .Where()
                    .True()
                    .AndConditionsForCriteria<Day>("D", new { DayId = "2", NonExistingProp = 5, DAY_DATE = DateTime.Today }, true, "DAY_DATE");

                var twp = queryBuilder.BuildSqlTextWithParameters();
                var xx = twp.ReplaceParametersInSqlStringForImmediateExecute();

                var actualDays = (await Dao.GetItemsArrayAsync<Day>(queryBuilder).ConfigureAwait(false));


                queryBuilder =
                    Dao
                    .NewQueryBuilder()
                    .Select()
                    .AllFields<Day>("D")
                    .From()
                    .Table<Day>("D")
                    .Where()
                    .True()
                    .AndConditionsForCriteria<Day, Day>("D", new Day { DayNumber = 1 }, true, x => x.DayDate);

                twp = queryBuilder.BuildSqlTextWithParameters();


                //Assert.True(actualDays.Item1 == actualDays.Item2);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        protected void TestForNullShouldProduceIsNullOperatorCore()
        {
            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Star("r")
                .From()
                .Table<Result>("r")
                .Where<Result>("r", r => SqlFunctions.IsNull(r.VisitorTeamId) && (r.DayId == "1" || r.DayId == "2"));

            var swp = queryBuilder.BuildSqlTextWithParameters();
            try
            {
                var ss = swp.ReplaceParametersInSqlStringForImmediateExecute();

                Assert.Contains("is null", ss, StringComparison.InvariantCultureIgnoreCase);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        protected void TestForCoalesceCore()
        {
            var queryBuilder =
                Dao
                .NewQueryBuilder()
                .Select()
                .Coalesce<Result, Day>("r", x => x.HomeTeamId, "d", x => x.DayDate, "SMTNG")
                .From()
                .Table<Result>("r")
                .TablesJoin<Result, Day>("d", InnerJoin<Day>.WithAlias("d"), (r, d) => true);

            var twp = queryBuilder.BuildSqlTextWithParameters();
            var sqlActualJoin = twp.SqlText;

            //Assert.Contains("coalesce", sqlActualJoin, StringComparison.InvariantCultureIgnoreCase);
        }

        protected async Task SelectMemberByIdAndPasswordShouldBeOneMember(string username, string password)
        {
            Member expectedMember =
                InitialMembers
                .Where(x => x.MemberName == username && x.Password == password)
                .First();

            Member actualMember = await Dao.GetItemByExampleAsync<Member>(x => (x.MemberName == username && x.Password == password));
            Assert.Equal(expectedMember.MemberId, actualMember.MemberId);
        }

        protected async Task UpdateHoursByJobTimeInJOB_TIMEShouldBeInt(int hours, JobTime jobTime)
        {
            decimal? expectedHours = jobTime.Hours + hours;
            await Dao.DoInTransactionAsync
            (
                async tDao =>
                {
                    var updateBuilder = tDao.NewUpdateTableBuilder<JobTime>().
                    Set(x => x.Hours, x => x.Hours + hours)
                        .Where
                        (
                            x => x.ReasonId == jobTime.ReasonId
                            && x.MemberId == jobTime.MemberId
                            && x.JobId == jobTime.JobId
                            && x.DateOfWork == jobTime.DateOfWork
                            && SqlFunctions.IsNull("")
                         );
                    int affected = await tDao.UpdateTableAsync(updateBuilder).ConfigureAwait(false);
                    Assert.Equal(1, affected);
                    decimal? actualHours =
                        (
                            await tDao.GetItemByExampleAsync<JobTime>
                            (
                                x => x.ReasonId == jobTime.ReasonId
                                && x.MemberId == jobTime.MemberId
                                && x.JobId == jobTime.JobId
                                && x.DateOfWork == jobTime.DateOfWork
                            ).ConfigureAwait(false)
                        )?.Hours;
                    Assert.Equal(expectedHours, actualHours);
                    throw new Exception();
                }
            )
            .ConfigureAwait(false);
        }

        protected async Task SelectJobTimesOfTheDayByDateOfWorkAndId218ShuoldBe()
        {
            var dt = PrefixDate;

            JobTimesOfTheDay[] expectedJobTimesOfTheDay =

                new JobTimesOfTheDay[]
                {
                    new JobTimesOfTheDay
                    {
                        JobId = 3,
                        Hours = 3,
                        Dateofwork = PrefixDate,
                        ReasonId = 6,
                        MemberId = 218,
                        Job = "DTCO as Product"
                    },
                    new JobTimesOfTheDay
                    {
                        JobId = 4,
                        Hours = 3,
                        Dateofwork = PrefixDate,
                        ReasonId = 7,
                        MemberId = 218,
                        Job = "NausSys for La MDS Yacht Ser"
                    }
                };
            var query =
                Dao
                .NewQueryBuilder()
                .Select()
                .Field<JobTime>("JT", x => x.Hours, expectedJobTimesOfTheDay[0].GetFieldName(x => x.Hours))
                .Comma()
                .Field<Job>("J", x => x.JobName, "job")
                .Comma()
                .Field<JobTime>("JT", x => x.DateOfWork, "dateOfWork")
                .Comma()
                .Field<Job>("J", x => x.JobId, "jobId")
                .Comma()
                .Field<Reason>("R", x => x.ReasonId, "reasonId")
                .Comma()
                .Field<JobTime>("JT", x => x.MemberId, "memberId")
                .From()
                .Tables
                (
                    FlatTable<JobTime>.WithAlias("JT"),
                    InnerJoin<Reason>.WithAlias("R"), (JT, R) => JT.ReasonId == R.ReasonId,
                    InnerJoin<Job>.WithAlias("J"), (JT, R, J) => JT.JobId == J.JobId
                )
                .Where<JobTime>("JT", JT => JT.MemberId == 218 && JT.DateOfWork == dt)
                //.And<JobTime>("JT", x => x.DateOfWork, WhereOperator.EqualTo, new DateTime())
                .OrderBy<JobTime>("JT", JT => JT.Hours, OrderByDirection.Descending);

            JobTimesOfTheDay[] result = await Dao.GetItemsArrayAsync<JobTimesOfTheDay>(query).ConfigureAwait(false);

            Assert.Equal(result, expectedJobTimesOfTheDay);
        }

        protected async Task SumHoursOfJobTimeAggregateByMemberIdAndDateOfWorkShuoldBeCore()
        {
            int expectedHours = 6;
            var query = Dao
            .NewQueryBuilder()
            .Select()
            .Sum<JobTime>("JT", x => x.Hours)
            .From()
            .Table<JobTime>("JT")
            .Where<JobTime>("JT", x => x.MemberId == 218)
            .And<JobTime>("JT", x => x.DateOfWork, WhereOperator.EqualTo, PrefixDate);
            int result = await Dao.ExecuteScalarAsync<int>(query);
            Assert.Equal(expectedHours, result);
        }

        protected async Task CheckJobTimeExistShuoldBeCore()
        {
            bool expectedBoolean = true;
            JobTime jobTime = new JobTime
            {
                JobId = 1,
                MemberId = 213,
                DateOfWork = PrefixDate,
                AmountTimeToInvoice = null,
                FreeAmountTime = null,
                TimeNote = null,
                ReasonId = 3,
                Hours = 3,
            };
            bool result = await Dao.EntityExistsAsync(jobTime);
            Assert.Equal(result, expectedBoolean);
        }


        internal class IndexConstraintDbItemComparer : System.Collections.IComparer, IComparer<IndexConstraintDbItem>, IEqualityComparer<IndexConstraintDbItem>
        {
            public int Compare(IndexConstraintDbItem x, IndexConstraintDbItem y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }

                if (x == null)
                {
                    return -1;
                }

                if (y == null)
                {
                    return 1;
                }

                Func<IndexConstraintDbItem, string> toString =
                    idx => $"{idx.ConstraintName}_{idx.TableName}_{idx.SchemaName}_{idx.IsUnique}_{idx.ReferencedField}_{idx.FieldOrder}_{idx.IsPrimaryKey}";

                return toString(x).CompareTo(toString(y));
            }

            public int Compare(object x, object y)
            {
                return Compare(x as IndexConstraintDbItem, y as IndexConstraintDbItem);
            }

            public bool Equals(IndexConstraintDbItem x, IndexConstraintDbItem y)
            {
                return Compare(x, y) == 0;
            }

            public int GetHashCode(IndexConstraintDbItem obj)
            {
                return obj.BuildHashCode();
            }
        }

        internal class FKConstraintDbItemComparer : System.Collections.IComparer, IComparer<FKConstraintDbItem>, IEqualityComparer<FKConstraintDbItem>
        {
            public int Compare(FKConstraintDbItem x, FKConstraintDbItem y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }

                if (x == null)
                {
                    return -1;
                }

                if (y == null)
                {
                    return 1;
                }

                Func<FKConstraintDbItem, string> toString =
                    idx => $"{idx.ConstraintName}_{idx.TableName}_{idx.SchemaName}_{idx.ColumnName}_{idx.ReferencedTable}_{idx.ReferencedField}";

                return toString(x).CompareTo(toString(y));
            }

            public int Compare(object x, object y)
            {
                return Compare(x as FKConstraintDbItem, y as FKConstraintDbItem);
            }

            public bool Equals(FKConstraintDbItem x, FKConstraintDbItem y)
            {
                return Compare(x, y) == 0;
            }

            public int GetHashCode(FKConstraintDbItem obj)
            {
                return obj.BuildHashCode();
            }
        }


    }

}
