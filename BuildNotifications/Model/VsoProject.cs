using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BuildNotifications.Model
{
    public class VsoProject : TreeItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<VsoBuild> Builds { get; set; }
        [JsonIgnore]
        public new IList<VsoBuild> Children => Builds;
        public new bool IsSelected { get; set; }
        [JsonIgnore]
        public new string DisplayName => Name;
    }
}
