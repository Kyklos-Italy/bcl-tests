using System.Threading.Tasks;
using TestMultipleLogin;
using Xunit;

namespace KMicro.Auth.Tests
{
    public class AuthCastelli
    {
        [Fact]
        public async Task TestAuth()
        {
            await AuthUsers.AuthUser("castelli");
        }
    }
}
