using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Helper;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace CleanArchitecture.ConsoleApp
{
    internal static class Program
    {
        internal static readonly IConfiguration Configuration;

        static Program()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(@"appsettings.json")
                .Build();
        }

        private static async Task Main()
        {
            Log.Logger = new LoggerConfiguration().WithSimpleConfiguration(Configuration, Assembly.GetExecutingAssembly()).CreateLogger();

            try
            {
                Log.Information("--- Console App Execution Started ---");
                await Runner.GetActivities();
                Log.Information("--- Console App Execution Completed ---");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "--- Console App terminated unexpectedly ---");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }
    }
}
