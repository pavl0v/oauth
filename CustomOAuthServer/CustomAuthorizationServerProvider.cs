using ExampleCommon;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CustomOAuthServer
{
    class CustomAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly IUserManager _userManager;

        public CustomAuthorizationServerProvider(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult(0);
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add(new KeyValuePair<string, string[]>("Access-Control-Allow-Origin", new[] { "*" }));

            //if (context.UserName != context.Password)
            if (!_userManager.IsPasswordValid(context.UserName, context.Password))
            {
                context.SetError("invalid_grant", "Username and password are invalid.");
                return Task.FromResult(0);
            }
            //var b = CheckCredentials(context.UserName, context.Password, "TCSBANK");

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            //identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "1"));
            identity.AddClaim(new Claim("id", "111"));

            context.Validated(identity);
            return Task.FromResult(0);
        }

        private bool CheckCredentials(string userName, string password, string domain)
        {
            string userPrincipalName = domain + "\\" + userName;

            try
            {
                using (var context = new PrincipalContext(ContextType.Domain, domain))
                {
                    return context.ValidateCredentials(userPrincipalName, password);
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
