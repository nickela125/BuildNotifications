using System.Collections.Generic;
using BuildNotifications.Model.DTO;

namespace BuildNotifications.Model
{
    public class VsoSubscibedBuildList
    {
        public AccountDetails AccountDetails { get; set; }
        public IList<VsoBuildDefinition> BuildDefinitions { get; set; }
    }
}
