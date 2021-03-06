﻿using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BuildNotifications.Model
{
    public class Account : TreeItem
    {
        public Account()
        {
            DisplayMenu = true;
        }

        public string Name { get; set; }
        public string Username { get; set; }
        public string EncodedCredentials { get; set; }
        private IList<Project> _projects;

        public IList<Project> Projects
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
        public new IList<Project> Children { get; set; }

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
                foreach (Project vsoProject in Children)
                {
                    vsoProject.SetIsSelected(value, false, true);
                }
            }
        }
    }
}
