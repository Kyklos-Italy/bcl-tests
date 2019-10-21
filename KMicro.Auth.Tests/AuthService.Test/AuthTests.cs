using Xunit;
using Flurl.Http;
using KMicro.Auth.Models.Rest.User;

namespace KMicro.Auth.Tests.Authenticate
{
    public class AuthTests
    {
        // TODO duplicated fields. Should be put in a base class or in a dedicated static class
        private static int Port { get; } = 27685;
        private static string ServerIp { get; } = "172.16.100.32";

        public static string AuthenticateUserUrl { get; } = $"http://{ServerIp}:{Port}/api/user/v1/authenticate";
        public static string AdminUrl { get; } = $"http://{ServerIp}:{Port}/api/admin/v1/";
        public static string ChangePasswordUserUrl { get; } = $"http://{ServerIp}:{Port}/api/user/v1/changePassword";

        private const string IncorrectUsername = "madeUpUser";
        private const string IncorrectPassword = "madeUpPassword";
        private const string IncorrectDomain = "madeUpDomain";
        private const string IncorrectApp = "madeUpApp";

        private const string CorrectUsername = "pippo";
        private const string CorrectPassword = "Maialino818!";
        private const string CorrectDomain = "KyklosTest";
        private const string CorrectApplicationName = "QualityX";

        [Fact]
        public async void AllCorrectCredentialsAuthSucceeds()
        {
            AuthenticationRequest request = AuthenticationRequest.FromUsernamePasswordDomainAndApp(CorrectUsername, CorrectPassword, CorrectDomain, CorrectApplicationName);
            AuthenticationResponse response = await AuthenticateUserUrl.PostJsonAsync(request).ReceiveJson<AuthenticationResponse>();
            Assert.True(response.IsAuthenticated);
        }

        [Fact]
        public async void AllIncorrectCredentialAuthFails()
        {
            AuthenticationRequest request = AuthenticationRequest.FromUsernamePasswordDomainAndApp(IncorrectUsername, IncorrectPassword, IncorrectDomain, IncorrectApp);
            AuthenticationResponse response = await AuthenticateUserUrl.PostJsonAsync(request).ReceiveJson<AuthenticationResponse>();
            Assert.False(response.IsAuthenticated);
        }

        [Fact]
        public async void IncorrectUsernameAuthFails()
        {
            AuthenticationRequest request = AuthenticationRequest.FromUsernamePasswordDomainAndApp(IncorrectUsername, CorrectPassword, CorrectDomain, CorrectApplicationName);
            AuthenticationResponse response = await AuthenticateUserUrl.PostJsonAsync(request).ReceiveJson<AuthenticationResponse>();
            Assert.False(response.IsAuthenticated);
        }

        [Fact]
        public async void IncorrectPasswordAuthFails()
        {
            AuthenticationRequest request = AuthenticationRequest.FromUsernamePasswordDomainAndApp(CorrectUsername, IncorrectPassword, CorrectDomain, CorrectApplicationName);
            AuthenticationResponse response = await AuthenticateUserUrl.PostJsonAsync(request).ReceiveJson<AuthenticationResponse>();
            Assert.False(response.IsAuthenticated);
        }

        [Fact]
        public async void IncorrectDomainAuthFails()
        {
            AuthenticationRequest request = AuthenticationRequest.FromUsernamePasswordDomainAndApp(CorrectUsername, CorrectPassword, IncorrectDomain, CorrectApplicationName);
            AuthenticationResponse response = await AuthenticateUserUrl.PostJsonAsync(request).ReceiveJson<AuthenticationResponse>();
            Assert.False(response.IsAuthenticated);
        }

        [Fact]
        public async void IncorrectAppAuthFails()
        {
            AuthenticationRequest request = AuthenticationRequest.FromUsernamePasswordDomainAndApp(CorrectUsername, CorrectPassword, CorrectDomain, IncorrectApp);
            AuthenticationResponse response = await AuthenticateUserUrl.PostJsonAsync(request).ReceiveJson<AuthenticationResponse>();
            Assert.False(response.IsAuthenticated);
        }

        // TODO we should update tets db with an approproiate user to test this case
        //[Fact]
        //public async void DifferentDomainCorrectCredentialsAuthFails()
        //{
        //    AuthenticationRequest request = AuthenticationRequest.FromUsernamePasswordDomainAndApp("pippo", "Maialino818!", "KyklosTest", );
        //    AuthenticationResponse response = await AuthenticateUserUrl.PostJsonAsync(request).ReceiveJson<AuthenticationResponse>();
        //    Assert.False(response.IsAuthenticated);
        //}

        [Fact]
        public async void DifferentAppCorrectCredentialsAuthFails()
        {
            AuthenticationRequest request = AuthenticationRequest.FromUsernamePasswordDomainAndApp("pippo", "Maialino818!", "KyklosTest", IncorrectApp);
            AuthenticationResponse response = await AuthenticateUserUrl.PostJsonAsync(request).ReceiveJson<AuthenticationResponse>();
            Assert.False(response.IsAuthenticated);
        }

    }
}
