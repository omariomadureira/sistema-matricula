using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SistemaMatricula.Startup))]
namespace SistemaMatricula
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateUserRoles();
        }
    }
}
