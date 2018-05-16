using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleCommon
{
    public interface IUserManager
    {
        string GetSalt(string username);
        bool IsUserExist(string username);
        bool IsPasswordValid(string username, string passwordHash);
    }
}
