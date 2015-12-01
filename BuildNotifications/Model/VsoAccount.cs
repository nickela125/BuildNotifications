using System.Collections.Generic;

namespace BuildNotifications.Model
{
    public class VsoAccount
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string EncodedCredentials { get; set; }
        public IList<VsoProject> Projects { get; set; }
    }
}
