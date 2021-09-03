using Common.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using Web.Filter;

namespace Web.Swagger
{
    public static class SwaggerConfigurationExtensions
    {
        public static void AddCustomSwagger(this IServiceCollection services)
        {
            Assert.NotNull(services, nameof(services));

            //Add services and configuration to use swagger
            services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();

                #region Add UnAuthorized to Response
                //Add 401 response and security requirements (Lock icon) to actions that need authorization
                options.OperationFilter<UnauthorizedResponsesOperationFilter>(true, "OAuth2");
                #endregion

                options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri("/Identities/get-token-by-username-and-password", UriKind.Relative),

                        }
                    }
                }); 
            });
        }
    }
}