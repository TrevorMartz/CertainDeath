//using CertainDeathEngine.DAL;
//using Ninject;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Hosting;
//using System.Web.Mvc;

//namespace CertainDeath.Infrastructure
//{
//    public class NinjectControllerFactory : DefaultControllerFactory
//    {
//        private IKernel ninjectKernel;

//        public NinjectControllerFactory()
//        {
//            ninjectKernel = new StandardKernel();
//            AddBindings();
//        }

//        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
//        {
//            return controllerType == null ? null : (IController)ninjectKernel.Get(controllerType);
//        }

//        private void AddBindings()
//        {
//            //Mock<IProductRepository> mock = new Mock<IProductRepository>();
//            //mock.Setup(m => m.Products).Returns(new List<Product> {
//            //    new Product() { Name = "Football", Price = 25},
//            //    new Product() { Name = "Surf Board", Price = 179},
//            //    new Product() { Name = "Running Shoes", Price = 95},
//            //}.AsQueryable());

//            //ninjectKernel.Bind<IProductRepository>().ToConstant(mock.Object);
//            ninjectKernel.Bind<IGameDAL>().To<BasicGameDAL>().InSingletonScope().WithConstructorArgument("path", HostingEnvironment.MapPath("~\\Data"));
//            ninjectKernel.Bind<IUserDAL>().To<BasicUserDAL>().InSingletonScope().WithConstructorArgument("path", HostingEnvironment.MapPath("~\\Data"));
//            //ninjectKernel.Bind<IGameDAL>().To<EFGameDAL>();
//            //ninjectKernel.Bind<IUserDAL>().To<EFUserDAL>();
//            ninjectKernel.Bind<IStatisticsDAL>().To<EFStatisticsDAL>().InSingletonScope();

//            //GlobalConfiguration.Configuration.DependencyResolver = new NinjectResolver(ninjectKernel);
//        }
//    }
//}