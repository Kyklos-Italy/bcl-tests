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

    [Collection("NoParallelization"), Order(20)]
    public class AuthLockTests
    {
        [Fact, Order(10)]
        public async Task UserLocksAfterTooManyNonSequentialFailedAttempts()
        {
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

        [Fact, Order(20)]
        public async Task UserLocksAfterTooManyAuthAttempts()
        {
            await CommonUtils.LockUser(NeverExpiresUser.Username, NeverExpiresUser.Domain, NeverExpiresUser.Application);
            var response = await CommonUtils.AuthenticateUser(NeverExpiresUser.Username,
                                                              NeverExpiresUser.Password,
                                                              NeverExpiresUser.Domain,
                                                              NeverExpiresUser.Application);
            Assert.False(response.IsAuthenticated);
            Assert.Equal("KS-E108", response.ResponseCode);
            await CommonUtils.WaitLockTimeout(response.CustomDataJson);
        }

        [Fact, Order(30)]
        public async Task CorrectCredentialsAfterUserLockTimeoutExpiresSucceeds()
        {
            await CommonUtils.LockUser(NeverExpiresUser.Username, NeverExpiresUser.Domain, NeverExpiresUser.Application);
            var response = await CommonUtils.AuthenticateUser(NeverExpiresUser.Username,
                                                              NeverExpiresUser.Password,
                                                              NeverExpiresUser.Domain,
                                                              NeverExpiresUser.Application);

            Assert.False(response.IsAuthenticated);
            Assert.Equal("KS-E108", response.ResponseCode);
            await CommonUtils.WaitLockTimeout(response.CustomDataJson);
            response = await CommonUtils.AuthenticateUser(NeverExpiresUser.Username,
                                                          NeverExpiresUser.Password,
                                                          NeverExpiresUser.Domain,
                                                          NeverExpiresUser.Application);
            Assert.True(response.IsAuthenticated, "Could not authenticate " + response.ResponseMessage);
        }

    }
}
