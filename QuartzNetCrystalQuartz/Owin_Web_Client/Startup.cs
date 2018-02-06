using CrystalQuartz.Core.SchedulerProviders;
using CrystalQuartz.Owin;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Owin_Web_Client.Startup))]
namespace Owin_Web_Client
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCrystalQuartz(new RemoteSchedulerProvider
            {
                SchedulerHost = "tcp://localhost:555/QuartzScheduler"
            });
        }
    }
}