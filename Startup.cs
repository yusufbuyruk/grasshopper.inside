using Owin;
using System.Web.Http;
using Newtonsoft.Json;
using System;

namespace OwinSelfhostSample
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "GetDocuments", 
                routeTemplate: "documents", 
                defaults: new { controller = "Documents", action = "GetDocuments" }
                );

            config.Routes.MapHttpRoute(
                name: "Upload", 
                routeTemplate: "upload/{filename}", 
                defaults: new { controller = "Documents", action = "Upload" }
                );

            config.Routes.MapHttpRoute(
                name: "Delete",
                routeTemplate: "delete/{id}",
                defaults: new { controller = "Documents", action = "Delete", id = 0 }
                );

            config.Routes.MapHttpRoute(
                name: "Load",
                routeTemplate: "load/{id}", 
                defaults: new { controller = "Documents", action = "Load", id = 0 }
                );

            config.Routes.MapHttpRoute(
                name: "GetCluster",
                routeTemplate: "getcluster/{id}",
                defaults: new { controller = "Documents", action = "GetCluster", id = 0 }
                );

            config.Routes.MapHttpRoute(
                name: "Compress", 
                routeTemplate: "compress/{id}", 
                defaults: new { controller = "Documents", action = "Compress", id = 0 }
                );

            config.Routes.MapHttpRoute(
                name: "Compute", 
                routeTemplate: "compute/{id}",
                defaults: new { controller = "Documents", action = "Compute", id = 0 } 
                );

            // dev  - Formatting.Indented
            // dist - Formatting.None
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;


            //// Timeout Middleware
            //appBuilder.Use(async (context, next) =>
            //{
            //    context.Environment["owin.RequestTimeout"] = TimeSpan.FromMinutes(2);
            //    await next.Invoke();
            //});

            appBuilder.UseWebApi(config);
        }
    }
}
