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

namespace Kyklos.Kernel.Data.PostgreSQL.Test.NetCore
{
    public class PostgreSQLDatasyncModifyDB : BaseDatasyncTest
    {
        protected override string Schema => "public";
        protected override string ProviderName => "PostgreSQL";
        

        private void Setup()
        {
            SetupCore().Wait();
        }

        public PostgreSQLDatasyncModifyDB() : base(XUnitTestSupport.NetPlatformType.NETCORE)
        {
            //JsonConvert.DefaultSettings =
            //    () =>
            //        new JsonSerializerSettings
            //        {
            //            Formatting = Newtonsoft.Json.Formatting.None,
            //            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            //        };

            SetupCore().Wait();

            //Setup();
        }

        private async Task SetupCore()
        {
            await PrepareDB().ConfigureAwait(false);
            await GenerateScriptsForCreateAndDropSequence().ConfigureAwait(false);
            await AddTeams().ConfigureAwait(false);
            await AddDays().ConfigureAwait(false);
            await AddResults().ConfigureAwait(false);
            await AddMembers().ConfigureAwait(false);
            await AddJobs().ConfigureAwait(false);
            await AddReasons().ConfigureAwait(false);
            await AddJobTimes().ConfigureAwait(false);
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
        public async Task FillDayDataTableShouldBe()
        {
            string sql = @"SELECT d.* FROM ""DAYS"" d";

            await FillDayDataTableShouldBeCore(sql);
        }
        [Fact]
        public Task CountAllResultsAfterFourAreDeletedShouldBe2()
        {
            return CountAllResultsAfterFourAreDeletedShouldBe1Core();
        }
        [Fact]
        public async Task DeleteResultByIdShouldBeIdRes4()
        {
            await DeleteResultByIdShouldBeIdRes4Core().ConfigureAwait(false);
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
        public async Task UpdateGoalsHomeTeamShouldBe3()
        {
            await UpdateGoalsHomeTeamShouldBe3Core().ConfigureAwait(false);
        }

        [Fact]
        public async Task UpsertTeamsShouldBe5Teams()
        {
            await UpsertTeamsShouldBe5TeamsCore().ConfigureAwait(false);
        }
    }
}
