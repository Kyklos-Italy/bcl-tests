using System.Threading.Tasks;
using TestMultipleLogin;
using Xunit;

namespace KMicro.Auth.Tests
{
    public class AuthBarbiero
    {
        [Fact]
        public async Task TestAuth()
        {
            await AuthUsers.AuthUser("barbiero");
        }
    }
}
