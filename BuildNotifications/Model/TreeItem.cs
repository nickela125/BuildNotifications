using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace BuildNotifications.Model
{
    public class TreeItem : ViewModelBase
    {
        [JsonIgnore]
        public IList<TreeItem> Children { get; set; }
        [JsonIgnore]
        public TreeItem Parent { get; set; }

        public virtual bool? IsSelected { get; set; }
        [JsonIgnore]
        public bool DisplayMenu { get; set; }
    }
}
