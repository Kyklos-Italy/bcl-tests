using KMicro.Auth.Tests.TestUsers;
using KMicro.Auth.Tests.Utils;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace KMicro.Auth.Tests
{
    public class AuthCamelot
    {
        private readonly ITestOutputHelper output;
        public AuthCamelot(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task AuthUser()
        {
            var authResponse = await CommonUtils.AuthenticateUser(MultipleUsers.users["camelot"].Username,
                                                                  MultipleUsers.users["camelot"].Password,
                                                                  MultipleUsers.users["camelot"].Domain,
                                                                  MultipleUsers.users["camelot"].Application);
            Assert.Equal("KS-U001", authResponse.ResponseCode);
            Assert.True(authResponse.IsAuthenticated, $"{authResponse.ResponseCode}:{authResponse.ResponseMessage}");
            Assert.NotEqual(authResponse.Jwt, string.Empty);
            output.WriteLine($"{authResponse.ResponseCode}:{authResponse.ResponseMessage}");
        }
    }
}
