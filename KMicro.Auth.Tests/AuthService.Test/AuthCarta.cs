using KMicro.Auth.Tests.TestUsers;
using System.Threading.Tasks;
using TestMultipleLogin;
using Xunit;

namespace KMicro.Auth.Tests.MultipleLogin
{
    public class AuthCarta
    {
        [Fact]
        public async Task TestAuth()
        {
            await AuthUsers.AuthUser(MultipleUsers.Carta);
        }
    }
}
