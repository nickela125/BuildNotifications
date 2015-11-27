using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;

namespace BuildNotifications.ViewModel.Locator
{
    public class CustomServiceLocator : ServiceLocatorImplBase
    {
        public IKernel Kernel { get; private set; }

        public CustomServiceLocator(IKernel kernel)
        {
            this.Kernel = kernel;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (key == null)
                return ResolutionExtensions.Get((IResolutionRoot)this.Kernel, serviceType, new IParameter[0]);
            else
                return ResolutionExtensions.Get((IResolutionRoot)this.Kernel, serviceType, key, new IParameter[0]);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return ResolutionExtensions.GetAll((IResolutionRoot)this.Kernel, serviceType, new IParameter[0]);
        }
    }
}
