using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using BuildNotifications.Model;
using GalaSoft.MvvmLight.CommandWpf;

namespace BuildNotifications.Interface.ViewModel
{
    public interface IMainViewModel
    {
        RelayCommand<CancelEventArgs> CloseCommand { get; }
        RelayCommand ManageBuildsCommand { get; }
        ObservableCollection<SubscribedBuild> SubscribedBuilds { get; set; }
        ListCollectionView GroupedSubscribedBuilds { get; set; }
    }
}
