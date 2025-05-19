using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Moongate.Core.Text;

/// <summary>
/// Provides utility methods for string operations, including various case conversion methods.
/// </summary>
public static partial class StringUtils
{

    [GeneratedRegex(@"[\s_-]|(?<=[a-z])(?=[A-Z])", RegexOptions.Compiled)]
    private static partial Regex WordSplitter();

    private static readonly Regex WordSplitterRegex = WordSplitter();

    /// <summary>
    /// Converts a string from camelCase or PascalCase to snake_case.
    /// </summary>
    /// <param name="text">The string to convert to snake_case.</param>
    /// <returns>A snake_case version of the input string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input text is null or empty.</exception>
    /// <example>
    /// "HelloWorld" becomes "hello_world"
    /// "APIResponse" becomes "api_response"
    /// "userId" becomes "user_id"
    /// </example>
    public static string ToSnakeCase(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentNullException(nameof(text));
        }

        if (text.Length < 2)
        {
            return text.ToLowerInvariant();
        }

        text = text.Replace('-', '_');

        var sb = new StringBuilder();
        sb.Append(char.ToLowerInvariant(text[0]));

        for (var i = 1; i < text.Length; ++i)
        {
            var c = text[i];
            var prev = text[i - 1];

            if (char.IsUpper(c))
            {
                if (!char.IsUpper(prev) || (i + 1 < text.Length && !char.IsUpper(text[i + 1])))
                {
                    sb.Append('_');
                }
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converts a string to camelCase.
    /// </summary>
    /// <param name="text">The string to convert to camelCase.</param>
    /// <returns>A camelCase version of the input string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input text is null or empty.</exception>
    /// <example>
    /// "HelloWorld" becomes "helloWorld"
    /// "API_RESPONSE" becomes "apiResponse"
    /// "user-id" becomes "userId"
    /// </example>
    public static string ToCamelCase(string text)
    {
        if (string.IsNullOrEmpty(text)) throw new ArgumentNullException(nameof(text));

        if (text.Length < 2) return text.ToLowerInvariant();

        var words = WordSplitterRegex.Split(text);
        var result = new StringBuilder(words[0].ToLowerInvariant());

        for (int i = 1; i < words.Length; i++)
        {
            if (string.IsNullOrEmpty(words[i])) continue;

            result.Append(CultureInfo.InvariantCulture.TextInfo.ToTitleCase(words[i].ToLowerInvariant()));
        }

        return result.ToString();
    }

    /// <summary>
    /// Converts a string to PascalCase.
    /// </summary>
    /// <param name="text">The string to convert to PascalCase.</param>
    /// <returns>A PascalCase version of the input string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input text is null or empty.</exception>
    /// <example>
    /// "hello_world" becomes "HelloWorld"
    /// "api-response" becomes "ApiResponse"
    /// "userId" becomes "UserId"
    /// </example>
    public static string ToPascalCase(string text)
    {
        if (string.IsNullOrEmpty(text)) throw new ArgumentNullException(nameof(text));

        if (text.Length < 2) return text.ToUpperInvariant();

        var words = WordSplitterRegex.Split(text);
        var result = new StringBuilder();

        foreach (var word in words)
        {
            if (string.IsNullOrEmpty(word)) continue;

            result.Append(CultureInfo.InvariantCulture.TextInfo.ToTitleCase(word.ToLowerInvariant()));
        }

        return result.ToString();
    }

    /// <summary>
    /// Converts a string to kebab-case.
    /// </summary>
    /// <param name="text">The string to convert to kebab-case.</param>
    /// <returns>A kebab-case version of the input string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input text is null or empty.</exception>
    /// <example>
    /// "HelloWorld" becomes "hello-world"
    /// "API_RESPONSE" becomes "api-response"
    /// "userId" becomes "user-id"
    /// </example>
    public static string ToKebabCase(string text)
    {
        if (string.IsNullOrEmpty(text)) throw new ArgumentNullException(nameof(text));

        if (text.Length < 2) return text.ToLowerInvariant();

        var words = WordSplitterRegex.Split(text);
        var result = new StringBuilder();

        bool isFirst = true;
        foreach (var word in words)
        {
            if (string.IsNullOrEmpty(word)) continue;

            if (!isFirst)
            {
                result.Append('-');
            }

            result.Append(word.ToLowerInvariant());
            isFirst = false;
        }

        return result.ToString();
    }

    /// <summary>
    /// Converts a string to UPPER_SNAKE_CASE (screaming snake case).
    /// </summary>
    /// <param name="text">The string to convert to UPPER_SNAKE_CASE.</param>
    /// <returns>An UPPER_SNAKE_CASE version of the input string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input text is null or empty.</exception>
    /// <example>
    /// "HelloWorld" becomes "HELLO_WORLD"
    /// "apiResponse" becomes "API_RESPONSE"
    /// "user-id" becomes "USER_ID"
    /// </example>
    public static string ToUpperSnakeCase(string text)
    {
        return ToSnakeCase(text).ToUpperInvariant();
    }

    /// <summary>
    /// Converts a string to Title Case.
    /// </summary>
    /// <param name="text">The string to convert to Title Case.</param>
    /// <returns>A Title Case version of the input string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input text is null or empty.</exception>
    /// <example>
    /// "hello_world" becomes "Hello World"
    /// "API_RESPONSE" becomes "Api Response"
    /// "user-id" becomes "User Id"
    /// </example>
    public static string ToTitleCase(string text)
    {
        if (string.IsNullOrEmpty(text)) throw new ArgumentNullException(nameof(text));

        var words = WordSplitterRegex.Split(text);
        var result = new StringBuilder();

        bool isFirst = true;
        foreach (var word in words)
        {
            if (string.IsNullOrEmpty(word)) continue;

            if (!isFirst)
            {
                result.Append(' ');
            }

            result.Append(CultureInfo.InvariantCulture.TextInfo.ToTitleCase(word.ToLowerInvariant()));
            isFirst = false;
        }

        return result.ToString();
    }


}
