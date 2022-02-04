using OneSCodeChanger.DI;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace OneSCodeChanger
{
    internal class Program
    {
        
        static async Task Main(string[] args)
        {
            
            var config = new HttpSelfHostConfiguration("http://localhost:5123");
            Startup.Init(config);

            config.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });
            config.MaxReceivedMessageSize = 2147483647;

            

            using (HttpSelfHostServer server = new HttpSelfHostServer(config))
            {
                server.OpenAsync().Wait();
                //Console.WriteLine("Press Enter to quit.");
                Console.ReadLine();
            }
        }

    }
}
