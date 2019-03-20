using System.Threading.Tasks;
using Xunit;

namespace Kyklos.Kernel.Data.MariaDB.Test.NetCore
{
    public class MariaDBDatasyncTestQuery6 : MariaDBDatasyncTestCommon
    {
        public MariaDBDatasyncTestQuery6()
        {            
        }

        #region Collection 6

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
        public async Task GetTableColumnNamesShouldBeSixStrings()
        {
            await GetTableColumnNamesShouldBeSixStringsCore().ConfigureAwait(false);
        }

        [Fact]
        public async Task GetEntityObjectNameShouldBeTeams()
        {
            await GetEntityObjectNameShouldBeTeamsCore().ConfigureAwait(false);
        }

        #endregion
    }
}
