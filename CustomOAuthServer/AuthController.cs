using ExampleCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CustomOAuthServer
{
    public class AuthController : ApiController
    {
        private readonly IUserManager _userManager;

        public AuthController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public string Salt(string username)
        {
            var salt = _userManager.GetSalt(username);

            return salt;
        }
    }
}
