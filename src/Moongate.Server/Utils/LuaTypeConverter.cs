using NLua;

namespace Moongate.Server.Utils;

/// <summary>
/// Utility class for converting .NET types to Lua type definitions and handling type conversions
/// </summary>
public static class LuaTypeConverter
{
    /// <summary>
    /// Converts a LuaTable to a Dictionary for easier manipulation
    /// </summary>
    /// <param name="luaTable">The LuaTable to convert</param>
    /// <returns>A dictionary representation of the LuaTable</returns>
    public static Dictionary<string, object> LuaTableToDictionary(LuaTable luaTable)
    {
        var dict = new Dictionary<string, object>();

        foreach (var key in luaTable.Keys)
        {
            dict[key.ToString()] = luaTable[key];

            if (luaTable[key] is LuaTable table)
            {
                dict[key.ToString()] = LuaTableToDictionary(table);
            }
        }

        return dict;
    }

    /// <summary>
    /// Gets the Lua type representation for a .NET Type
    /// </summary>
    /// <param name="type">The .NET Type to convert</param>
    /// <returns>The Lua type representation as string</returns>
    public static string GetLuaType(Type type)
    {
        if (type == null)
        {
            return "nil";
        }

        // Handle Action types with parameters
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Action<>))
        {
            var genericArgs = type.GetGenericArguments();
            var paramTypes = string.Join(", ", genericArgs.Select(GetLuaType));
            return $"fun({paramTypes})";
        }

        // Handle Action without parameters
        if (type == typeof(Action))
        {
            return "function";
        }

        // Handle Func<> types
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<,>))
        {
            var genericArgs = type.GetGenericArguments();
            // For Func<T1, TResult>, create fun(T1): TResult format
            return $"fun({GetLuaType(genericArgs[0])}): {GetLuaType(genericArgs[1])}";
        }

        // Handle multi-parameter Func types
        if (type.IsGenericType && type.Name.StartsWith("Func`"))
        {
            var genericArgs = type.GetGenericArguments();
            var paramTypes = genericArgs.Take(genericArgs.Length - 1).Select(GetLuaType);
            var returnType = GetLuaType(genericArgs.Last());
            return $"fun({string.Join(", ", paramTypes)}): {returnType}";
        }

        return type.Name switch
        {
            "String"                                                            => "string",
            "String[]"                                                          => "string[]",
            "Int32" or "Int64" or "Single" or "Double" or "Decimal" or "Single" => "number",
            "Boolean"                                                           => "boolean",
            "Void"                                                              => "nil",
            "Object"                                                            => "any",
            "LuaFunction"                                                       => "function",
            "Action"                                                            => "function",
            "LuaTable"                                                          => "table",
            var name when type.IsArray                                          => $"{GetLuaType(type.GetElementType())}[]",
            var name when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)
                => $"{GetLuaType(type.GetGenericArguments()[0])}[]",
            var name when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>)
                => "table",
            _ => type.Name
        };
    }

    /// <summary>
    /// Gets a detailed Lua type representation with additional information
    /// </summary>
    /// <param name="type">The .NET Type to convert</param>
    /// <returns>A detailed Lua type representation</returns>
    public static string GetDetailedLuaType(Type type)
    {
        // Handle Action types with better descriptions
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Action<>))
        {
            var genericArgs = type.GetGenericArguments();
            if (genericArgs.Length == 1 && genericArgs[0] == typeof(string[]))
            {
                return "fun(args: string[])";
            }

            var paramTypes = genericArgs.Select((arg, index) => $"param{index + 1}: {GetLuaType(arg)}");
            return $"fun({string.Join(", ", paramTypes)})";
        }

        // Handle Action without parameters
        if (type == typeof(Action))
        {
            return "function";
        }

        // Handle Func<> types with detailed argument information
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<,>))
        {
            var genericArgs = type.GetGenericArguments();
            return $"fun(param1: {GetLuaType(genericArgs[0])}): {GetLuaType(genericArgs[1])}";
        }

        // Handle multi-parameter Func types
        if (type.IsGenericType && type.Name.StartsWith("Func`"))
        {
            var genericArgs = type.GetGenericArguments();
            var paramTypes = genericArgs.Take(genericArgs.Length - 1)
                .Select((arg, index) => $"param{index + 1}: {GetLuaType(arg)}");
            var returnType = GetLuaType(genericArgs.Last());
            return $"fun({string.Join(", ", paramTypes)}): {returnType}";
        }

        if (type.IsGenericType)
        {
            var genericType = type.GetGenericTypeDefinition();
            if (genericType == typeof(Dictionary<,>))
            {
                var keyType = GetLuaType(type.GetGenericArguments()[0]);
                var valueType = GetLuaType(type.GetGenericArguments()[1]);
                return $"table<{keyType}, {valueType}>";
            }
            else if (genericType == typeof(List<>) || genericType == typeof(IEnumerable<>))
            {
                var elementType = GetLuaType(type.GetGenericArguments()[0]);
                return $"{elementType}[]";
            }
        }

        if (type.IsEnum)
        {
            var values = string.Join("|", Enum.GetNames(type));
            return $"string # One of: {values}";
        }

        return GetLuaType(type);
    }

    /// <summary>
    /// Gets the Lua type for a runtime object value
    /// </summary>
    /// <param name="value">The object value to analyze</param>
    /// <returns>The Lua type representation</returns>
    public static string GetLuaType(object value)
    {
        if (value == null)
        {
            return "nil";
        }

        return value switch
        {
            string                                              => "string",
            int or long or float or double or decimal or Single => "number",
            bool                                                => "boolean",
            LuaFunction                                         => "function",
            LuaTable                                            => "table",
            System.Collections.IEnumerable                      => "table",
            Delegate d when d.GetType().IsGenericType &&
                            d.GetType().GetGenericTypeDefinition() == typeof(Func<,>) =>
                GetLuaType(d.GetType()),
            Delegate d when d.GetType().IsGenericType &&
                            d.GetType().GetGenericTypeDefinition() == typeof(Action<>) =>
                GetLuaType(d.GetType()),
            Action => "function",
            _      => value.GetType().Name
        };
    }

    /// <summary>
    /// Gets a user-friendly parameter name for Lua function definitions
    /// </summary>
    /// <param name="parameterName">The original parameter name</param>
    /// <param name="parameterType">The parameter type</param>
    /// <returns>A Lua-friendly parameter name</returns>
    public static string GetLuaParameterName(string parameterName, Type parameterType)
    {
        // Handle reserved Lua keywords
        if (parameterName == "function")
        {
            return "fn";
        }

        // For Action<string[]>, make it more descriptive
        if (parameterType.IsGenericType &&
            parameterType.GetGenericTypeDefinition() == typeof(Action<>) &&
            parameterType.GetGenericArguments()[0] == typeof(string[]))
        {
            return "handler";
        }

        return parameterName;
    }
}
