using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ExampleServer.Controllers
{
    public class ValuesController : ApiController
    {
        [HttpGet]
        [Authorize]
        public IEnumerable<string> Get()
        {
            return new List<string> { "ASP.NET", "Docker", "Windows Containers" };
        }
    }
}
