using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace EVEMarketWatch
{
    public class UploadController : ApiController
    {
        // POST api/upload 
        public void Post([FromBody]JToken value)
        {
            Console.WriteLine("POST" + value);
        }
    } 
}
