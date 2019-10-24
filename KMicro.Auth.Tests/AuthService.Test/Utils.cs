using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Flurl.Http;
using KMicro.Auth.Models.Rest.User;
using KMicro.Auth.Tests.TestAPI;
using KMicro.Auth.Tests.TestUsers;

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

        public static async Task<ChangePasswordResponse> ChangePassword(string user, string oldPassword, string newPassword, string domain, string app)
        {
            var authResponse = await CommonUtils.AuthenticateUser(user, oldPassword, domain, app);
            ChangePasswordRequest changePasswordRequest = ChangePasswordRequest.New(NeverExpiresUser.Username,
                                                                                    NeverExpiresUser.Domain,
                                                                                    NeverExpiresUser.Application,
                                                                                    authResponse.Jwt,
                                                                                    newPassword);

            ChangePasswordResponse changePasswordResponse = await APIs.ChangePasswordUserUrl.PostJsonAsync(changePasswordRequest).ReceiveJson<ChangePasswordResponse>();
            return changePasswordResponse;
        }

        public static async Task DoWrongAuthenticationAttempt(string user, string domain, string app)
        {
            var authResponse = await CommonUtils.AuthenticateUser(user, IncorrectData.Password, domain, app);
            Assert.False(authResponse.IsAuthenticated, "This authentication should be purposely incorrect");
        }

        public static async Task LockUser(string user, string domain, string app)
        {
            List<Task> tasksToRun = new List<Task>();

            for (int i = 0; i < 30; i++)
                tasksToRun.Add(CommonUtils.DoWrongAuthenticationAttempt(user, domain, app));

            await Task.WhenAll(tasksToRun);
        }
    }
}
