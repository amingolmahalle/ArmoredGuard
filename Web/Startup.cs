using Common.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.Configurations;
using Web.Swagger;

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
            services.AddInjectionServices();
            services.AddDbContext(_configuration);
            services.AddCustomIdentity(_securitySettings.IdentitySettings);
            services.AddJwtAuthentication(_securitySettings.JwtSettings);
            services.AddCustomSwagger();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins("http://localhost:5000", "https://localhost:5001")
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
            services.AddControllers().AddNewtonsoftJson();

            services.AddStackExchangeRedisCache(options => { options.Configuration = "localhost:6379"; });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(config => { config.MapControllers(); });
        }
    }
}