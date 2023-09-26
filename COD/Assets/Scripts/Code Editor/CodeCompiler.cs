using UnityEngine;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using System.Diagnostics;
using System.Threading.Tasks;
using System.ComponentModel;

public class CodeCompiler : MonoBehaviour
{
    public TMP_InputField codeInputField;
    public ModalControl consoleModalControl;

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
    private string fileName = "C:\\Users\\Kurumi\\Desktop\\Test\\Compilers\\C#\\CSCompiler.exe";
#else
    private string fileName = $"{Path.GetDirectoryName(Application.dataPath)}/Compilers/C#/CSCompiler.exe";
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
            string code = codeInputField.text.Replace("\"", "\\\"");
            process = new Process();
            process.StartInfo.FileName = fileName;
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

    /*private void CompileAndExecuteCode()
    {
        Destroy(outputTextInstance);
        outputTextInstance = Instantiate(outputTextGameObject, outputTextParent);
        TextMeshProUGUI outputText = outputTextInstance.GetComponent<TextMeshProUGUI>();

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeInputText.text);
        string assemblyName = Path.GetRandomFileName();

        MetadataReference[] references = new MetadataReference[]
        {
            MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location),
        };

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName,
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        using (MemoryStream ms = new MemoryStream())
        {
            EmitResult result = compilation.Emit(ms);

            if (!result.Success)
            {
                IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                StringBuilder sb = new StringBuilder();
                foreach (Diagnostic failure in failures)
                {
                    FileLinePositionSpan fileLinePositionSpan = failure.Location.GetMappedLineSpan();

                    int lineNumber = fileLinePositionSpan.StartLinePosition.Line;
                    int columnStart = fileLinePositionSpan.StartLinePosition.Character;
                    int columnEnd = fileLinePositionSpan.EndLinePosition.Character;

                    string line = codeInputText.text.Split('\n')[lineNumber];
                    line = line.Insert(columnEnd, "</u>");
                    line = line.Insert(columnStart, "<u>");

                    int codeFailureLineNumber = failure.Location.GetLineSpan().StartLinePosition.Line;
                    string codeFailureLine = codeInputText.text.Split('\n')[codeFailureLineNumber];

                    sb.AppendLine($"Error at (Line {lineNumber}, Column {columnStart})");
                    sb.AppendLine($"> {line.TrimStart()}");
                    sb.AppendLine($"<color=#{errorColor.ToHexString()}>error {failure.Id}: {failure.GetMessage()}</color>\n");
                }

                outputText.text = sb.ToString();

                return;
            }

            // Load the assembly into memory
            ms.Seek(0, SeekOrigin.Begin);
            Assembly assembly = Assembly.Load(ms.ToArray());
            
            // Find and execute the Main method
            Type programType = assembly
                .GetTypes()
                .FirstOrDefault(t => t.GetMethod("Main", BindingFlags.Static | BindingFlags.Public) != null);

            if (programType == null)
            {
                outputText.text = $"<color=#{errorColor.ToHexString()}>error CS5001: Program does not contain a public static 'Main' method suitable for an entry point</color>";
                return;
            }
            
            StringWriter outputWriter = new StringWriter();
            Console.SetOut(outputWriter);
            
            MethodInfo mainMethod = programType.GetMethod("Main", BindingFlags.Static | BindingFlags.Public);
            mainMethod.Invoke(null, new object[] { new string[] { } });
            outputText.text = outputWriter.ToString();
        }
    }*/
}
