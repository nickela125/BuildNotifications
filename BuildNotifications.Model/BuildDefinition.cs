using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Model.Helpers;
using Newtonsoft.Json;

namespace BuildNotifications.Model
{
    public class BuildDefinition : TreeItem
    {
        public BuildDefinition()
        {
            DisplayMenu = false;
        }

        public string Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public new Project Parent { get; set; }
        
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