using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HOTP.Startup))]
namespace HOTP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
