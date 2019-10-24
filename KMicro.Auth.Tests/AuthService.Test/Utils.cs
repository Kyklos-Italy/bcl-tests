using System.Threading.Tasks;
using Flurl.Http;
using KMicro.Auth.Models.Rest.User;
using KMicro.Auth.Tests.TestAPI;

namespace KMicro.Auth.Tests.Utils
{
    public class CommonUtils
    {
        public static async Task<AuthenticationResponse> AuthenticateUser(string username, string password, string domain, string app)
        {
            AuthenticationRequest authRequest = AuthenticationRequest.FromUsernamePasswordDomainAndApp(username, password, domain, app);
            AuthenticationResponse authResponse = await APIs.AuthenticateUserUrl.PostJsonAsync(authRequest).ReceiveJson<AuthenticationResponse>();
            return authResponse;
        }
    }
}
