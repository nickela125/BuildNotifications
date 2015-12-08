using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BuildNotifications.Model
{
    public class VsoAccount : TreeItem
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string EncodedCredentials { get; set; }
        public IList<VsoProject> Projects { get; set; }

        [JsonIgnore]
        public new IList<VsoProject> Children => Projects;
        public new bool IsSelected { get; set; }
        [JsonIgnore]
        public new string DisplayName => Name;
    }
}
