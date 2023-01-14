using System.Reflection;
using System.Text.RegularExpressions;

namespace opentelemetry_newrelic_template.Extensions;

public static class MethodBaseExtensions
{
    public static string GetNameOfAsyncMethod(this MethodBase currentMethod)
    {
        var input = currentMethod.DeclaringType?.UnderlyingSystemType.Name;
        Match match = Regex.Match(input, "<(.*?)>");
        return match.Groups[1].ToString();
    }

    public static string GetNameOfMethod(this MethodBase currentMethod) => currentMethod.Name;
}
