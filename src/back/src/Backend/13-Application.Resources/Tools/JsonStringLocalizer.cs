using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Text.Json;

namespace Application.Resources.Tools;

public class JsonStringLocalizer(IDistributedCache cache) : IStringLocalizer
{
    public LocalizedString this[string name]
    {
        get
        {
            var value = GetString(name);
            return new LocalizedString(name, value ?? name, value == null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var actualValue = this[name];
            return !actualValue.ResourceNotFound
                ? new LocalizedString(name, string.Format(actualValue.Value, arguments), false)
                : actualValue;
        }
    }

    private string? GetString(string key)
    {
        var cacheKey = $"locale_{Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName}_{key}";
        var cacheValue = cache.GetString(cacheKey);
        if (!string.IsNullOrEmpty(cacheValue))
        {
            return cacheValue;
        }

        var map = LoadStringMap();

        if (map != null && map.TryGetValue(key, out string? result))
        {
            if (!string.IsNullOrEmpty(result))
            {
                cache.SetString(cacheKey, result);
            }
            return result;
        }
        return default;
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        Dictionary<string, string> map = LoadStringMap() ?? [];
        foreach (var key in map.Keys)
        {
            yield return new LocalizedString(key, map[key], false);
        }
    }

    public IEnumerable<LocalizedString> GetStrings(bool includeParentCultures)
    {
        throw new NotImplementedException();
    }

    private static Dictionary<string, string>? LoadStringMap()
    {
        var cultureInfo = CultureInfo.CurrentUICulture;

        var cultureName = cultureInfo.TwoLetterISOLanguageName;

        // TODO voir avec JUPL si il a une idée pour débloquer ça
        var resourceNames = typeof(ApplicationResources).Assembly.GetManifestResourceNames();
        var resourceName = resourceNames.First(r => r.Contains($"{cultureName}.json"));
        var stream = typeof(ApplicationResources).Assembly.GetManifestResourceStream(resourceName);

        if (stream != null)
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(stream);
        }
        return [];
    }
}

public class JsonStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly IDistributedCache _cache;

    public JsonStringLocalizerFactory(IDistributedCache cache)
    {
        _cache = cache;
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        return new JsonStringLocalizer(_cache);
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        throw new NotImplementedException();
    }
}
