using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using MccSoft.PdfGenerator.App.Options;
using MccSoft.PdfGenerator.App.Services;
using MccSoft.PdfGenerator.App.Utils;

[assembly: ApiController]

namespace MccSoft.PdfGenerator.App
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            _logger = logger;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            _logger.LogInformation($"Start {nameof(ConfigureServices)}");
            
            services.ConfigureCustomOptions(Configuration);

            JsonSerializerSettings SetupJson(JsonSerializerSettings settings)
            {
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                settings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        ProcessDictionaryKeys = false,
                        OverrideSpecifiedNames = false
                    }
                };
                settings.Converters = new List<JsonConverter>
                {
                    new StringEnumConverter()
                };
                return settings;
            }

            JsonConvert.DefaultSettings = () => SetupJson(new JsonSerializerSettings());

            services
                .AddControllers()
                .AddNewtonsoftJson(
                    setupAction => SetupJson(setupAction.SerializerSettings));

            services.AddCors(
                x => x.AddPolicy(
                    "mypolicy",
                    configurePolicy => configurePolicy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin()));

            ConfigureContainer(services);
        }

        public virtual void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseCors("mypolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(
                endpoints => { endpoints.MapControllers(); });

            _logger.LogInformation("Service started.");
        }

        /// <summary>
        /// Register application services here.
        /// Don't move registration outside of this function not to break dependent startups.
        /// </summary>
        private void ConfigureContainer(IServiceCollection services)
        {
            services
                .AddScoped<DateTimeProvider>()
                .AddScoped<PdfGeneratorService>()
                .AddScoped<ViewRenderService>();
            services.AddRazorPages();
        }
    }
}