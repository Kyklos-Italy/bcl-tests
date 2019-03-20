using System.Threading.Tasks;
using Xunit;
using XUnitTestSupport;

namespace Kyklos.Kernel.Data.MariaDB.Test.NetFramework
{
    [TestCaseOrderer("XUnitTestSupport.PriorityOrderer", "XUnitTestSupport")]
    public class MariaDBDatasyncTestCRUD : MariaDBDatasyncTestCommon
    {
        [Fact, TestPriority(10)]
        public async Task GetNextValueForSequenceShouldBe()
        {
            await GetNextValueForSequenceShouldBeCore(1L, "my_sequence").ConfigureAwait(false);
        }

        [Fact, TestPriority(20)]
        public async Task GetRangeValuesForSequenceShouldBe()
        {
            long[] range = new long[]
            {
                2L, 3L, 4L, 5L, 6L
            };

            await GetRangeValuesForSequenceShouldBeCore(range, "my_sequence").ConfigureAwait(false);
        }

        [Fact, TestPriority(30)]
        public async Task DeleteResultByIdShouldBeIdRes4()
        {
            await DeleteResultByIdShouldBeIdRes4Core().ConfigureAwait(false);
        }

        [Fact, TestPriority(40)]
        public async Task UpsertTeamsShouldBe5Teams()
        {
            await UpsertTeamsShouldBe5TeamsCore().ConfigureAwait(false);
        }

        [Fact, TestPriority(50)]
        public async Task UpdateGoalsHomeTeamShouldBe3()
        {
            await UpdateGoalsHomeTeamShouldBe3Core().ConfigureAwait(false);
        }

        [Fact, TestPriority(60)]
        public async Task InsertAndDeleteAResultInTransactionShouldBeIdRes7()
        {
            await InsertAndDeleteAResultInTransactionShouldBeIdRes7Core().ConfigureAwait(false);
        }

        [Fact, TestPriority(70)]
        public async Task InsertDuplicateDayKeyInTransactionWithCorrectionShouldBeIdDay1()
        {
            await InsertDuplicateDayKeyInTransactionWithCorrectionShouldBeIdDay1Core().ConfigureAwait(false);
        }

        [Fact, TestPriority(80)]
        public async Task InsertTwoDaysInTwoTransactionsShouldBeIdDay4IdDay1()
        {
            await InsertTwoDaysInTwoTransactionsShouldBeIdDay4IdDay1Core().ConfigureAwait(false);
        }

        [Fact, TestPriority(90)]
        public Task CountAllResultsAfterFourAreDeletedShouldBe1()
        {
            return CountAllResultsAfterFourAreDeletedShouldBe1Core();
        }
    }
}
