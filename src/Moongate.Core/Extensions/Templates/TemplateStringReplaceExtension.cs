using Moongate.Core.Instances;

namespace Moongate.Core.Extensions.Templates;

public static class TemplateStringReplaceExtension
{
    public static string ReplaceTemplate(this string template, object context = null)
    {
        return MoongateInstanceHolder.TemplateServiceService.TranslateText(template, context);
    }
}
