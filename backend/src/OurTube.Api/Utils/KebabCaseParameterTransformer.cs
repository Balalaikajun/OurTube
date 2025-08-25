using System.Text.RegularExpressions;

namespace OurTube.Api.Utils;

public class KebabCaseParameterTransformer: IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        if (value == null) return null;
        
        return Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1-$2").ToLower();
    }
}