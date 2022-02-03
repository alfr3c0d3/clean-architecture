using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CleanArchitecture.Api.Filters;
using CleanArchitecture.Api.Middleware;
using CleanArchitecture.Application;
using CleanArchitecture.Application.Activities;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Infrastructure.Persistence;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CleanArchitecture.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication();
            services.AddInfrastructure(Configuration);

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithExposedHeaders("WWW-Authenticate")
                        .WithOrigins("http://localhost:3000")
                        .AllowCredentials();
                });
            });

            // In case of MVC Project use line 56 instead of 57
            //services.AddControllersWithViews();
            services.AddControllers(opt =>
            {
                // Make All Endpoint to require Authorization by default. Place [Anonymous] Attribute above public Endpoint
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            }).AddFluentValidation(cfg =>
            {
                cfg.RegisterValidatorsFromAssemblyContaining<Create>();
            });

            // Set Identity Builder 
            var builder = services.AddIdentityCore<AppUser>();
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
            identityBuilder.AddEntityFrameworkStores<ApplicationDbContext>();
            identityBuilder.AddSignInManager<SignInManager<AppUser>>();

            // Add Custom Policies to the [Authorize] Attribute 
            //services.AddAuthorization(opt =>
            //{
            //    opt.AddPolicy("IsActivityHost", policy =>
            //    {
            //        policy.Requirements.Add(new IsHostRequirement());
            //    });
            //});

            // Add Authentication with Jwt by Auth0
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"]));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Set up Swagger Page
            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(x => x.FullName);
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                var swaggerDoc = Configuration.GetSection("Swagger:Document");

                c.SwaggerDoc(swaggerDoc["Name"], new OpenApiInfo
                {
                    Title = swaggerDoc["Info:Title"],
                    Version = swaggerDoc["Info:Version"],
                    Description = swaggerDoc["Info:Description"]
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.RequestBodyFilter<SwaggerRequestBodyFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (env.IsProduction()) return;

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                var swaggerEndpoint = Configuration.GetSection("Swagger:UI:EndPoint");

                c.SwaggerEndpoint(swaggerEndpoint["Url"], swaggerEndpoint["Name"]);
                c.RoutePrefix = string.Empty;
                c.DisplayOperationId();
            });
        }
    }
}
