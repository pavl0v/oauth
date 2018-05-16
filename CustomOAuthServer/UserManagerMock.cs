using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomOAuthServer
{
    class UserManagerMock : ExampleCommon.IUserManager
    {
        public string GetSalt(string username)
        {
            return "salt";
        }

        public bool IsPasswordValid(string username, string passwordHash)
        {
            return true;
        }

        public bool IsUserExist(string username)
        {
            return true;
        }
    }
}
