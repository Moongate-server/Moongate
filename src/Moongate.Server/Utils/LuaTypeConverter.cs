using NLua;
using LuaFunction = KeraLua.LuaFunction;

namespace Moongate.Server.Utils;

public static class LuaTypeConverter
{
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

    public static string GetLuaType(Type type)
    {
        if (type == null)
        {
            return "nil";
        }

        // Handle Func<> types
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<,>))
        {
            var genericArgs = type.GetGenericArguments();
            // For Func<T1, TResult>, create fun(T1): TResult format
            return $"fun({GetLuaType(genericArgs[0])}): {GetLuaType(genericArgs[1])}";
        }

        return type.Name switch
        {
            "String"                                                            => "string",
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

    public static string GetDetailedLuaType(Type type)
    {
        // Handle Func<> types with detailed argument information
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<,>))
        {
            var genericArgs = type.GetGenericArguments();
            return $"fun(context: {GetLuaType(genericArgs[0])}): {GetLuaType(genericArgs[1])}";
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
            _ => value.GetType().Name
        };
    }
}
