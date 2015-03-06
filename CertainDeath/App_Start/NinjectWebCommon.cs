[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(CertainDeath.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(CertainDeath.NinjectWebCommon), "Stop")]

namespace CertainDeath
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using System.Web.Http;
    using Infrastructure;
    using CertainDeathEngine.DAL;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper Bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            Bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            Bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);

                // Install our Ninject-based IDependencyResolver into the Web API config
                GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            // This is where we tell Ninject how to resolve service requests
            //kernel.Bind<IProductRepository>().ToConstant(mock.Object);
            //kernel.Bind<IGameDAL>().To<BasicGameDAL>().InSingletonScope().WithConstructorArgument("path", HostingEnvironment.MapPath("~\\Data"));
            //kernel.Bind<IUserDAL>().To<BasicUserDAL>().InSingletonScope().WithConstructorArgument("path", HostingEnvironment.MapPath("~\\Data"));
            kernel.Bind<IGameDAL>().To<EFGameDAL>();
            kernel.Bind<IUserDAL>().To<EFUserDAL>();
            kernel.Bind<IStatisticsDAL>().To<EFStatisticsDAL>().InSingletonScope();
        }

        public static void RegisterNinject(HttpConfiguration configuration)
        {
            // Set Web API Resolver
            configuration.DependencyResolver = new NinjectResolver(new Bootstrapper().Kernel);

        }
    }
}
