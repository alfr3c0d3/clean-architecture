using System;
using System.Threading.Tasks;
using CleanArchitecture.Application;
using CleanArchitecture.Application.Activities;
using CleanArchitecture.Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CleanArchitecture.ConsoleApp
{
    internal static class Runner
    {
        private static readonly IServiceProvider ServiceProvider;
        static Runner()
        {
            ServiceProvider = new ServiceCollection()
                .AddSingleton(Program.Configuration)
                .AddApplication()
                .AddInfrastructure(Program.Configuration)
                .BuildServiceProvider();
        }

        internal static async Task GetActivities()
        {
            try
            {
                var mediator = ServiceProvider.GetService<IMediator>();

                if (mediator == null)
                {
                    throw new ArgumentNullException("Mediator Service Not Found");
                }

                LogMessage("Getting Activities using mediator");

                var activities = await mediator.Send(new List.Query());

                LogMessage($"Found {activities.Count} activities");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error(ex, ex.Message);
            }
        }

        private static void LogMessage(string message)
        {
            Console.WriteLine(message);
            Log.Information(message);
        }
    }
}
