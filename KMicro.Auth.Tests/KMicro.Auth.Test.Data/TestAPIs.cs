
namespace KMicro.Auth.Tests.TestAPI
{
    public class APIs
    {
        private static int Port { get; } = 27685;
        //private static string ServerIp { get; } = "grendizer"; /*"172.16.100.32";*/
        private static string ServerIp { get; } = "localhost";

        public static string AdminUrl { get; } = $"http://{ServerIp}:{Port}/api/admin/v1/";
        public static string UserUrl { get; } = $"http://{ServerIp}:{Port}/api/user/v1/";

        public static string AuthenticateUserUrl { get; } = $"{UserUrl}{RestMethodNames.User.Authenticate}";
        public static string ChangePasswordUserUrl { get; } = $"{UserUrl}{RestMethodNames.User.ChangePassword}";

        public static string AdminResetUserUrl { get; } = $"{AdminUrl}{RestMethodNames.Admin.ResetUser}";
        public static string AdminCreateOrUpdateUserUrl { get; } = $"{AdminUrl}{RestMethodNames.Admin.CreateOrUpdateUser}";
        public static string AdminDeleteUserUrl { get; } = $"{AdminUrl}{RestMethodNames.Admin.DeleteUser}";
    }

    public class APIResponses
    {
        public static string ResetDBOkResponse = "Ok";
    }
}
