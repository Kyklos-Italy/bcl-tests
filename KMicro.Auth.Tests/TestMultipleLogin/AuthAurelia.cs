using System.Threading.Tasks;
using TestMultipleLogin;
using Xunit;

namespace KMicro.Auth.Tests
{
    public class AuthAurelia
    {
        [Fact]
        public async Task TestAuth()
        {
            await AuthUsers.AuthUser("aurelia");
        }
    }
}
