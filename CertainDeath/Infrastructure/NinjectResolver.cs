//////using System.Web.Http.Dependencies;
//////using Ninject;

//////namespace CertainDeath.Infrastructure
//////{
//////    public class NinjectResolver : NinjectDependencyScope, IDependencyResolver
//////    {
//////        private readonly IKernel _kernel;

//////        public NinjectResolver(IKernel kernel)
//////            : base(kernel)
//////        {
//////            _kernel = kernel;
//////        }

//////        public IDependencyScope BeginScope()
//////        {
//////            return new NinjectDependencyScope(_kernel.BeginBlock());
//////        }
//////    }
//////}