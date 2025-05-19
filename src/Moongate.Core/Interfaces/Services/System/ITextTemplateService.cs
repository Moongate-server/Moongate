namespace Orion.Core.Server.Interfaces.Services.System;

public interface ITextTemplateService
{
    public void AddVariableBuilder(string variableName, Func<object> builder);

    public void AddVariable(string variableName, object value);

    string TranslateText(string text, object context = null);

    Dictionary<string, string> GetVariablesAndContent();

    List<string> GetVariables();

    void RebuildVariables();
}
