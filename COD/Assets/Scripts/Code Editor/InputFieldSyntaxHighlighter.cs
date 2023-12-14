using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public enum Language
{
    CS, Java
}

public class InputFieldSyntaxHighlighter : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI inputText;
    public TextMeshProUGUI displayText;
    public Language language;

    private static string keywordHexColor = "#4C9CD6";
    private static string stringHexColor = "#BA9D85";
    private static string numericHexColor = "#B5CE7B";
    private static string commentHexColor = "#57a648";

    private static string[] javaKeywords =
    {
        "abstract", "assert", "boolean", "break", "byte", "case", "catch", "char", "class", "const",
        "continue", "default", "do", "double", "else", "enum", "extends", "final", "finally", "float",
        "for", "if", "goto", "implements", "import", "instanceof", "int", "interface", "long", "native",
        "new", "package", "private", "protected", "public", "return", "short", "static", "strictfp",
        "super", "switch", "synchronized", "this", "throw", "throws", "transient", "try", "void", "volatile", "while"
    };

    private static string[] csKeywords = 
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const",
        "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern",
        "false", "finally", "fixed", "float", "for", "foreach", "get", "goto", "if", "implicit", "in", "int", "interface",
        "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
        "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "set", "short", "sizeof", "stackalloc",
        "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked",
        "unsafe", "ushort", "using", "virtual", "void", "volatile", "while"
    };

    void Awake()
    {
        inputField.onValueChanged.AddListener((s) => SyntaxHighlight(s));
        SyntaxHighlight(inputField.text);
    }

    public void SyntaxHighlight(string text)
    {
        text = HighlightKeywords(text);
        text = HighlightNumerics(text);
        text = HighlightComments(text);
        text = HighlightStrings(text);

        displayText.text = text;
    }

    private string HighlightKeywords(string text)
    {
        string pattern = @"\b(?<![\w@])(" + string.Join("|", language == Language.CS ? csKeywords : javaKeywords) + @")(?![\w@])\b";

        return Regex.Replace(text, pattern, $"<color={keywordHexColor}>$&</color>");
    }

    private string HighlightNumerics(string text)
    {
        string pattern = @"[+-]?\b\d+(\.\d+)?[dDfFbBlLmM]?\b";

        return Regex.Replace(text, pattern, $"<color={numericHexColor}>$&</color>");
    }

    private string HighlightComments(string text)
    {
        string pattern = "//(.*?)(\n|$)";

        text = Regex.Replace(text, pattern, match =>
        {
            string capturedText = match.Groups[1].Value;
            return $"<color={commentHexColor}>//{capturedText}\n</color>";
        });

        return text;
    }

    private string HighlightStrings(string text)
    {
        string pattern = "\"([^\"]*)\""; // Match content inside double quotes

        string result = Regex.Replace(text, pattern, match =>
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
