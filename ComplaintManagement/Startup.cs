using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Web;

[assembly: OwinStartupAttribute(typeof(ComplaintManagement.Startup))]
namespace ComplaintManagement
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        static Startup()
        {
            //OAuthOptions = new OAuthAuthorizationServerOptions
            //{
            //    TokenEndpointPath = new PathString("/token"),
            //    Provider = new OAuthProvider(),
            //    AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
            //    AllowInsecureHttp = true
            //};
        }
        public void Configuration(IAppBuilder app)
        {
            // Branch the pipeline here for requests that start with "/signalr"
            //app.Map("/signalr", map =>
            //{
            //    // Setup the CORS middleware to run before SignalR.
            //    // By default this will allow all origins. You can 
            //    // configure the set of origins and/or http verbs by
            //    // providing a cors options with a different policy.
            //    map.UseCors(CorsOptions.AllowAll);
            //    var hubConfiguration = new HubConfiguration
            //    {
            //        // You can enable JSONP by uncommenting line below.
            //        // JSONP requests are insecure but some older browsers (and some
            //        // versions of IE) require JSONP to work cross domain
            //        // EnableJSONP = true
            //    };
            //    // Run the SignalR pipeline. We're not using MapSignalR
            //    // since this branch already runs under the "/signalr"
            //    // path.
            //    hubConfiguration.EnableDetailedErrors = true;
            //    map.RunSignalR(hubConfiguration);
            //});
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Index"),
                LogoutPath = new PathString("/Account/LogOff"),
                ExpireTimeSpan = TimeSpan.FromDays(1.0),
                Provider = new CookieAuthenticationProvider
                {
                    OnApplyRedirect = ctx =>
                    {
                        if (!IsAjaxRequest(ctx.Request))
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                    }
                }
            });
            ConfigureAuth(app);
        }
        private static bool IsAjaxRequest(IOwinRequest request)
        {
            string apiPath = VirtualPathUtility.ToAbsolute("~/api/");
            return request.Uri.LocalPath.StartsWith(apiPath);
        }
        public void ConfigureAuth(IAppBuilder app)
        {
        
        }
    }
}
