
namespace Moongate.Core.Data.Events.TextTemplate;

public record AddVariableBuilderEvent(string VariableName, Func<object> Builder);
