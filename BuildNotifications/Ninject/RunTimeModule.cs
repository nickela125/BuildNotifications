using BuildNotifications.Interface;
using BuildNotifications.Interface.ViewModel;
using BuildNotifications.ViewModel;
using Ninject.Modules;

namespace BuildNotifications.Ninject
{
    public class RunTimeModule : NinjectModule
    {
        public override void Load()
        {
            // ViewModel
            Bind<IMainViewModel>().To<MainViewModel>();
            Bind<IBuildListViewModel>().To<BuildListViewModel>();
            Bind<IConfigureAccountViewModel>().To<ConfigureAccountViewModel>();
        }
    }
}
