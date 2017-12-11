using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BadBeeAdminPanel.Startup))]
namespace BadBeeAdminPanel
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
