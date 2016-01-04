using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Helpers;
using Newtonsoft.Json;

namespace BuildNotifications.Model
{
    public class VsoBuildDefinition : TreeItem
    {
        public VsoBuildDefinition()
        {
            DisplayMenu = false;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public new string DisplayName => Name;
        [JsonIgnore]
        public new VsoProject Parent { get; set; }
        public BuildStatus? CurrentBuildStatus { get; set; }
        public string CurrentBuildId { get; set; }
        public BuildResult? LastCompletedBuildResult { get; set; }
        
        private bool? _isSelected;
        public override bool? IsSelected
        {
            get { return _isSelected; }
            set { SetIsSelected(value, true); }
        }

        internal void SetIsSelected(bool? value, bool updateParent)
        {
            _isSelected = value;
            RaisePropertyChanged(() => IsSelected);

            if (Parent == null) return;

            if (updateParent)
            {
                bool? parentSelectedValue =
                    TreeHelper.GetChildrenSelectedState(Parent.Children.Cast<TreeItem>().ToList());
                Parent.SetIsSelected(parentSelectedValue, true, false);
            }
        }
    }
}