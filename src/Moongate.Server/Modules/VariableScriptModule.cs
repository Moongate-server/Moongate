using Moongate.Core.Attributes.Scripts;
using Orion.Core.Server.Interfaces.Services.System;

namespace Moongate.Server.Modules;

[ScriptModule("template")]
public class VariableScriptModule
{
    private readonly ITextTemplateService _textTemplateService;

    public VariableScriptModule(ITextTemplateService textTemplateService)
    {
        _textTemplateService = textTemplateService;
    }

    [ScriptFunction("add_variable", "Add Variable to the text template service and you can find by use {{name}}")]
    public void AddVariable(string name, object value)
    {
        _textTemplateService.AddVariable(name, value);
    }

    [ScriptFunction(
        "add_variabile_builder",
        "Add Variable Builder to the text template service and you can find by use {{name}}"
    )]
    public void AddVariableBuilder(string name, Func<object> builder)
    {
        _textTemplateService.AddVariableBuilder(name, builder);
    }


    [ScriptFunction("translate", "Replaces the text with the variables")]
    public string TranslateText(string text, object context = null)
    {
        return _textTemplateService.TranslateText(text, context);
    }

    [ScriptFunction("get_all", " Get all variables")]
    public List<string> GetVariables()
    {
        return _textTemplateService.GetVariables();
    }
}
