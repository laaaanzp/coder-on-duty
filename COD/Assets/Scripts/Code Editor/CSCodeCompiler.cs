using UnityEngine;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using System.Diagnostics;
using System.Threading.Tasks;
using System.ComponentModel;

public class CSCodeCompiler : MonoBehaviour
{
    public TMP_InputField codeInputField;
    public ModalControl consoleModalControl;
    public TypeChooser typeChooser;

    public TextMeshProUGUI outputTMPText;
    public int outputMaxDisplayLine = 1000;

    [Header("Console Colors")]
    public Color errorColor;

    private Process process;
    private bool isProcessing;
    private bool justStarted;

    private int currentLineCount;


    #region fileName fix for editor and actual build
#if UNITY_EDITOR
    private string csCompilerPath = "C:\\Users\\peloe\\OneDrive\\Desktop\\Capstone\\build\\Compilers\\C#\\CSCodeCompiler.exe";
    private string javaCompilerPath = "C:\\Users\\peloe\\OneDrive\\Desktop\\Capstone\\build\\Compilers\\Java\\JavaCodeCompiler.exe";
#else
    private string csCompilerPath = $"{Path.GetDirectoryName(Application.dataPath)}/Compilers/C#/CSCodeCompiler.exe";
    private string javaCompilerPath = $"{Path.GetDirectoryName(Application.dataPath)}/Compilers/Java/JavaCodeCompiler.exe";
#endif
    #endregion

    public async void ExecuteCode()
    {
        outputTMPText.text = "Compiling...";

        justStarted = true;
        await CompileAndExecuteViaOutsideProcess();

        // CompileAndExecuteCode();
    }

    public void StopCurrentExecution()
    {
        try
        {
            isProcessing = false;
            process?.Kill();
            process?.Close();
        }
        catch { }
    }

    public void CopyCode()
    {
        GUIUtility.systemCopyBuffer = codeInputField.text;
    }

    async Task CompileAndExecuteViaOutsideProcess()
    {
        await Task.Run(() =>
        {
            isProcessing = true;
            string code = codeInputField.text.Replace("\n", "###NEWLINE###").Replace("\"", "###QUOTE###").Replace("\\", "###BACKSLASH###");
            process = new Process();
            process.StartInfo.FileName = csCompilerPath;
            process.StartInfo.Arguments = $"\"{code}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            StreamReader reader = process.StandardOutput;
            StreamReader errorReader = process.StandardError;

            string outputLine;
            string errorLine;

            while (isProcessing)
            {
                isProcessing = !process.HasExited;
                while ((outputLine = reader.ReadLine()) != null)
                {
                    AppendConsoleOutput(outputLine);
                }

                while ((errorLine = errorReader.ReadLine()) != null)
                {
                    AppendConsoleOutput($"<color=#{errorColor.ToHexString()}>{errorLine}</color>");
                }
            }

            AppendConsoleOutput("\n<b>Execution finished...</b>");

            process.Close();
            isProcessing = false;
        });
    }
 
    private void AppendConsoleOutput(string newOutput)
    {
        MultithreadControl.RunOnMainThread(() =>
        {
            if (justStarted)
            {
                outputTMPText.text = "";
                justStarted = false;
                currentLineCount = 0;
            }

            if (currentLineCount > outputMaxDisplayLine)
            {
                string[] lines = outputTMPText.text.Split("\n");
                outputTMPText.text = string.Join("\n", lines, 1, lines.Length - 1);
            }

            outputTMPText.text += newOutput + "\n";
            currentLineCount++;
        });
    }

    public void Templates()
    {
        typeChooser.Open("C#", SetCode);
    }

    public void SetCode(string code)
    {
        codeInputField.text = code;
    }
}
