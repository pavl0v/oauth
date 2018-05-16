using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomOAuthServer
{
    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        // https://www.codeproject.com/Articles/876870/Implement-OAuth-JSON-Web-Tokens-Authentication-in
        // https://stackoverflow.com/questions/38338580/conflict-between-system-identitymodel-tokens-and-microsoft-identitymodel-tokens/38342794

        private readonly string _issuer = string.Empty;
        private readonly string _audience = string.Empty;

        public CustomJwtFormat(string issuer, string audience)
        {
            _issuer = issuer;
            _audience = audience;
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            var keyByteArray = Encoding.Default.GetBytes(ExampleCommon.Constants.SECURITY_KEY);
            var securityKey = new SymmetricSecurityKey(keyByteArray);
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var issued = data.Properties.IssuedUtc;
            var expires = data.Properties.ExpiresUtc;
            var token = new JwtSecurityToken(
                _issuer, 
                _audience, 
                data.Identity.Claims,
                issued.Value.LocalDateTime, 
                expires.Value.LocalDateTime, 
                signingCredentials);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.WriteToken(token);

            return jwt;
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}
