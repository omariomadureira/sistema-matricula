using Microsoft.Owin;
using Owin;
using SistemaMatricula.Helpers;

[assembly: OwinStartupAttribute(typeof(SistemaMatricula.Startup))]
namespace SistemaMatricula
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            try
            {
                ConfigureAuth(app);
                CreateUserRoles();
            }
            catch (System.Exception e)
            {
                string notes = LogHelper.Notes(app, e.Message);
                Models.Log.Add(Models.Log.TYPE_ERROR, "SistemaMatricula.Startup", notes);
            }
        }
    }
}
