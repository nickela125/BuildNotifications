using System.Collections.Generic;
using Newtonsoft.Json;

namespace BuildNotifications.Model
{
    public class TreeItem
    {
        [JsonIgnore]
        public IList<TreeItem> Children { get; set; }
        public bool IsSelected { get; set; }
        [JsonIgnore]
        public string DisplayName { get; set; }
    }
}
