using System.Collections.Generic;

namespace BuildNotifications.Model
{
    public class VsoProject
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<VsoBuild> Builds { get; set; }
    }
}
