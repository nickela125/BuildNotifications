using BuildNotifications.Model;

namespace BuildNotifications.Helpers
{
    public static class VsoAddressHelper
    {
        public static string GetFullAddress(string accountName, string path)
        {
            string baseAddress = string.Format(Constants.VsoBaseAddress, accountName);
            baseAddress = baseAddress.TrimEnd('/');
            path = path.TrimStart('/');
            return baseAddress + "/" + path;
        }
    }
}
