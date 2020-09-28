using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebFramework.Configurations;

namespace Web
{
    public class Startup
    {
        private readonly SecuritySettings _securitySettings;

        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            _securitySettings = configuration.GetSection(nameof(SecuritySettings)).Get<SecuritySettings>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SecuritySettings>(_configuration.GetSection(nameof(SecuritySettings)));

            services.AddJwtAuthentication(_securitySettings.JwtSettings);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            });
        }
    }
}