using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class CSSyntaxHighlighter : MonoBehaviour
{
    private TextMeshProUGUI codeText;

    private static string keywordHexColor = "#4C9CD6";
    private static string stringHexColor = "#BA9D85";
    private static string numericHexColor = "#B5CE7B";

    private static string[] keywords = {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const",
        "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern",
        "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface",
        "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
        "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc",
        "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked",
        "unsafe", "ushort", "using", "virtual", "void", "volatile", "while"
    };

    void Awake()
    {
        codeText = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        SyntaxHighlight();
    }

    public void SyntaxHighlight()
    {
        HighlightKeywords();
        HighlightNumerics();
        HighlightStrings();
    }

    private void HighlightKeywords()
    {
        string pattern = @"\b(?<![\w@])(" + string.Join("|", keywords) + @")(?![\w@])\b";

        codeText.text = Regex.Replace(codeText.text, pattern, $"<color={keywordHexColor}>$&</color>");
    }

    private void HighlightNumerics()
    {
        string pattern = @"[+-]?\b\d+(\.\d+)?[dDfFbBlL]?\b";

        codeText.text = Regex.Replace(codeText.text, pattern, $"<color={numericHexColor}>$&</color>");
    }

    private void HighlightStrings()
    {
        string pattern = "\"([^\"]*)\""; // Match content inside double quotes

        string result = Regex.Replace(codeText.text, pattern, match =>
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

        codeText.text = result;
    }
}

