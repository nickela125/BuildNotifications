using System.Collections.Generic;
using System.ComponentModel;
using BuildNotifications.Model;
using GalaSoft.MvvmLight.CommandWpf;

namespace BuildNotifications.Interface.ViewModel
{
    public interface IMainViewModel
    {
        RelayCommand<CancelEventArgs> CloseCommand { get; }
        RelayCommand ManageBuildsCommand { get; }
        IList<SubscribedBuild> SubscribedBuilds { get; set; }
    }
}
