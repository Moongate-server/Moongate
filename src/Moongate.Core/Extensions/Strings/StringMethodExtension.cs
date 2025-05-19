using Moongate.Core.Text;

namespace Moongate.Core.Extensions.Strings;

/// <summary>
/// Provides extension methods for string operations, particularly for case conversions.
/// </summary>
public static class StringMethodExtension
{
    /// <summary>
    /// Converts a string to snake_case.
    /// </summary>
    /// <param name="text">The string to convert.</param>
    /// <returns>A snake_case version of the input string.</returns>
    public static string ToSnakeCase(this string text)
    {
        return StringUtils.ToSnakeCase(text);
    }

    /// <summary>
    /// Converts a string to UPPER_SNAKE_CASE.
    /// </summary>
    /// <param name="text">The string to convert.</param>
    /// <returns>An UPPER_SNAKE_CASE version of the input string.</returns>
    public static string ToSnakeCaseUpper(this string text)
    {
        return StringUtils.ToUpperSnakeCase(text);
    }

    /// <summary>
    /// Converts a string to camelCase.
    /// </summary>
    /// <param name="text">The string to convert.</param>
    /// <returns>A camelCase version of the input string.</returns>
    public static string ToCamelCase(this string text)
    {
        return StringUtils.ToCamelCase(text);
    }

    /// <summary>
    /// Converts a string to PascalCase.
    /// </summary>
    /// <param name="text">The string to convert.</param>
    /// <returns>A PascalCase version of the input string.</returns>
    public static string ToPascalCase(this string text)
    {
        return StringUtils.ToPascalCase(text);
    }

    /// <summary>
    /// Converts a string to kebab-case.
    /// </summary>
    /// <param name="text">The string to convert.</param>
    /// <returns>A kebab-case version of the input string.</returns>
    public static string ToKebabCase(this string text)
    {
        return StringUtils.ToKebabCase(text);
    }

    /// <summary>
    /// Converts a string to Title Case.
    /// </summary>
    /// <param name="text">The string to convert.</param>
    /// <returns>A Title Case version of the input string.</returns>
    public static string ToTitleCase(this string text)
    {
        return StringUtils.ToTitleCase(text);
    }
}
