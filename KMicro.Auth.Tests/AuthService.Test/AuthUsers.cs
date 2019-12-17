using KMicro.Auth.Tests.TestUsers;
using KMicro.Auth.Tests.Utils;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace TestMultipleLogin
{
    public class AuthUsers
    {
        public static async Task AuthUser(string usr)
        {
            User user = MultipleUsers.Users[usr];
            var authResponse = await CommonUtils.AuthenticateUser(user.Username,
                                                                  user.Password,
                                                                  user.Domain,
                                                                  user.Application);
            Assert.Equal("KS-U001", authResponse.ResponseCode);
            Assert.True(authResponse.IsAuthenticated, $"{authResponse.ResponseCode}:{authResponse.ResponseMessage}");
            Assert.NotEqual(authResponse.Jwt, string.Empty);
            Trace.WriteLine($"{authResponse.ResponseCode}:{authResponse.ResponseMessage}");
        }
    }
}
