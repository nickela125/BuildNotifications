using BuildNotifications.Client;
using BuildNotifications.Design.ViewModel;
using BuildNotifications.Interface.Client;
using BuildNotifications.Interface.Service;
using BuildNotifications.Interface.ViewModel;
using BuildNotifications.Service;
using BuildNotifications.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using Ninject.Modules;

namespace BuildNotifications.Ninject
{
    public class DesignTimeModule : NinjectModule
    {
        public override void Load()
        {
            // MVVM Light
            Bind<IMessenger>().ToMethod(x => Messenger.Default);

            // ViewModel
            Bind<IMainViewModel>().To<DesignMainViewModel>().InSingletonScope();
            Bind<IConfigureAccountViewModel>().To<ConfigureAccountViewModel>();
            Bind<IManageBuildsViewModel>().To<DesignManageBuildsViewModel>();

            // Service
            Bind<IAccountService>().To<AccountService>().InSingletonScope();
            Bind<IMapper>().To<Mapper>().InSingletonScope();
            Bind<IBuildService>().To<BuildService>().InSingletonScope();

            // Client
            Bind<IRestClient>().To<RestClient>();
            Bind<IVsoClient>().To<VsoClient>();
        }
    }
}
