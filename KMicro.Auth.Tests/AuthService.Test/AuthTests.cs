using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using KMicro.Auth.Models.Rest.User;
using KMicro.Auth.Tests.TestUsers;
using KMicro.Auth.Tests.Utils;

namespace KMicro.Auth.Tests.Authenticate
{
    public class AuthTests
    {
        [Fact]
        public async Task ChangePasswordOnFirstAccessEnabledReturnsJwt()
        {
            // TODO we currently don't have a way to reset  first access flag.
            Assert.True(false);
            //AuthenticationRequest request = AuthenticationRequest.FromUsernamePasswordDomainAndApp(CorrectUsername, CorrectPassword, CorrectDomain, CorrectApplicationName);
            //AuthenticationResponse response = await AuthenticateUserUrl.PostJsonAsync(request).ReceiveJson<AuthenticationResponse>();
            //Assert.False(response.IsAuthenticated);
            //Assert.NotEqual(response.Jwt, string.Empty);


            //string jwt = await CommonUtils.AuthenticateUser(NeverExpiresUser.Username,
            //                                   NeverExpiresUser.Password,
            //                                   NeverExpiresUser.Domain,
            //                                   NeverExpiresUser.Application);
            //Assert.NotEqual(jwt, string.Empty);
        }

        [Fact]
        public async Task MultipleAsyncAuthsAllSucceed()
        {
            List<Task<AuthenticationResponse>> authRequestsAttemps = new List<Task<AuthenticationResponse>>();

            for (int i = 0; i < 10; i++)
                authRequestsAttemps.Add(CommonUtils.AuthenticateUser(NeverLocksUser.Username,
                                                                     NeverLocksUser.Password,
                                                                     NeverLocksUser.Domain,
                                                                     NeverLocksUser.Application));
            
            var responses = await Task.WhenAll(authRequestsAttemps);
            bool atLeastOneFailed = responses.Any(s => s.IsAuthenticated == false);
            Assert.False(atLeastOneFailed);
        }

        [Fact]
        public async Task AllCorrectCredentialsAuthSucceeds()
        {
            var authResponse = await CommonUtils.AuthenticateUser(NeverLocksUser.Username,
                                                                  NeverLocksUser.Password,
                                                                  NeverLocksUser.Domain,
                                                                  NeverLocksUser.Application);
            Assert.True(authResponse.IsAuthenticated, authResponse.ResponseMessage);
            Assert.NotEqual(authResponse.Jwt, string.Empty);
        }

        [Fact]
        public async Task AllIncorrectCredentialAuthFails()
        {
            var authResponse = await CommonUtils.AuthenticateUser(IncorrectData.Username,
                                                                  IncorrectData.Password,
                                                                  IncorrectData.Domain,
                                                                  IncorrectData.Application);
            Assert.False(authResponse.IsAuthenticated);
        }

        [Fact]
        public async Task IncorrectUsernameAuthFails()
        {
            var authResponse = await CommonUtils.AuthenticateUser(IncorrectData.Username,
                                                                  NeverLocksUser.Password,
                                                                  NeverLocksUser.Domain,
                                                                  NeverLocksUser.Application);
            Assert.Equal("KS-E101", authResponse.ResponseCode);
            Assert.False(authResponse.IsAuthenticated);
        }

        [Fact]
        public async Task IncorrectPasswordAuthFails()
        {
            var authResponse = await CommonUtils.AuthenticateUser(NeverLocksUser.Username,
                                                                  IncorrectData.Password,
                                                                  NeverLocksUser.Domain,
                                                                  NeverLocksUser.Application);
            Assert.Equal("KS-E102", authResponse.ResponseCode);
            Assert.False(authResponse.IsAuthenticated);
        }

        [Fact]
        public async Task IncorrectDomainAuthFails()
        {
            var authResponse = await CommonUtils.AuthenticateUser(NeverLocksUser.Username,
                                                      NeverLocksUser.Password,
                                                      IncorrectData.Domain,
                                                      NeverLocksUser.Application);

            Assert.Equal("KS-E103", authResponse.ResponseCode);
            Assert.False(authResponse.IsAuthenticated);
        }

        [Fact]
        public async Task IncorrectAppAuthFails()
        {
            var authResponse = await CommonUtils.AuthenticateUser(NeverLocksUser.Username,
                                                                  NeverLocksUser.Password,
                                                                  NeverLocksUser.Domain,
                                                                  IncorrectData.Application);
            Assert.False(authResponse.IsAuthenticated);
            Assert.Equal("KS-E104", authResponse.ResponseCode);
        }

        [Fact]
        public async Task UserLocksAfterTooManyAuthAttempts()
        {
            await CommonUtils.LockUser(NeverExpiresUser.Username, NeverExpiresUser.Domain, NeverExpiresUser.Application);
            var response = await CommonUtils.AuthenticateUser(NeverExpiresUser.Username, 
                                                              NeverExpiresUser.Password, 
                                                              NeverExpiresUser.Domain, 
                                                              NeverExpiresUser.Application);
            Assert.False(response.IsAuthenticated);           
            Assert.Equal("KS-E108", response.ResponseCode);
        }

        [Fact]
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

        [Fact]
        public async Task CorrectCredentialsAfterUserLockTimeoutExpiresSucceeds()
        {
            await CommonUtils.LockUser(NeverExpiresUser.Username, NeverExpiresUser.Domain, NeverExpiresUser.Application);
            await Task.Delay(100000);
            var response = await CommonUtils.AuthenticateUser(NeverExpiresUser.Username, 
                                                              NeverExpiresUser.Password,
                                                              NeverExpiresUser.Domain, 
                                                              NeverExpiresUser.Application);
            Assert.True(response.IsAuthenticated);
        }

    }

}
