using System.Diagnostics;
using Gadget.Server;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

//Activity.DefaultIdFormat = ActivityIdFormat.W3C;

//Host.CreateDefaultBuilder(args)
//    .ConfigureWebHostDefaults(webBuilder =>
//    {
//        // webBuilder.UseUrls("http://+:5001");
//        webBuilder.UseStartup<Startup>();
//    }).Build().Run();
public static class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();
}
