﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Facebook;
using CertainDeathEngine;
using System.Data.Entity;
using CertainDeathEngine.DB;
using System.IO;
using System.Web.Helpers;
using log4net;

//using CertainDeath.Infrastructure;

namespace CertainDeath
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));
            ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            logger.Info("Application_Start");


            AreaRegistration.RegisterAllAreas();
            FacebookConfig.Register(GlobalFacebookConfiguration.Configuration);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;

            //NinjectWebCommon.RegisterNinject(GlobalConfiguration.Configuration);

            //Build the database
            //Database.SetInitializer<CDDBModel>(null);

            // Set up the Ninject stuff
            //ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());

            // Init the static game stuff
            CertainDeathEngine.Init.InitAll();
        }
    }
}