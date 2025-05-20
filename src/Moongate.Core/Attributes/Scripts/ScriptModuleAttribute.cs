namespace Moongate.Core.Attributes.Scripts;

[AttributeUsage(AttributeTargets.Class)]
public class ScriptModuleAttribute : Attribute
{
    public string TableName { get; }

    public ScriptModuleAttribute(string tableName)
    {
        TableName = tableName;
    }

}
