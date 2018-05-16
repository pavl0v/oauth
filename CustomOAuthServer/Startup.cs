using ExampleCommon;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Ninject;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CustomOAuthServer
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // http://localhost:801/api/auth/salt/?username=name

            IKernel kernel = CreateKernel();

            HttpConfiguration config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Response in Json format
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                AccessTokenExpireTimeSpan = TimeSpan.FromSeconds(ExampleCommon.Constants.ACCESS_TOKEN_EXPIRED_SECONDS),
                AccessTokenFormat = new CustomJwtFormat(ExampleCommon.Constants.ISSUER, ExampleCommon.Constants.AUDIENCE),
                AllowInsecureHttp = true,
                Provider = new CustomAuthorizationServerProvider(kernel.Get<IUserManager>()),
                RefreshTokenProvider = new AuthenticationTokenProvider
                {
                    OnCreate = CreateRefreshToken,
                    OnReceive = ReceiveRefreshToken,
                },
                TokenEndpointPath = new PathString(ExampleCommon.Constants.TOKEN_ENDPOINT_PATH)
            });

            app.UseNinjectMiddleware(CreateKernel);
            app.UseNinjectWebApi(config);

            app.UseWebApi(config);
        }

        private void CreateRefreshToken(AuthenticationTokenCreateContext context)
        {
            context.Ticket.Properties.ExpiresUtc = new DateTimeOffset(DateTime.Now.AddHours(ExampleCommon.Constants.REFRESH_TOKEN_EXPIRED_HOURS));
            context.SetToken(context.SerializeTicket());
        }

        private void ReceiveRefreshToken(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);
        }

        private IKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            kernel.Bind<IUserManager>().To<UserManagerMock>().InSingletonScope();

            return kernel;
        }
    }
}
