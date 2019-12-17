using KMicro.Auth.Tests.TestUsers;
using KMicro.Auth.Tests.Utils;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace KMicro.Auth.Tests
{
    public class AuthCasagrande
    {
        private readonly ITestOutputHelper output;
        public AuthCasagrande(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task AuthUser()
        {
            var authResponse = await CommonUtils.AuthenticateUser(MultipleUsers.users["casagrande"].Username,
                                                                  MultipleUsers.users["casagrande"].Password,
                                                                  MultipleUsers.users["casagrande"].Domain,
                                                                  MultipleUsers.users["casagrande"].Application);
            Assert.Equal("KS-U001", authResponse.ResponseCode);
            Assert.True(authResponse.IsAuthenticated, $"{authResponse.ResponseCode}:{authResponse.ResponseMessage}");
            Assert.NotEqual(authResponse.Jwt, string.Empty);
            output.WriteLine($"{authResponse.ResponseCode}:{authResponse.ResponseMessage}");
        }
    }
}
