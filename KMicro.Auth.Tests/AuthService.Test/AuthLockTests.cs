using KMicro.Auth.Tests.TestAPI;
using KMicro.Auth.Tests.TestUsers;
using KMicro.Auth.Tests.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions.Ordering;

namespace KMicro.Auth.Tests.LockTests
{
    [CollectionDefinition("NoParallelization", DisableParallelization = true)]
    public class NoParallelizationTests { }


    [Collection("NoParallelization"), Order(30)]
    public class AuthLockTests
    {
        [Fact]
        public async Task UserLocksAfterTooManyAuthAttempts()
        {
            //await Task.Delay(500).ConfigureAwait(false);
            await CommonUtils.LockUser(NeverExpiresUser.Username, NeverExpiresUser.Domain, NeverExpiresUser.Application);
            var response = await CommonUtils.AuthenticateUser(NeverExpiresUser.Username,
                                                              NeverExpiresUser.Password,
                                                              NeverExpiresUser.Domain,
                                                              NeverExpiresUser.Application);
            Assert.False(response.IsAuthenticated);
            Assert.Equal("KS-E108", response.ResponseCode);
            string resetDbResponse = await CommonUtils.ResetDbData();
            Assert.Equal(APIResponses.ResetDBOkResponse, resetDbResponse);
        }

        [Fact]
        public async Task UserLocksAfterTooManyNonSequentialFailedAttempts()
        {
            //await Task.Delay(500).ConfigureAwait(false);
            List<Task> tasksToRun = new List<Task>();

            tasksToRun.Add(CommonUtils.DoWrongAuthenticationAttempt(NeverExpiresUser.Username, NeverExpiresUser.Domain, NeverExpiresUser.Application));
            await Task.WhenAll(tasksToRun);
            var response = await CommonUtils.AuthenticateUser(NeverExpiresUser.Username, NeverExpiresUser.Password, NeverExpiresUser.Domain, NeverExpiresUser.Application);
            Assert.True(response.IsAuthenticated, "Could not authenticate " + response.ResponseMessage);
            tasksToRun.Clear();

            for (int i = 0; i < 10; i++)
                tasksToRun.Add(CommonUtils.DoWrongAuthenticationAttempt(NeverExpiresUser.Username, NeverExpiresUser.Domain, NeverExpiresUser.Application));

            await Task.WhenAll(tasksToRun);
            response = await CommonUtils.AuthenticateUser(NeverExpiresUser.Username, NeverExpiresUser.Password, NeverExpiresUser.Domain, NeverExpiresUser.Application);
            Assert.False(response.IsAuthenticated, "Should be locked after many failed authentications");
            Assert.Equal("KS-E108", response.ResponseCode);
        }
    }
}
