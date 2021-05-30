using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

//Activity.DefaultIdFormat = ActivityIdFormat.W3C;

//Host.CreateDefaultBuilder(args)
//    .ConfigureWebHostDefaults(webBuilder =>
//    {
//        // webBuilder.UseUrls("http://+:5001");
//        webBuilder.UseStartup<Startup>();
//    }).Build().Run();
namespace Gadget.Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://+:5001")
                .UseStartup<Startup>();
    }
}
