using Owin;
using System.Web.Http;
using Newtonsoft.Json;

namespace OwinSelfhostSample
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "GetDocuments", 
                routeTemplate: "api/documents", 
                defaults: new { controller = "Documents", action = "GetDocuments" }
                );

            config.Routes.MapHttpRoute(
                name: "Upload", 
                routeTemplate: "api/upload/{filename}", 
                defaults: new { controller = "Documents", action = "Upload" }
                );

            config.Routes.MapHttpRoute(
                name: "Delete",
                routeTemplate: "api/delete/{filename}",
                defaults: new { controller = "Documents", action = "Delete" }
                );

            config.Routes.MapHttpRoute(
                name: "Load", 
                routeTemplate: "api/load/{filename}", 
                defaults: new { controller = "Documents", action = "Load" }
                );

            config.Routes.MapHttpRoute(
                name: "Compress", 
                routeTemplate: "api/compress/{filename}", 
                defaults: new { controller = "Documents", action = "Compress" }
                );

            config.Routes.MapHttpRoute(
                name: "Compute", 
                routeTemplate: "api/compute/{id}",
                defaults: new { controller = "Documents", action = "Compute", id = 0 } 
                );

            // dev  - Formatting.Indented
            // dist - Formatting.None
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;

            appBuilder.UseWebApi(config);
        }
    }
}
