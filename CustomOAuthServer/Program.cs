using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomOAuthServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (WebApp.Start<Startup>(url: ExampleCommon.Constants.BASE_URL_OAUTH_SERVER))
            {
                Console.Write("Custom OAuth server started at " + ExampleCommon.Constants.BASE_URL_OAUTH_SERVER);
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
