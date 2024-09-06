using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Sally.ApiService
{
    using Sally.ApiService.Logging;
    using Sally.ServiceDefaults.API.Logger;

    public class Startup
    {
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                Log.Info("Configuring Kestrel server options");
                options.ListenAnyIP(Configuration.GetValue<int>("Kestrel:Port"));
            });

            var serviceProvider = services.BuildServiceProvider();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddProvider(new CustomLoggerProvider());
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
