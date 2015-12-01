using BuildNotifications.Model;

namespace BuildNotifications.Helpers
{
    public static class VsoAddressHelper
    {
        public static string GetFullAddress(VsoAccount account, string path)
        {
            string baseAddress = string.Format(Constants.VsoBaseAddress, account.Name);
            baseAddress = baseAddress.TrimEnd('/');
            path = path.TrimStart('/');
            return baseAddress + "/" + path;
        }
    }
}
