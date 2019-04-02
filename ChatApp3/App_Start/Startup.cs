using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ChatApp3.App_Start.Startup))]

namespace ChatApp3.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
