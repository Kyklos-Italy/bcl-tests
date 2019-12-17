
namespace KMicro.Auth.Tests.TestAPI
{
    public class APIs
    {
        private static int Port { get; } = 27685;
        private static string ServerIp { get; } = /*"grendizer";*/ "172.16.100.32";

        public static string AdminUrl { get; } = $"http://{ServerIp}:{Port}/api/admin/v1/";
        public static string AuthenticateUserUrl { get; } = $"http://{ServerIp}:{Port}/api/user/v1/authenticate";
        public static string ChangePasswordUserUrl { get; } = $"http://{ServerIp}:{Port}/api/user/v1/changePassword";
    }

    public class APIResponses
    {
        public static string ResetDBOkResponse = "Ok";
    }
}
