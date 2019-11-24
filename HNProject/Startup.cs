using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(HNProject.Startup))]

namespace HNProject
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            SwaggerConfig.Register(config);
            app.UseWebApi(config);

            //SingalR
            var config1 = new HubConfiguration();
            config1.EnableJSONP = true;
            app.MapSignalR("/signalr", config1);
            GlobalHost.Configuration.MaxIncomingWebSocketMessageSize = null;
        }
    }


}
