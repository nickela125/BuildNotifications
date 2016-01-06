using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BuildNotifications.Model
{
    public class VsoAccount : TreeItem
    {
        public VsoAccount()
        {
            DisplayMenu = true;
        }

        public string Name { get; set; }
        public string Username { get; set; }
        public string EncodedCredentials { get; set; }
        private IList<VsoProject> _projects;

        public IList<VsoProject> Projects
        {
            get { return _projects;}
            set
            {
                _projects = value;
                Children = Projects;
                Children.ToList().ForEach(p => p.Parent = this);
            }
        }

        [JsonIgnore]
        public new IList<VsoProject> Children { get; set; }
        
        [JsonIgnore]
        public new string DisplayName => Name;

        private bool? _isSelected;
        public override bool? IsSelected
        {
            get { return _isSelected; }
            set { SetIsSelected(value, true); }
        }
        internal void SetIsSelected(bool? value, bool updateChildren)
        {
            _isSelected = value;
            RaisePropertyChanged(() => IsSelected);

            if (updateChildren)
            {
                foreach (VsoProject vsoProject in Children)
                {
                    vsoProject.SetIsSelected(value, false, true);
                }
            }
        }
    }
}
