using System;
using System.Threading;
using System.Threading.Tasks;
using Kyklos.Kernel.Core.Exceptions;
using Kyklos.Kernel.Data.Async;
using Kyklos.Kernel.Data.Async.Support;
using Xunit;

namespace Kyklos.Kernel.Data.MariaDB.Test.NetFramework
{
    public class MariaDBDatasyncTestQuery3 : MariaDBDatasyncTestCommon
    {
        public MariaDBDatasyncTestQuery3()
        {            
        }

        #region Collection 3

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
        public void IgnoreDaoEscapeShouldBeBackTick()
        {
            IAsyncDao myDao = AsyncDaoFactory.CreateAsyncDao(ConnectionString, ProviderName, Schema, ignoreEscape: true);
            bool actualBool = ContainsEscapeShouldBeCore('`', myDao, true);
            Assert.False(actualBool);
        }

        [Fact]
        public void NotIgnoreDaoEscapeShouldBeBackTick()
        {
            IAsyncDao myDao = AsyncDaoFactory.CreateAsyncDao(ConnectionString, ProviderName, Schema, ignoreEscape: true);
            bool actualBool = ContainsEscapeShouldBeCore('`', myDao, false);
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
            await CountAllResultsShouldBeN(6).ConfigureAwait(false);
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

        #endregion

    }
}
