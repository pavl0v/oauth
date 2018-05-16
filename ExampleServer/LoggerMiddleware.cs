using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleServer
{
    public class LoggerMiddleware : OwinMiddleware
    {
        //private readonly ILog _logger;

        public LoggerMiddleware(OwinMiddleware next/*, ILog logger*/) 
            : base(next)
        {
            //_logger = logger;
        }

        public override async Task Invoke(IOwinContext context)
        {
            //_logger.LogInfo("Middleware begin");
            Console.WriteLine(context.Request.Path);
            await this.Next.Invoke(context);
            //_logger.LogInfo("Middleware end");
        }
    }
}
