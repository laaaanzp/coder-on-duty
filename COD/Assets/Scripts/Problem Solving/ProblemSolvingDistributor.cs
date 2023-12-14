using System;
using System.Collections.Generic;
using UnityEngine;

public class ProblemSolvingDistributor : MonoBehaviour
{
    [SerializeField] private DragAndArrangePuzzle dragAndArrangePuzzle;
    [SerializeField] private DragAndDropPuzzle dragAndDropPuzzle;
    [SerializeField] private FindAndSelect findAndSelectPuzzle;
    [SerializeField] private Crossword crosswordPuzzle;
    [SerializeField] private MinigameDevice minigamesElectronicDevice;

    [Header("Training Computers - Level 1")]
    [SerializeField] private ElectronicDeviceTraining[] trainingComputers;
    public static List<IPuzzle> puzzleSolvings;
    private GameObject[] computerObjects;

    void Start()
    {
        puzzleSolvings = new List<IPuzzle>();

        computerObjects = GameObject.FindGameObjectsWithTag("Computer");
        computerObjects = Tools.ShuffleArray(computerObjects);

        if (DatabaseManager.instance.currentLanguage.currentLevel == 1)
        {
            DistributeDragAndDrop(trainingComputers[0]);
            DistributeDragAndArrange(trainingComputers[1]);
            DistributeFindAndSelect(trainingComputers[2]);
        }
        else if (DatabaseManager.instance.currentLanguage.currentLevel == 2)
        {
            DistributeDragAndDrop(computerObjects[0].GetComponent<ElectronicDevice>());
            DistributeDragAndArrange(computerObjects[1].GetComponent<ElectronicDevice>());
            DistributeFindAndSelect(computerObjects[2].GetComponent<ElectronicDevice>());
        }
        else if (DatabaseManager.instance.currentLanguage.currentLevel == 3)
        {
            DistributeDragAndDrop(computerObjects[0].GetComponent<ElectronicDevice>());
            DistributeDragAndArrange(computerObjects[1].GetComponent<ElectronicDevice>());
            DistributeFindAndSelect(computerObjects[2].GetComponent<ElectronicDevice>());
        }
        else 
        {
            DistributeDragAndDrop(computerObjects[0].GetComponent<ElectronicDevice>());
            DistributeDragAndArrange(computerObjects[1].GetComponent<ElectronicDevice>());
            DistributeFindAndSelect(computerObjects[2].GetComponent<ElectronicDevice>());
            DistributeCrossword(minigamesElectronicDevice);
        }

        InitializeDevices();
    }

    void DistributeCrossword(MinigameDevice minigameDevice)
    {
        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        string levelName = DatabaseManager.instance.currentLanguage.currentLevelName;
        TextAsset problemTextAsset = Resources.Load<TextAsset>($"Problems/{languageName}/{levelName}/crossword");

        string[] problemSplitted = problemTextAsset.text.Split("####################");
        string crosswordText = problemSplitted[0];
        string crosswordClues = problemSplitted[1];

        crosswordPuzzle.Initialize(crosswordText, crosswordClues);

        minigameDevice.puzzleModal = crosswordPuzzle;
    }

    void DistributeDragAndDrop(ElectronicDevice electronicDevice)
    {
        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        string levelName = DatabaseManager.instance.currentLanguage.currentLevelName;
        TextAsset problemTextAsset = Resources.Load<TextAsset>($"Problems/{languageName}/{levelName}/dragAndDrop");

        string[] problemSplitted = problemTextAsset.text.Split("##############################");
        
        string[] answers = Tools.ShuffleArray(problemSplitted[0].Split('\n'));
        string problem = problemSplitted[1].Trim();
        string output = "";

        try { output = problemSplitted[2].Trim(); }
        catch { }

        dragAndDropPuzzle.Initialize(problem, answers, output);

        puzzleSolvings.Add(dragAndDropPuzzle);
        electronicDevice.puzzleModal = dragAndDropPuzzle;
    }

    void DistributeDragAndArrange(ElectronicDevice electronicDevice)
    {
        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        string levelName = DatabaseManager.instance.currentLanguage.currentLevelName;
        TextAsset problemTextAsset = Resources.Load<TextAsset>($"Problems/{languageName}/{levelName}/dragAndArrange");

        string[] problemSplitted = problemTextAsset.text.Split("##############################");

        string problem = problemSplitted[0].Trim();
        string[] lines = problem.Split('\n');
        string output = "";

        try { output = problemSplitted[1].Trim(); }
        catch { }

        dragAndArrangePuzzle.Initialize(lines, output);

        puzzleSolvings.Add(dragAndArrangePuzzle);
        electronicDevice.puzzleModal = dragAndArrangePuzzle;
    }

    void DistributeFindAndSelect(ElectronicDevice electronicDevice)
    {
        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        string levelName = DatabaseManager.instance.currentLanguage.currentLevelName;
        TextAsset problemTextAsset = Resources.Load<TextAsset>($"Problems/{languageName}/{levelName}/findAndSelect");

        string[] lines = problemTextAsset.text.Split('\n');

        findAndSelectPuzzle.Initialize(lines);

        puzzleSolvings.Add(findAndSelectPuzzle);
        electronicDevice.puzzleModal = findAndSelectPuzzle;
    }

    void DistributeDragAndDrop(ElectronicDeviceTraining electronicDevice)
    {
        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        string levelName = DatabaseManager.instance.currentLanguage.currentLevelName;
        TextAsset problemTextAsset = Resources.Load<TextAsset>($"Problems/{languageName}/{levelName}/dragAndDrop");

        string[] problemSplitted = problemTextAsset.text.Split("##############################");

        string[] answers = Tools.ShuffleArray(problemSplitted[0].Split('\n'));
        string problem = problemSplitted[1].Trim();
        string output = "";

        try { output = problemSplitted[2].Trim(); }
        catch { }

        dragAndDropPuzzle.Initialize(problem, answers, output);

        puzzleSolvings.Add(dragAndDropPuzzle);
        electronicDevice.puzzleModal = dragAndDropPuzzle;
    }

    void DistributeDragAndArrange(ElectronicDeviceTraining electronicDevice)
    {
        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        string levelName = DatabaseManager.instance.currentLanguage.currentLevelName;
        TextAsset problemTextAsset = Resources.Load<TextAsset>($"Problems/{languageName}/{levelName}/dragAndArrange");

        string[] problemSplitted = problemTextAsset.text.Split("##############################");

        string problem = problemSplitted[0].Trim();
        string[] lines = problem.Split('\n');
        string output = "";

        try { output = problemSplitted[1].Trim(); }
        catch { }

        dragAndArrangePuzzle.Initialize(lines, output);

        puzzleSolvings.Add(dragAndArrangePuzzle);
        electronicDevice.puzzleModal = dragAndArrangePuzzle;
    }

    void DistributeFindAndSelect(ElectronicDeviceTraining electronicDevice)
    {
        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        string levelName = DatabaseManager.instance.currentLanguage.currentLevelName;
        TextAsset problemTextAsset = Resources.Load<TextAsset>($"Problems/{languageName}/{levelName}/findAndSelect");

        string[] lines = problemTextAsset.text.Split('\n');

        findAndSelectPuzzle.Initialize(lines);

        puzzleSolvings.Add(findAndSelectPuzzle);
        electronicDevice.puzzleModal = findAndSelectPuzzle;
    }

    void InitializeDevices()
    {
        foreach (GameObject computerObject in computerObjects)
        {
            ElectronicDevice electronicDevice = computerObject.GetComponent<ElectronicDevice>();
            electronicDevice.Initialize();
        }
        foreach (ElectronicDeviceTraining trainingComputer in trainingComputers)
        {
            trainingComputer.Initialize();
        }
        minigamesElectronicDevice.Initialize();
    }
}
