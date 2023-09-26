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

    [Command("Lists all the available commands.")]
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

    [Command("Sets the speed of time of the game. (Min value: 0, Max value: 100)")]
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

        if (input.Replace(" ", "") == "")
            return;

        Log($"> {input}");
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
}
