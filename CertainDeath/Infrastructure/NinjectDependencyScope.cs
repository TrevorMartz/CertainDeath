using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Web.Http.Dependencies;
using Ninject;
using Ninject.Syntax;

namespace CertainDeath.Infrastructure
{
    // Provides a Ninject implementation of IDependencyScope
    // which resolves services using the Ninject container.
    public class NinjectDependencyScope : IDependencyScope
    {
        IResolutionRoot _resolver;

        internal NinjectDependencyScope(IResolutionRoot resolver)
        {
            _resolver = resolver;
        }

        public object GetService(Type serviceType)
        {
            if (_resolver == null)
            {
                throw new ObjectDisposedException("this", "This scope has already been disposed");
            }

            return _resolver.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (_resolver == null)
            {
                throw new ObjectDisposedException("this", "This scope has already been disposed");
            }

            return _resolver.GetAll(serviceType);
        }

        public void Dispose()
        {
            IDisposable disposable = _resolver as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }

            _resolver = null;
        }
    }
}