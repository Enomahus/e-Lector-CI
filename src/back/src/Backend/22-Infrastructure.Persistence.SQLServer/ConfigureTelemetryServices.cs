using Microsoft.Data.SqlClient;
using OpenTelemetry.Instrumentation.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Persistence.SQLServer;

[ExcludeFromCodeCoverage]
public static class ConfigureTelemetryServices
{
    public static void AddEntityFrameworkCoreTelemetry(EntityFrameworkInstrumentationOptions options)
    {
        options.SetDbStatementForText = true;
        options.EnrichWithIDbCommand = (activity, command) =>
        {
            foreach (var p in command.Parameters.OfType<SqlParameter>())
            {
                activity.SetTag(p.ParameterName, p.SqlValue);
            }
        };
    }
}
