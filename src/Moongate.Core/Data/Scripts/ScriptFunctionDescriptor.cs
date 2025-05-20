namespace Moongate.Core.Data.Scripts;

public class ScriptFunctionDescriptor
{
    public string FunctionName { get; set; } = null!;
    public string? Help { get; set; }

    public List<ScriptFunctionParameterDescriptor> Parameters { get; set; } = new();
    public string ReturnType { get; set; }

    public Type RawReturnType { get; set; } = null!;

    public override string ToString()
    {
        return
            $"{FunctionName}({string.Join(", ", Parameters.Select(p => $"{p.ParameterType} {p.ParameterName}"))}) : {ReturnType}";
    }
}
