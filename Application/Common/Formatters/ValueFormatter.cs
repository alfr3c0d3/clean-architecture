using Serilog.Formatting.Json;

namespace CleanArchitecture.Application.Common.Formatters
{
    internal static class ValueFormatter
    {
        internal static readonly JsonValueFormatter Instance = new JsonValueFormatter();
    }
}
