using System;
using System.Collections.Generic;
using Castle.Windsor;
using Microsoft.Practices.ServiceLocation;
using UI.CastleWindsorAdapter.ServiceLocator.Extensions;

namespace UI.CastleWindsorAdapter.ServiceLocator.Adapters
{
    public class CastleWindsorServiceLocatorAdapter : ServiceLocatorImplBase
    {
        private readonly IWindsorContainer _windsorContainer;

        public CastleWindsorServiceLocatorAdapter(IWindsorContainer theWindsorContainer)
        {
            _windsorContainer = theWindsorContainer;
        }

        public IWindsorContainer WindsorContainer => _windsorContainer;
        protected override object DoGetInstance(Type serviceType, string key)
        {
            return CastleWindsorContainerExtensions.Resolve(WindsorContainer, serviceType);

            //if (key == null)
            //    return WindsorContainer.Resolve(serviceType);
            //return WindsorContainer.Resolve(key, serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return WindsorContainer.ResolveAll(serviceType) as IEnumerable<object>;
        }
    }
}