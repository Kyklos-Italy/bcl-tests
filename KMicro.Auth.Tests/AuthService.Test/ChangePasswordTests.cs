using System;
using System.Threading.Tasks;
using Xunit;
using Flurl.Http;
using KMicro.Auth.Models.Rest.User;
using KMicro.Auth.Tests.TestUsers;
using KMicro.Auth.Tests.Utils;
using KMicro.Auth.Tests.TestAPI;

namespace KMicro.Auth.Tests.ChangePassword
{
    public class ChangePasswordTests
    {
        [Fact]
        public async Task CriteriaCompliantPasswordSucceeds()
        {
            string newPassword = "POR8888li717?";

            ChangePasswordResponse changePasswordResponse =  await CommonUtils.ChangePassword(NeverExpiresUser.Username, 
                                                                                              NeverExpiresUser.Password,
                                                                                              newPassword, 
                                                                                              NeverExpiresUser.Domain,
                                                                                              NeverExpiresUser.Application);

            ChangePasswordResponse resetPasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                            newPassword,
                                                                                            NeverExpiresUser.Password,
                                                                                            NeverExpiresUser.Domain,                                   
                                                                                            NeverExpiresUser.Application);                                    
                                                                   
            Assert.True(resetPasswordResponse.Succeded, "[WARNING]Could not reset original password after test");
            Assert.True(changePasswordResponse.Succeded, "Could not change password: " + changePasswordResponse.ResponseMessage + ", details: " + changePasswordResponse.CustomDataJson);
        }

        [Fact]
        public async Task TooShortPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverLocksUser.Password,
                                                                                             "F4f!",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);           
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task PasswordContainingUserFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverLocksUser.Password,
                                                                                             "83579r!" + NeverExpiresUser.Username,
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task InvalidPatternPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverLocksUser.Password,
                                                                                             "KYKlos19pippo!",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task NoNumericCharacterPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverLocksUser.Password,
                                                                                             "KIKKs?",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task NoLowercaseCharacterPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverLocksUser.Password,
                                                                                             "KIKK4?",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task NoSpecialCharacterPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverLocksUser.Password,
                                                                                             "KIKKa5",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task TooManyConsecutiveEqualCharactersPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverLocksUser.Password,
                                                                                             "KkKKka5?",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task TooLongPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverLocksUser.Password,
                                                                                             "M41alinoMaialoso?",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async void TooManyNumricCharactersPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverLocksUser.Password,
                                                                                             "M414l1n0811?",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task TooManySpecialCharactersPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverLocksUser.Password,
                                                                                             "M?&%?!$$81?",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task TooManyLowercaseCharactersPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverLocksUser.Password,
                                                                                             "Maialino?73",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task TooManyUppercaseCharactersPasswordFails()
        {
            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(NeverExpiresUser.Username,
                                                                                             NeverLocksUser.Password,
                                                                                             "MAIALIno?73",
                                                                                             NeverExpiresUser.Domain,
                                                                                             NeverExpiresUser.Application);
            Assert.False(changePasswordResponse.Succeded);
        }

        [Fact]
        public async Task ChangePasswordWithJwtAfterPasswordExpiredSucceds()
        {
           
            var authResponse = await CommonUtils.AuthenticateUser(PasswordExpiredUser.Username,
                                                                  "prceLLINO616!", 
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

            Assert.True(changePasswordResponse.Succeded, changePasswordResponse.ResponseMessage);
        }

        [Fact]
        public async Task NewPasswordIsInLastNPasswordsFails()
        {
            string username = NeverExpiresUser.Username;
            string password = NeverExpiresUser.Password;
            string domain = NeverExpiresUser.Domain;
            string application = NeverExpiresUser.Application;
            AuthenticationResponse authResponse;
            var randomNumberGenerator = new Random();

            for (int i = 0; i < 4; i++)
            {
                authResponse =  await CommonUtils.AuthenticateUser(username, password, domain, application);
                password += randomNumberGenerator.Next(1, 99).ToString(); 
                         
                ChangePasswordRequest changePasswordRequest = ChangePasswordRequest.New(username, domain, application, authResponse.Jwt, password);
                ChangePasswordResponse changePasswordResponse = await APIs.ChangePasswordUserUrl.PostJsonAsync(changePasswordRequest).ReceiveJson<ChangePasswordResponse>();
                Assert.True(changePasswordResponse.Succeded);

            }

            authResponse = await CommonUtils.AuthenticateUser(username, password, domain, application);
            ChangePasswordRequest lastChangePasswordRequest = ChangePasswordRequest.New(username,
                                                                                        domain,
                                                                                        application,
                                                                                        authResponse.Jwt,
                                                                                        NeverExpiresUser.Password);

            ChangePasswordResponse lastChangePasswordResponse = await APIs.ChangePasswordUserUrl.PostJsonAsync(lastChangePasswordRequest).ReceiveJson<ChangePasswordResponse>();
            Assert.False(lastChangePasswordResponse.Succeded);

        }
    }
}

