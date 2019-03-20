using System.Threading.Tasks;
using Xunit;

namespace Kyklos.Kernel.Data.MariaDB.Test.NetFramework
{
    public class MariaDBDatasyncTestQuery4 : MariaDBDatasyncTestCommon
    {
        public MariaDBDatasyncTestQuery4()
        {            
        }

        #region Collection 4

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

        #endregion

    }
}
