﻿using BuildNotifications.Client;
using BuildNotifications.Interface;
using BuildNotifications.Interface.Client;
using BuildNotifications.Interface.Service;
using BuildNotifications.Interface.ViewModel;
using BuildNotifications.Service;
using BuildNotifications.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using Ninject.Modules;

namespace BuildNotifications.Ninject
{
    public class RunTimeModule : NinjectModule
    {
        public override void Load()
        {
            // MVVM Light
            Bind<IMessenger>().ToMethod(x => Messenger.Default);

            // ViewModel
            Bind<IMainViewModel>().To<MainViewModel>().InSingletonScope();
            Bind<IConfigureAccountViewModel>().To<ConfigureAccountViewModel>();
            Bind<IManageAccountsViewModel>().To<ManageAccountsViewModel>();

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
