using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class CodeHighlighter : MonoBehaviour
{
    private static string keywordHexColor = "#4C9CD6";
    private static string stringHexColor = "#BA9D85";
    private static string numericHexColor = "#B5CE7B";

    private static string[] csKeywords = {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const",
        "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern",
        "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface",
        "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
        "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc",
        "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked",
        "unsafe", "ushort", "using", "virtual", "void", "volatile", "while"
    };

    public static string CSSyntaxHighlight(string code)
    {
        code = CSHighlightKeywords(code);
        code = HighlightNumerics(code);
        code = HighlightStrings(code);

        return code;
    }

    private static string CSHighlightKeywords(string code)
    {
        string pattern = @"\b(?<![\w@])(" + string.Join("|", csKeywords) + @")(?![\w@])\b";

        return Regex.Replace(code, pattern, $"<color={keywordHexColor}>$&</color>");
    }

    private static string HighlightNumerics(string code)
    {
        string pattern = @"[+-]?\b\d+(\.\d+)?[dDfFbBlL]?\b";

        return Regex.Replace(code, pattern, $"<color={numericHexColor}>$&</color>");
    }

    private static string HighlightStrings(string code)
    {
        string pattern = "\"([^\"]*)\""; // Match content inside double quotes

        string result = Regex.Replace(code, pattern, match =>
        {
            string content = match.Groups[1].Value; // Get content within quotes
            string[] words = content.Split(' '); // Split words

            string parenthesizedWords = string.Join(" ", words.Select(word =>
            {
                word = word.Replace($"<color={keywordHexColor}>", "");
                word = word.Replace($"<color={numericHexColor}>", "");
                word = word.Replace($"</color>", "");

                return word;
            }));

            return $"<color={stringHexColor}>\"{parenthesizedWords}\"</color>";
        });

        return result;
    }
}
