using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleCommon
{
    public static class Constants
    {
        public const string AUDIENCE = "all";
        public const string BASE_URL_OAUTH_SERVER = "http://localhost:801";
        public const string BASE_URL_SOURCE_SERVER = "http://localhost:802";
        public const string ISSUER = "http://localhost:801";
        public const string SECURITY_KEY = "UHxNtYMRYwvfpO1dS5pWLKL0M2DgOj40EbN4SoBWgfc";
        public const string TOKEN_ENDPOINT_PATH = "/token";
        public const int ACCESS_TOKEN_EXPIRED_SECONDS = 7;
        public const int REFRESH_TOKEN_EXPIRED_HOURS = 5;
    }
}
