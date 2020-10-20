using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace MccSoft.PdfGenerator.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder<Startup>(args)
                .Build()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder<T>(string[] args) where T : class
        {
            return WebHost.CreateDefaultBuilder<T>(args);
        }
    }
}