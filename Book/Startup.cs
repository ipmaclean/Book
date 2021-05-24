using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Book.Startup))]
namespace Book
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
