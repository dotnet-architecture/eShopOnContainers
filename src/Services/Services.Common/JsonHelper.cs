using System.Text.Json;

namespace Services.Common;

public static class JsonHelper
{
    public static readonly JsonSerializerOptions CaseInsensitiveOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
