using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WhatIsIt.Startup))]
namespace WhatIsIt
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
