using Flurl.Http;
using KMicro.Auth.Models.Rest.Common;
using KMicro.Auth.Models.Rest.Admin;
using KMicro.Auth.Models.Rest.User;
using KMicro.Auth.Tests.TestAPI;
using KMicro.Auth.Tests.TestUsers;
using Kyklos.Kernel.IO.Async;
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

        public static async Task<ResetUserResponse> ResetUser()
        {
            AdminUser[] adminUsers = await ReadAdminUsers();
            ResetUserRequest resetRequest = new ResetUserRequest("1", adminUsers[0].Username, adminUsers[0].Password, "pan-ko", "Moncler", "scmx", "Password01", "");
            try
            {
                ResetUserResponse resetUserResponse = await $"{APIs.AdminResetUserUrl}".WithHeader("AUTH-X-API-KEY", await _GetAPIKey()).PostJsonAsync(resetRequest).ReceiveJson<ResetUserResponse>();
                var authJson = Newtonsoft.Json.JsonConvert.SerializeObject(resetUserResponse, Newtonsoft.Json.Formatting.Indented);
                return resetUserResponse;
            }
            catch (FlurlHttpException exc)
            {
                var errorDetail = await exc.GetResponseJsonAsync<ProblemDetailResponse<AuthenticationProblem>>();
                var authErrorJson = Newtonsoft.Json.JsonConvert.SerializeObject(errorDetail, Newtonsoft.Json.Formatting.Indented);
                ResetUserResponse errorResponse =
                    new ResetUserResponse
                    (
                        responseId: errorDetail.ProblemDetailId,
                        correlationId: errorDetail.CorrelationId,
                        succeded: false,
                        responseCode: errorDetail.CustomProblem.ErrorCode,
                        responseMessage: errorDetail.Detail,
                        customDataJson: errorDetail.CustomProblem.CustomDataJson
                    );
                return errorResponse;
            }
        }

        public static async Task<ChangePasswordResponse> ChangePassword(string user, string oldPassword, string newPassword, string domain, string app)
        {
            var authResponse = await AuthenticateUser(user, oldPassword, domain, app);
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
            return await $"{APIs.AdminUrl}restoredata".WithHeader("AUTH-X-API-KEY", await _GetAPIKey()).PutJsonAsync("nocare").ReceiveString();
        }

        private static async Task<string> _GetAPIKey()
        {
            string APIkeyFilename = "api_key.txt";

            try
            {
                return await KFile.ReadAllTextAsync(APIkeyFilename).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await Console.Out.WriteLineAsync($"Could not read admin APIs key from {APIkeyFilename}: {exception.Message}").ConfigureAwait(false);
                return string.Empty;
            }
        }

        private static async Task<AdminUser[]> ReadAdminUsers()
        {
            const string AdminCredentialsFilename = "admin_users.txt";
            try
            {
                return (await
                    KFile
                    .ReadAllLinesAsync(AdminCredentialsFilename)
                    .ConfigureAwait(false))
                    .Select
                    (
                        line =>
                        {
                            var parts = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            return new AdminUser(parts[0], parts[1]);
                        }
                    )
                    .ToArray();

            }
            catch (Exception exception)
            {
                await Console.Out.WriteLineAsync($"Could not read admin users from {AdminCredentialsFilename}: {exception.Message}").ConfigureAwait(false);
                return null;
            }
        }
    }
}
