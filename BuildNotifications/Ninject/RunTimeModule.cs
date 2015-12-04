using BuildNotifications.Client;
using BuildNotifications.Interface;
using BuildNotifications.Interface.Client;
using BuildNotifications.Interface.Service;
using BuildNotifications.Interface.ViewModel;
using BuildNotifications.Service;
using BuildNotifications.ViewModel;
using Ninject.Modules;

namespace BuildNotifications.Ninject
{
    public class RunTimeModule : NinjectModule
    {
        public override void Load()
        {
            // ViewModel
            Bind<IMainViewModel>().To<MainViewModel>().InSingletonScope();
            Bind<IBuildListViewModel>().To<BuildListViewModel>().InSingletonScope();
            Bind<IConfigureAccountViewModel>().To<ConfigureAccountViewModel>();

            // Service
            Bind<IAccountService>().To<AccountService>().InSingletonScope();
            Bind<IMapper>().To<Mapper>().InSingletonScope();

            // Client
            Bind<IRestClient>().To<RestClient>();
            Bind<IVsoClient>().To<VsoClient>();
        }
    }
}
