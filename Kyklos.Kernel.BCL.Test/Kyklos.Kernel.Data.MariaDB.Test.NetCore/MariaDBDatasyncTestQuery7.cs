﻿using Kyklos.Kernel.Data.Test.Entities;
using System.Threading.Tasks;
using Xunit;

namespace Kyklos.Kernel.Data.MariaDB.Test.NetCore
{
    public class MariaDBDatasyncTestQuery7 : MariaDBDatasyncTestCommon
    {
        public MariaDBDatasyncTestQuery7()
        {            
        }

        #region Collection 7

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
        public async Task CountWithNestedQueryShouldBeTheSame()
        {
            await CountWithNestedQueryShouldBeTheSameCore();
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

        #endregion
    }
}
