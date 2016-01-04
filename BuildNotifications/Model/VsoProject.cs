using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Helpers;
using Newtonsoft.Json;

namespace BuildNotifications.Model
{
    public class VsoProject : TreeItem
    {
        public VsoProject()
        {
            DisplayMenu = false;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        private IList<VsoBuildDefinition> _builds;
        public IList<VsoBuildDefinition> Builds {
            get { return _builds; }
            set
            {
                _builds = value;
                Children = Builds;
                Children.ToList().ForEach(p => p.Parent = this);
            }
        }
        [JsonIgnore]
        public new IList<VsoBuildDefinition> Children { get; set; }

        [JsonIgnore]
        public new string DisplayName => Name;
        [JsonIgnore]
        public new VsoAccount Parent { get; set; }

        private bool? _isSelected;
        public override bool? IsSelected
        {
            get { return _isSelected; }
            set { SetIsSelected(value, true, true); }
        }

        internal void SetIsSelected(bool? value, bool updateParent, bool updateChildren)
        {
            _isSelected = value;
            RaisePropertyChanged(() => IsSelected);

            if (Parent != null && updateParent)
            {
                bool? parentSelectedValue =
                    TreeHelper.GetChildrenSelectedState(Parent.Children.Cast<TreeItem>().ToList());
                Parent.SetIsSelected(parentSelectedValue, false);
            }

            if (updateChildren && value != null)
            {
                foreach (VsoBuildDefinition vsoBuildDefinition in Children)
                {
                    vsoBuildDefinition.SetIsSelected(value, false);
                }
            }
        }
    }
}
