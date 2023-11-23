using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Command : Attribute
{
    public string helpMessage;

    public Command()
    {
        this.helpMessage = "";
    }

    public Command(string helpMessage)
    {
        this.helpMessage = helpMessage;
    }
}

public class DebugConsole : MonoBehaviour
{
    private enum LogType
    {
        NORMAL, SUCCESS, ERROR, WARNING
    }

    public TextMeshProUGUI logText;
    public TMP_InputField commandInputField;

    [Header("Console Colors")]
    public Color normalColor;
    public Color successColor;
    public Color warningColor;
    public Color errorColor;

    private static DebugConsole instance;
    private SortedDictionary<string, MethodInfo> commands = new SortedDictionary<string, MethodInfo>();

    private string lastCommand = "";

    void Awake()
    {
        instance = this;
        commandInputField.onSubmit.AddListener(OnSubmit);
        Application.logMessageReceived += OnError;

        LoadCommands();
    }

    private void LoadCommands()
    {
        var methods = AppDomain.CurrentDomain.GetAssemblies()
                               .SelectMany(x => x.GetTypes())
                               .Where(x => x.IsClass)
                               .SelectMany(x => x.GetMethods())
                               .Where(x => x.GetCustomAttributes(typeof(Command), false).FirstOrDefault() != null);

        foreach (MethodInfo method in methods)
        {
            commands.Add(method.Name, method);
        }
    }

    [Command("Lists all the available commands. Syntax: Help (optional<command: string>)")]
    public void Help(string[] parameters)
    {
        StringBuilder sb = new StringBuilder("List of all available commands:");

        foreach (KeyValuePair<string, MethodInfo> entry in commands)
        {
            string commandName = entry.Key;
            string helpMessage = ((Command)entry.Value.GetCustomAttributes(typeof(Command), true)[0]).helpMessage;

            if (helpMessage != null)
            {
                sb.Append($"\n<b>{commandName}</b> -> {helpMessage}");
            }
            else
            {
                sb.Append($"\n<b>{commandName}</b>");
            }
        }

        Log(sb.ToString());
    }

    [Command()]
    public void ClearData(string[] parameters)
    {
        SecurePlayerPrefs.Init();
        SecurePlayerPrefs.SetString("csharp-name", "");
        SecurePlayerPrefs.SetInt("csharp-level", 1);
        SecurePlayerPrefs.SetInt("csharp-score", 0);
        SecurePlayerPrefs.SetInt("csharp-time", 0);
        SecurePlayerPrefs.SetInt("csharp-accuracy", 0);
        SecurePlayerPrefs.SetInt("csharp-stars", 0);


        SecurePlayerPrefs.SetString("java-name", "");
        SecurePlayerPrefs.SetInt("java-level", 1);
        SecurePlayerPrefs.SetInt("java-score", 0);
        SecurePlayerPrefs.SetInt("java-time", 0);
        SecurePlayerPrefs.SetInt("java-accuracy", 0);
        SecurePlayerPrefs.SetInt("java-stars", 0);

        SecurePlayerPrefs.Save();

        LanguageDatabase.GetInstance("java").ResetProgress();
        LanguageDatabase.GetInstance("csharp").ResetProgress();

        LogSuccess("Success");
    }

    [Command("Sets the value of a key. Syntax: SetPrefs <type: int, float, bool, string> <key: string> <value: type>")]
    public void SetPrefs(string[] parameters)
    {
        if (parameters.Length != 4)
        {
            LogError("Error. Missing 3 arguments.");
            return;
        }

        switch (parameters[1])
        {
            case "float":
                if (float.TryParse(parameters[3], out float fValue))
                {
                    SecurePlayerPrefs.SetFloat(parameters[2], fValue);
                    LogSuccess("Set value success.");
                }
                else
                {
                    LogError("Invalid float value.");
                }
                break;
            case "bool":
                if (bool.TryParse(parameters[3], out bool bValue))
                {
                    SecurePlayerPrefs.SetBool(parameters[2], bValue);
                    LogSuccess("Set value success.");
                }
                else
                {
                    LogError("Invalid float value.");
                }
                break;
            case "int":
                if (int.TryParse(parameters[3], out int iValue))
                {
                    SecurePlayerPrefs.SetInt(parameters[2], iValue);
                    LogSuccess("Set value success.");
                }
                else
                {
                    LogError("Invalid float value.");
                }
                break;
            case "string":
                SecurePlayerPrefs.SetString(parameters[2], parameters[3]);
                LogSuccess("Set value success.");
                break;
            default:
                LogError($"Invalid type. \"{parameters[3]}\"");
                break;
        }

    }

    [Command("Gets the value of a key. Syntax: GetPrefs <type: int, float, bool, string> <key: type>")]
    public void GetPrefs(string[] parameters)
    {
        if (parameters.Length != 3)
        {
            LogError("Error. Missing 2 argument.");
            return;
        }

        if (!SecurePlayerPrefs.HasKey(parameters[2]))
        {
            LogError("Invalid key.");
            return;
        }

        switch (parameters[1])
        {
            case "float":
                Log(SecurePlayerPrefs.GetFloat(parameters[2]));
                break;
            case "bool":
                Log(SecurePlayerPrefs.GetBool(parameters[2]));
                break;
            case "int":
                Log(SecurePlayerPrefs.GetInt(parameters[2]));
                break;
            case "string":
                Log(SecurePlayerPrefs.GetString(parameters[2]));
                break;
            default:
                LogError($"Invalid type. \"{parameters[3]}\"");
                break;
        }
    }

    [Command("Sets the speed of time of the game. Syntax: SetTimeScale <speed: float(0, 100)>")]
    public void SetTimeScale(string[] parameters)
    {
        if (parameters.Length == 1)
        {
            LogError("Error. Missing 1 argument with float type.");
            return;
        }

        if (float.TryParse(parameters[1], out float timeScale))
        {
            Time.timeScale = timeScale;
            LogSuccess($"Timescale set to {timeScale}.");
        }
        else
        {
            LogError($"Error. Invalid argument ({parameters[1]}). It should be a type of float.");
        }
    }

    private void OnSubmit(string input)
    {
        commandInputField.text = "";
        Focus();

        if (string.IsNullOrWhiteSpace(input))
            return;

        Log($"> {input}");
        lastCommand = input;
        HandleCommand(input);
    }

    private void HandleCommand(string input)
    {
        string command = input.Split().First();

        if (commands.TryGetValue(command, out MethodInfo method))
        {
            object[] parameters = new object[] { input.Split() };
            method?.Invoke(this, parameters);
        }
    }

    private void AppendLog(LogType logType, params object[] messages)
    {
        string message = string.Join(' ', messages);

        switch (logType)
        {
            case LogType.NORMAL:
                message = $"<color=#{normalColor.ToHexString()}>{message}</color>";
                break;

            case LogType.SUCCESS:
                message = $"<color=#{successColor.ToHexString()}>{message}</color>";
                break;

            case LogType.ERROR:
                message = $"<color=#{errorColor.ToHexString()}>{message}</color>";
                break;

            case LogType.WARNING:
                message = $"<color=#{warningColor.ToHexString()}>{message}</color>";
                break;
        }

        logText.text += $"\n{message}";
    }

    public static void Log(params object[] messages)
    {
        if (instance == null)
            return;

        instance.AppendLog(LogType.NORMAL, messages);
    }

    public static void LogSuccess(params object[] messages)
    {
        if (instance == null)
            return;

        instance.AppendLog(LogType.SUCCESS, messages);
    }

    public static void LogWarning(params object[] messages)
    {
        if (instance == null)
            return;

        instance.AppendLog(LogType.WARNING, messages);
    }

    public static void LogError(params object[] messages)
    {
        if (instance == null)
            return;

        instance.AppendLog(LogType.ERROR, messages);
    }

    private void Focus()
    {
        commandInputField.Select();
        commandInputField.ActivateInputField();
    }

    void OnError(string condition, string stackTrace, UnityEngine.LogType logType)
    {
        switch (logType)
        {
            case UnityEngine.LogType.Log:
                Log(condition);
                break;

            case UnityEngine.LogType.Error:
            case UnityEngine.LogType.Assert:
            case UnityEngine.LogType.Exception:
                LogError(condition);
                LogError(stackTrace);
                break;

            case UnityEngine.LogType.Warning:
                LogWarning(condition);
                break;
        }
    }

    void OnEnable()
    {
        Focus();
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= OnError;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            commandInputField.SetTextWithoutNotify(lastCommand);
        }
    }
}
