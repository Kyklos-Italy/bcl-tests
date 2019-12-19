using Flurl.Http;
using KMicro.Auth.Models.Rest.User;
using KMicro.Auth.Tests.TestAPI;
using KMicro.Auth.Tests.TestUsers;
using Kyklos.Kernel.TimeSupport;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace KMicro.Auth.Tests.Utils
{
    public class CommonUtils
    {
        private static void CheckResponse(AuthenticationResponse response)
        {
            if (!response.IsAuthenticated)
            {
                if (response.ResponseCode == "KS-E500")
                {
                    XunitContext.WriteLine("SERVER ERROR");
                }
                XunitContext.WriteLine($"{response.ResponseCode}:{response.ResponseMessage}");
                string logs = string.Concat(XunitContext.Logs.Select(x => x));
                Assert.True(response.IsAuthenticated, logs);
            }
        }

        public static async Task<AuthenticationResponse> AuthenticateUser(string username, string password, string domain, string app)
        {
            AuthenticationRequest authRequest = AuthenticationRequest.FromUsernamePasswordDomainAndApp(username, password, domain, app);
            try
            {
                AuthenticationResponse authResponse = await APIs.AuthenticateUserUrl.PostJsonAsync(authRequest).ReceiveJson<AuthenticationResponse>();
                var authJson = Newtonsoft.Json.JsonConvert.SerializeObject(authResponse, Newtonsoft.Json.Formatting.Indented);
                return authResponse;
            }
            catch (FlurlHttpException exc)
            {
                var errorDetail = await exc.GetResponseJsonAsync<ProblemDetailResponse<AuthenticationProblem>>();
                var authErrorJson = Newtonsoft.Json.JsonConvert.SerializeObject(errorDetail, Newtonsoft.Json.Formatting.Indented);
                AuthenticationResponse errorResponse =
                    new AuthenticationResponse
                    (
                       responseId: errorDetail.ProblemDetailId,
                       correlationId: errorDetail.CorrelationId,
                       isAuthenticated: errorDetail.CustomProblem.IsAuthenticated,
                       jwt: errorDetail.CustomProblem.Jwt,
                       responseCode: errorDetail.CustomProblem.ErrorCode,
                       responseMessage: errorDetail.Detail,
                       customDataJson: errorDetail.CustomProblem.CustomDataJson
                    );
                return errorResponse;
            }
        }

        public static async Task<ChangePasswordResponse> ChangePassword(string user, string oldPassword, string newPassword, string domain, string app)
        {
            var authResponse = await CommonUtils.AuthenticateUser(user, oldPassword, domain, app);
            CheckResponse(authResponse);
            ChangePasswordRequest changePasswordRequest = ChangePasswordRequest.New(user,
                                                                                    domain,
                                                                                    app,
                                                                                    authResponse.Jwt,
                                                                                    newPassword);
            try
            {
                ChangePasswordResponse changePasswordResponse = await APIs.ChangePasswordUserUrl.PostJsonAsync(changePasswordRequest).ReceiveJson<ChangePasswordResponse>();
                var changePwdJson = Newtonsoft.Json.JsonConvert.SerializeObject(changePasswordResponse, Newtonsoft.Json.Formatting.Indented);
                return changePasswordResponse;
            }
            catch (FlurlHttpException exc)
            {
                var errorDetail = await exc.GetResponseJsonAsync<ProblemDetailResponse<ChangePasswordProblem>>();
                var changePwdErrorJson = Newtonsoft.Json.JsonConvert.SerializeObject(errorDetail, Newtonsoft.Json.Formatting.Indented);
                ChangePasswordResponse errorResponse =
                    new ChangePasswordResponse
                    (
                        responseId: errorDetail.ProblemDetailId,
                        correlationId: errorDetail.CorrelationId,
                        succeded: errorDetail.CustomProblem.Succeded,
                        responseCode: errorDetail.CustomProblem.ErrorCode,
                        responseMessage: errorDetail.Detail,
                        customDataJson: errorDetail.CustomProblem.CustomDataJson
                    );
                return errorResponse;
            }

        }

        public static async Task WaitLockTimeout(string json)
        {
            JObject customData = JObject.Parse(json);
            var lockPeriod = customData["LockPeriod"].Value<string>();
            var period = Period.Parse(lockPeriod);
            await Task.Delay(period.ToDuration().Add(1.Seconds()));
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

        public static async Task<string> ResetDbData()
        {
            return await $"{APIs.AdminUrl}restoredata".WithHeader("AUTH-X-API-KEY", _GetAPIKey()).PutJsonAsync("nocare").ReceiveString();
        }

        private static string _GetAPIKey()
        {
            string APIkeyFilename = "api_key.txt";

            try
            {
                return File.ReadAllText(APIkeyFilename);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Could not read admin APIs key from {APIkeyFilename}: {exception.Message}");
                return string.Empty;
            }
        }
    }
}
