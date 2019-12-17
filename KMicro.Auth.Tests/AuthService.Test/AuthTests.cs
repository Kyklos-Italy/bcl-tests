using KMicro.Auth.Models.Rest.User;
using KMicro.Auth.Tests.TestUsers;
using KMicro.Auth.Tests.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.Ordering;

namespace KMicro.Auth.Tests.Authenticate
{
    [Order(10)]
    public class AuthTests
    {
        private readonly ITestOutputHelper output;
        public AuthTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact, Order(10)]
        public async Task MultipleCorrectAsyncAuthsAllSucceed()
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
            Assert.Equal("KS-U001", authResponse.ResponseCode);
            Assert.True(authResponse.IsAuthenticated, $"{authResponse.ResponseCode}:{authResponse.ResponseMessage}");
            Assert.NotEqual(authResponse.Jwt, string.Empty);
            output.WriteLine($"{authResponse.ResponseCode}:{authResponse.ResponseMessage}");
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

            Assert.Equal("KS-E101", authResponse.ResponseCode);
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
            Assert.Equal("KS-E101", authResponse.ResponseCode);
        }

        [Fact]
        public async Task MultipleAsyncAuthenticationsAreProcessedCorrectly()
        {
            await Task.Delay(50).ConfigureAwait(false);
            List<Task<AuthenticationResponse>> correctAuthentications = new List<Task<AuthenticationResponse>>();
            List<Task<AuthenticationResponse>> wrongAuthentications = new List<Task<AuthenticationResponse>>();

            for (int i = 0; i < 10; i++)
            {
                correctAuthentications.Add(CommonUtils.AuthenticateUser(NeverLocksUser.Username,
                                                                        NeverLocksUser.Password,
                                                                        NeverLocksUser.Domain,
                                                                        NeverLocksUser.Application));

                wrongAuthentications.Add(CommonUtils.AuthenticateUser(IncorrectData.Username,
                                                                      IncorrectData.Password,
                                                                      IncorrectData.Domain,
                                                                      IncorrectData.Application));
            }

            var correctResponses = await Task.WhenAll(correctAuthentications);
            var wrongResponses = await Task.WhenAll(wrongAuthentications);

            Assert.True(correctResponses.All(a => a.IsAuthenticated == true));
            Assert.True(wrongResponses.All(a => a.IsAuthenticated == false));
        }
    }
}

