using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RentC.WebUI.Startup))]
namespace RentC.WebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
