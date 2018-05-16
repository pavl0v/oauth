using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.StaticFiles.ContentTypes;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ExampleServer
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use<LoggerMiddleware>();

            ServeStaticFiles(app);

            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Response in Json format
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            // Turn on Windows Authentication
            //HttpListener listener = (HttpListener)app.Properties["System.Net.HttpListener"];
            //listener.AuthenticationSchemeSelectorDelegate = request =>
            //{
            //    if (request.HttpMethod == "OPTIONS")
            //        return AuthenticationSchemes.Anonymous;
            //    else
            //        return AuthenticationSchemes.Ntlm;
            //};

            // Turn on JWT Authentication
            var keyByteArray = Encoding.Default.GetBytes(ExampleCommon.Constants.SECURITY_KEY);
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AllowedAudiences = new[] { ExampleCommon.Constants.AUDIENCE },
                IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                {
                    new SymmetricKeyIssuerSecurityTokenProvider(ExampleCommon.Constants.ISSUER, keyByteArray)
                }
            });

            app.UseWebApi(config);
            //app.UseWelcomePage();
        }

        private void ServeStaticFiles(IAppBuilder app)
        {
            #region Serve Static Files
            
            var physicalFileSystem = new PhysicalFileSystem(@"./public");
            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem
            };
            options.StaticFileOptions.FileSystem = physicalFileSystem;
            //options.StaticFileOptions.ServeUnknownFileTypes = true;
            options.DefaultFilesOptions.DefaultFileNames = new[]
            {
                "index.html"
            };
            app.UseFileServer(options);

            var mimeTypes = new FileExtensionContentTypeProvider();
            app.UseStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = mimeTypes,
                DefaultContentType = "html"
            });

            // Anything not handled will land at the welcome page.
            //app.UseWelcomePage();

            #endregion
        }
    }
}
