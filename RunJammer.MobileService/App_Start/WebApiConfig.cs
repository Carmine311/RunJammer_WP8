using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Web.Http;
using RunJammer.MobileService.Models;
using Microsoft.WindowsAzure.Mobile.Service;

namespace RunJammer.MobileService
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));
            
            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            
            Database.SetInitializer(new RunJammerMobileServiceInitializer());
        }
    }

    public class RunJammerMobileServiceInitializer : DropCreateDatabaseIfModelChanges<RunJammerMobileServiceContext>
    {
        protected override void Seed(RunJammerMobileServiceContext context)
        {
            

            base.Seed(context);
        }
    }
}

