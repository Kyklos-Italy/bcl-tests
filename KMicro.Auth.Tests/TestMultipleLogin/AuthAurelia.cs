using KMicro.Auth.Tests.TestUsers;
using KMicro.Auth.Tests.Utils;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace KMicro.Auth.Tests
{
    public class AuthAurelia
    {
        private readonly ITestOutputHelper output;
        public AuthAurelia(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task AuthUser()
        {
            var authResponse = await CommonUtils.AuthenticateUser(MultipleUsers.users["aurelia"].Username,
                                                                  MultipleUsers.users["aurelia"].Password,
                                                                  MultipleUsers.users["aurelia"].Domain,
                                                                  MultipleUsers.users["aurelia"].Application);
            Assert.Equal("KS-U001", authResponse.ResponseCode);
            Assert.True(authResponse.IsAuthenticated, $"{authResponse.ResponseCode}:{authResponse.ResponseMessage}");
            Assert.NotEqual(authResponse.Jwt, string.Empty);
            output.WriteLine($"{authResponse.ResponseCode}:{authResponse.ResponseMessage}");
        }
    }
}
