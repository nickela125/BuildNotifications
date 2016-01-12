using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Model.Helpers;
using Newtonsoft.Json;

namespace BuildNotifications.Model
{
    public class Project : TreeItem
    {
        public Project()
        {
            DisplayMenu = false;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        private IList<BuildDefinition> _builds;
        public IList<BuildDefinition> Builds {
            get { return _builds; }
            set
            {
                _builds = value;
                Children = Builds;
                Children.ToList().ForEach(p => p.Parent = this);
            }
        }
        [JsonIgnore]
        public new IList<BuildDefinition> Children { get; set; }
        
        [JsonIgnore]
        public new Account Parent { get; set; }

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
                foreach (BuildDefinition vsoBuildDefinition in Children)
                {
                    vsoBuildDefinition.SetIsSelected(value, false);
                }
            }
        }
    }
}
