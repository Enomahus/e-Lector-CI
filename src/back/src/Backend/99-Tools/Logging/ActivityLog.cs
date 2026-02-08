using Newtonsoft.Json;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Tools.Serialization;

namespace Tools.Logging;

[ExcludeFromCodeCoverage]
public class ActivityLog(Activity? activity) : IDisposable
{

    public ActivityLog AddParameters(params object[] parameters)
    {
        if (parameters?.Length > 0)
        {
            var settings = new JsonSerializerSettings() { ContractResolver = new SensitiveDataResolver() };
            var parametersJson = parameters.Select(p => JsonConvert.SerializeObject(p, settings)).ToList();
            activity = activity?.AddTag("parameters", string.Join(", ", parametersJson));
        }

        return this;
    }

    public void AddEvent(string message)
    {
        activity?.AddEvent(new ActivityEvent(message));
    }

    public void SetException(Exception ex)
    {
        activity?.AddException(ex);        
        activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
    }

    public void AddTag(string key, string value)
    {
        activity?.AddTag(key, value);
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        activity?.Dispose();
    }
}
