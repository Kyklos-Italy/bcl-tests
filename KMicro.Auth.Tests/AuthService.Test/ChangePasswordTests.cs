using Flurl.Http;
using KMicro.Auth.Models.Rest.User;
using KMicro.Auth.Tests.TestAPI;
using KMicro.Auth.Tests.TestUsers;
using KMicro.Auth.Tests.Utils;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions.Ordering;

namespace KMicro.Auth.Tests.ChangePassword
{
    [Order(10)]
    public class ChangePasswordTests
    {
        [Fact]
        public async Task TooShortPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverExpiresUser.Password,
                                                                                             "Ab1!",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.Equal("KS-E112", changePasswordResponse.ResponseCode);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task PasswordContainingUserFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverExpiresUser.Password,
                                                                                             "83579r!" + NeverExpiresUser.Username,
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.Equal("KS-E112", changePasswordResponse.ResponseCode);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task InvalidPatternPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverExpiresUser.Password,
                                                                                             "KYKlos19pippo!",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.Equal("KS-E112", changePasswordResponse.ResponseCode);
            Assert.Contains("PV-E021", changePasswordResponse.CustomDataJson);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task NoNumericCharacterPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverExpiresUser.Password,
                                                                                             "KIKKs?",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.Equal("KS-E112", changePasswordResponse.ResponseCode);
            Assert.Contains("PV-E010", changePasswordResponse.CustomDataJson);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task NoLowercaseCharacterPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverExpiresUser.Password,
                                                                                             "KIKK4?",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.Equal("KS-E112", changePasswordResponse.ResponseCode);
            Assert.Contains("PV-E020", changePasswordResponse.CustomDataJson);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task NoSpecialCharacterPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverExpiresUser.Password,
                                                                                             "KIKKa5",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.Equal("KS-E112", changePasswordResponse.ResponseCode);
            Assert.Contains("PV-E040", changePasswordResponse.CustomDataJson);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task TooManyConsecutiveEqualCharactersPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverExpiresUser.Password,
                                                                                             "aaaaaabA1?",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.Equal("KS-E112", changePasswordResponse.ResponseCode);
            Assert.Contains("PV-E050", changePasswordResponse.CustomDataJson);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task TooLongPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverExpiresUser.Password,
                                                                                             "1234567890ABCDEabcdefghi??!!!",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.Equal("KS-E112", changePasswordResponse.ResponseCode);
            Assert.Contains("PV-E071", changePasswordResponse.CustomDataJson);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async void TooManyNumricCharactersPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverExpiresUser.Password,
                                                                                             "1234567891011Aa!",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.Equal("KS-E112", changePasswordResponse.ResponseCode);
            Assert.Contains("PV-E011", changePasswordResponse.CustomDataJson);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task TooManySpecialCharactersPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverExpiresUser.Password,
                                                                                             "M?&%?!$$81?",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.Equal("KS-E112", changePasswordResponse.ResponseCode);
            Assert.Contains("PV-E041", changePasswordResponse.CustomDataJson);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task TooManyLowercaseCharactersPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverExpiresUser.Password,
                                                                                             "abcdefghijklmn?73",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.Equal("KS-E112", changePasswordResponse.ResponseCode);
            Assert.Contains("PV-E021", changePasswordResponse.CustomDataJson);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task TooManyUppercaseCharactersPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverExpiresUser.Password,
                                                                                             "MAIALIno?73",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.Equal("KS-E112", changePasswordResponse.ResponseCode);
            Assert.Contains("PV-E031", changePasswordResponse.CustomDataJson);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task ChangePasswordWithJwtAfterPasswordExpiredSucceds()
        {

            var authResponse = await CommonUtils.AuthenticateUser(PasswordExpiredUser.Username,
                                                                  PasswordExpiredUser.Password,
                                                                  PasswordExpiredUser.Domain,
                                                                  PasswordExpiredUser.Application);
            Assert.False(authResponse.IsAuthenticated);
            Assert.NotEqual(authResponse.Jwt, string.Empty);


            ChangePasswordRequest changePasswordRequest = ChangePasswordRequest.New(PasswordExpiredUser.Username,
                                                                                    PasswordExpiredUser.Domain,
                                                                                    PasswordExpiredUser.Application,
                                                                                    authResponse.Jwt,
                                                                                    "prceLLINO616!");

            ChangePasswordResponse changePasswordResponse = await APIs.ChangePasswordUserUrl.PostJsonAsync(changePasswordRequest).ReceiveJson<ChangePasswordResponse>();

            Assert.Equal("KS-U002", changePasswordResponse.ResponseCode);
            Assert.True(changePasswordResponse.Succeded, changePasswordResponse.ResponseMessage);
        }
    }
}

