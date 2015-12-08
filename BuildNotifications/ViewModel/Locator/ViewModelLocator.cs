using BuildNotifications.Interface;
using BuildNotifications.Interface.ViewModel;
using BuildNotifications.Ninject;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Ninject;

namespace BuildNotifications.ViewModel.Locator
{
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            StandardKernel kernel = new StandardKernel(new RunTimeModule());

            ServiceLocator.SetLocatorProvider(() => new CustomServiceLocator(kernel));
        }

        public IMainViewModel Main => GetInstance<IMainViewModel>();
        public IBuildListViewModel BuildList => GetInstance<IBuildListViewModel>();
        public IConfigureAccountViewModel ConfigureAccount => GetInstance<IConfigureAccountViewModel>();
        public IManageAccountsViewModel ManageAccounts => GetInstance<IManageAccountsViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

        private static T GetInstance<T>()
        {
            var viewModel = ServiceLocator.Current.GetInstance<T>();

            return viewModel;
        }
    }
}
