using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MessagingSystem.WebUI.Startup))]
namespace MessagingSystem.WebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
