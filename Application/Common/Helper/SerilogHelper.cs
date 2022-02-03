using System;
using System.Reflection;
using System.Runtime.InteropServices;
using CleanArchitecture.Application.Common.Formatters;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Exceptions;

namespace CleanArchitecture.Application.Common.Helper
{
    public static class SerilogHelper
    {
        /// <summary>
        /// Provides standardized, centralized Serilog wire-up for a suite of applications.
        /// </summary>
        /// <param name="loggerConfig">The Logger Configuration being created</param>
        /// <param name="assembly">The assembly being Executed</param>
        /// <param name="config">IConfiguration settings -- generally read this from appsettings.json</param>
        /// <param name="logName">The Log name</param>
        public static LoggerConfiguration WithSimpleConfiguration(this LoggerConfiguration loggerConfig, IConfiguration config, [Optional] Assembly assembly, string logName = "log")
        {
            var assemblyName = (assembly ?? Assembly.GetExecutingAssembly()).GetName();

            return loggerConfig
                .ReadFrom.Configuration(config) // minimum levels defined per project in json files 
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithThreadId()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Assembly", $"{assemblyName.Name}")
                .Enrich.WithProperty("Version", $"{assemblyName.Version}")
                .WriteTo.File(new CustomLogFormatter(), $"{config["Serilog:Path"]}/{logName}-{DateTime.Today:MM-dd-yyyy}.json");
        }

    }
}
