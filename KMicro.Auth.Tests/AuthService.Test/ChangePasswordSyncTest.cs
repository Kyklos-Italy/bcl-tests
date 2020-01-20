using Flurl.Http;
using KMicro.Auth.Models.Rest.Common;
using KMicro.Auth.Models.Rest.User;
using KMicro.Auth.Tests.TestAPI;
using KMicro.Auth.Tests.TestUsers;
using KMicro.Auth.Tests.Utils;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions.Ordering;

namespace KMicro.Auth.Tests
{
    [Collection("NoParallelization")]
    public class ChangePasswordSyncTest
    {
        [Fact, Order(140)]
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
                authResponse = await CommonUtils.AuthenticateUser(username, password, domain, application);
                password += randomNumberGenerator.Next(1, 9).ToString();

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
            try
            {
                ChangePasswordResponse lastChangePasswordResponse = await APIs.ChangePasswordUserUrl.PostJsonAsync(lastChangePasswordRequest).ReceiveJson<ChangePasswordResponse>();
                Assert.False(true, "Password Changed But Was Expected To Fail");
            }
            catch (FlurlHttpException exc)
            {
                var errorDetail = await exc.GetResponseJsonAsync<ProblemDetailResponse<ChangePasswordProblem>>();
                var changePwdErrorJson = Newtonsoft.Json.JsonConvert.SerializeObject(errorDetail, Newtonsoft.Json.Formatting.Indented);
                Assert.Equal("KS-E112", errorDetail.CustomProblem.ErrorCode);
                Assert.Contains("PV-E510", errorDetail.CustomProblem.CustomDataJson);
                Assert.False(errorDetail.CustomProblem.Succeded);
            }
        }

        [Fact, Order(150)]
        public async Task CriteriaCompliantPasswordSucceeds()
        {
            string resetDbResponse = await CommonUtils.ResetDbData();
             Assert.Equal(APIResponses.ResetDBOkResponse, resetDbResponse);

            string newPassword = "POR8088li717?";

            ChangePasswordResponse changePasswordResponse = await CommonUtils.ChangePassword(AllowOldPasswordsUser.Username,
                                                                                              AllowOldPasswordsUser.Password,
                                                                                              newPassword,
                                                                                              AllowOldPasswordsUser.Domain,
                                                                                              AllowOldPasswordsUser.Application);

            Assert.Equal("KS-U002", changePasswordResponse.ResponseCode);
            Assert.True(changePasswordResponse.Succeded, "Could not change password: " + changePasswordResponse.ResponseMessage + ", details: " + changePasswordResponse.CustomDataJson);
        }

        [Fact, Order(160)]
        public async Task ResetUser()
        {
            var resetResponse = await CommonUtils.ResetUser();

            Assert.Equal("KS-A006",resetResponse.ResponseCode);
        }

        [Fact, Order(170)]
        public async Task ResetDb()
        {
            string resetDbResponse = await CommonUtils.ResetDbData();

            Assert.Equal(APIResponses.ResetDBOkResponse, resetDbResponse);
        }
    }
}
