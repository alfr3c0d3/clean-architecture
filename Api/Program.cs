using System;
using System.IO;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Helper;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CleanArchitecture.Api
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            #region Run Migrations 
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    var userManager = services.GetRequiredService<UserManager<AppUser>>();
                    await context.Database.MigrateAsync();
                    Seed.SeedData(context, userManager).Wait();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred during migration");
                }
            }
            #endregion

            try
            {
                Log.Information("Starting Web Host");
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Web Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .ConfigureKestrel(opt => {
                        opt.AddServerHeader = false;
                        opt.Limits.MaxRequestBodySize = 200 * (int)Math.Pow(1024, 2);
                    })
                    .UseStartup<Startup>()
                    .UseSerilog((context, loggerConfig) => loggerConfig.WithSimpleConfiguration(Configuration));
            });
    }
}
