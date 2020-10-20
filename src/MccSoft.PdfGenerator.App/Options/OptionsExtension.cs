using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MccSoft.PdfGenerator.App.Options
{
    public static class OptionsExtension
    {
        public static void ConfigureCustomOptions(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<ExecutableChromeOptions>(configuration.GetSection("Chrome"));
        }
    }
}