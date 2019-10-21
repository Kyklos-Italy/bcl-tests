using System.Threading.Tasks;
using Xunit;
using Flurl.Http;
using KMicro.Auth.Models.Rest.User;

namespace KMicro.Auth.Tests.ChangePassword
{
    public class ChangePasswordTests
    {
        // TODO duplicated fields. Should be put in a base class or in a dedicated static class
        private static int Port { get; } = 27685;
        private static string ServerIp { get; } = "172.16.100.32";

        public static string AuthenticateUserUrl { get; } = $"http://{ServerIp}:{Port}/api/user/v1/authenticate";
        public static string ChangePasswordUserUrl { get; } = $"http://{ServerIp}:{Port}/api/user/v1/changePassword";

        private static string CriteriaCompliantPassword = "Fu!fa81";


        private async Task<string> AuthenticateUser(string username, string password, string domain, string app)
        {
            AuthenticationRequest authRequest = AuthenticationRequest.FromUsernamePasswordDomainAndApp(username, password, domain, app);
            AuthenticationResponse authResponse = await AuthenticateUserUrl.PostJsonAsync(authRequest).ReceiveJson<AuthenticationResponse>();
            Assert.True(authResponse.IsAuthenticated, "Authentication failed. Could not proceed to change password");
            return authResponse.Jwt;
        }


        [Fact]
        public async void CriteriaCompliantPasswordSucceeds()
        {
            // TODO remove magic strings
            string jwt = await AuthenticateUser("pippo", "Maialino818!", "KyklosTest", "QualityX");

            ChangePasswordRequest changePasswordRequest = ChangePasswordRequest.New("pippo", "KyklosTest", "QualityX", jwt, CriteriaCompliantPassword);
            ChangePasswordResponse changePasswordResponse = await ChangePasswordUserUrl.PostJsonAsync(changePasswordRequest).ReceiveJson<ChangePasswordResponse>();
            Assert.True(changePasswordResponse.Succeded);
        }

        [Fact]
        public async void TooShortPasswordFails()
        {
            string jwt = await AuthenticateUser("pippo", "Maialino818!", "KyklosTest", "QualityX");

            ChangePasswordRequest changePasswordRequest = ChangePasswordRequest.New("user01", "kyklos", "qx", jwt, "F4f!");
            ChangePasswordResponse changePasswordResponse = await ChangePasswordUserUrl.PostJsonAsync(changePasswordRequest).ReceiveJson<ChangePasswordResponse>();
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async void PasswordContainingUserFails()
        {
            string jwt = await AuthenticateUser("pippo", "Maialino818!", "KyklosTest", "QualityX");

            ChangePasswordRequest changePasswordRequest = ChangePasswordRequest.New("user01", "kyklos", "qx", jwt, CriteriaCompliantPassword + "pippo");
            ChangePasswordResponse changePasswordResponse = await ChangePasswordUserUrl.PostJsonAsync(changePasswordRequest).ReceiveJson<ChangePasswordResponse>();
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async void InvalidPatternPasswordFails()
        {
            string jwt = await AuthenticateUser("pippo", "Maialino818!", "KyklosTest", "QualityX");

            ChangePasswordRequest changePasswordRequest = ChangePasswordRequest.New("user01", "MonclerTest", "qx", jwt, "Moncler?81");
            ChangePasswordResponse changePasswordResponse = await ChangePasswordUserUrl.PostJsonAsync(changePasswordRequest).ReceiveJson<ChangePasswordResponse>();
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async void NoNumericCharacterPasswordFails()
        {
            string jwt = await AuthenticateUser("pippo", "Maialino818!", "KyklosTest", "QualityX");

            ChangePasswordRequest changePasswordRequest = ChangePasswordRequest.New("user01", "MonclerTest", "qx", jwt, "KIKKs?");
            ChangePasswordResponse changePasswordResponse = await ChangePasswordUserUrl.PostJsonAsync(changePasswordRequest).ReceiveJson<ChangePasswordResponse>();
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async void NoLowercaseCharacterPasswordFails()
        {
            string jwt = await AuthenticateUser("pippo", "Maialino818!", "KyklosTest", "QualityX");

            ChangePasswordRequest changePasswordRequest = ChangePasswordRequest.New("user01", "MonclerTest", "qx", jwt, "KIKK4?");
            ChangePasswordResponse changePasswordResponse = await ChangePasswordUserUrl.PostJsonAsync(changePasswordRequest).ReceiveJson<ChangePasswordResponse>();
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async void NoSpecialCharacterPasswordFails()
        {
            string jwt = await AuthenticateUser("pippo", "Maialino818!", "KyklosTest", "QualityX");

            ChangePasswordRequest changePasswordRequest = ChangePasswordRequest.New("user01", "MonclerTest", "qx", jwt, "KIKKa5");
            ChangePasswordResponse changePasswordResponse = await ChangePasswordUserUrl.PostJsonAsync(changePasswordRequest).ReceiveJson<ChangePasswordResponse>();
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async void TooManyConsecutiveEqualCharactersPasswordFails()
        {
            string jwt = await AuthenticateUser("pippo", "Maialino818!", "KyklosTest", "QualityX");

            ChangePasswordRequest changePasswordRequest = ChangePasswordRequest.New("user01", "MonclerTest", "qx", jwt, "KkKKka5?");
            ChangePasswordResponse changePasswordResponse = await ChangePasswordUserUrl.PostJsonAsync(changePasswordRequest).ReceiveJson<ChangePasswordResponse>();
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async void TooLongPasswordFails()
        {
            string jwt = await AuthenticateUser("pippo", "Maialino818!", "KyklosTest", "QualityX");

            ChangePasswordRequest changePasswordRequest = ChangePasswordRequest.New("user01", "MonclerTest", "qx", jwt, "M41alinoMaialoso?");
            ChangePasswordResponse changePasswordResponse = await ChangePasswordUserUrl.PostJsonAsync(changePasswordRequest).ReceiveJson<ChangePasswordResponse>();
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async void TooManyNumricCharactersPasswordFails()
        {
            string jwt = await AuthenticateUser("pippo", "Maialino818!", "KyklosTest", "QualityX");

            ChangePasswordRequest changePasswordRequest = ChangePasswordRequest.New("user01", "MonclerTest", "qx", jwt, "M414l1n081?");
            ChangePasswordResponse changePasswordResponse = await ChangePasswordUserUrl.PostJsonAsync(changePasswordRequest).ReceiveJson<ChangePasswordResponse>();
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async void TooManySpecialCharactersPasswordFails()
        {
            string jwt = await AuthenticateUser("pippo", "Maialino818!", "KyklosTest", "QualityX");

            ChangePasswordRequest changePasswordRequest = ChangePasswordRequest.New("user01", "MonclerTest", "qx", jwt, "M?&%?!$$81?");
            ChangePasswordResponse changePasswordResponse = await ChangePasswordUserUrl.PostJsonAsync(changePasswordRequest).ReceiveJson<ChangePasswordResponse>();
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async void TooManyLowercaseCharactersPasswordFails()
        {
            string jwt = await AuthenticateUser("pippo", "Maialino818!", "KyklosTest", "QualityX");

            ChangePasswordRequest changePasswordRequest = ChangePasswordRequest.New("user01", "MonclerTest", "qx", jwt, "Maialino?73");
            ChangePasswordResponse changePasswordResponse = await ChangePasswordUserUrl.PostJsonAsync(changePasswordRequest).ReceiveJson<ChangePasswordResponse>();
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async void TooManyUppercaseCharactersPasswordFails()
        {
            string jwt = await AuthenticateUser("pippo", "Maialino818!", "KyklosTest", "QualityX");

            ChangePasswordRequest changePasswordRequest = ChangePasswordRequest.New("user01", "MonclerTest", "qx", jwt, "MAIALIno?73");
            ChangePasswordResponse changePasswordResponse = await ChangePasswordUserUrl.PostJsonAsync(changePasswordRequest).ReceiveJson<ChangePasswordResponse>();
            Assert.False(changePasswordResponse.Succeded);
        }

    }
}

