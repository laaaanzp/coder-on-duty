using System;
using System.Collections.Generic;
using UnityEngine;

public class ProblemSolvingDistributor : MonoBehaviour
{
    [SerializeField] private DragAndArrangePuzzle dragAndArrangePuzzle;
    [SerializeField] private DragAndDropPuzzle dragAndDropPuzzle;
    [SerializeField] private FindAndSelect findAndSelectPuzzle;
    [SerializeField] private Crossword crosswordPuzzle;
    [SerializeField] private WordSearch wordSearchPuzzle;
    [SerializeField] private MatchingType matchingTypePuzzle;
    [SerializeField] private MinigameDevice minigamesElectronicDevice;

    [Header("Training Computers - Level 1")]
    [SerializeField] private ElectronicDeviceTraining[] trainingComputers;

    [Header("Developer Computers - Level 2")]
    [SerializeField] private ElectronicDevice[] developerComputers;

    [Header("Random Computers - Level 3")]
    [SerializeField] private ElectronicDevice[] randomComputers;

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
            DistributeDragAndDrop(developerComputers[0].GetComponent<ElectronicDevice>());
            DistributeDragAndArrange(developerComputers[1].GetComponent<ElectronicDevice>());
            DistributeFindAndSelect(developerComputers[2].GetComponent<ElectronicDevice>());
        }
        else if (DatabaseManager.instance.currentLanguage.currentLevel == 3)
        {
            DistributeDragAndDrop(randomComputers[0].GetComponent<ElectronicDevice>());
            DistributeDragAndArrange(randomComputers[1].GetComponent<ElectronicDevice>());
            DistributeFindAndSelect(randomComputers[2].GetComponent<ElectronicDevice>());
        }
        else 
        {
            DistributeDragAndDrop(computerObjects[0].GetComponent<ElectronicDevice>());
            DistributeDragAndArrange(computerObjects[1].GetComponent<ElectronicDevice>());
            DistributeFindAndSelect(computerObjects[2].GetComponent<ElectronicDevice>());
        }

        switch (DatabaseManager.instance.currentLanguage.currentLevel)
        {
            case 4:
            case 7:
            case 10:
                DistributeCrossword(minigamesElectronicDevice);
                break;
            case 5:
            case 8:
            case 11:
                DistributeWordSearch(minigamesElectronicDevice);
                break;        
            case 6:
            case 9:
                DistributeMatchingType(minigamesElectronicDevice);
                break;
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

    void DistributeWordSearch(MinigameDevice minigameDevice)
    {
        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        string levelName = DatabaseManager.instance.currentLanguage.currentLevelName;
        TextAsset problemTextAsset = Resources.Load<TextAsset>($"Problems/{languageName}/{levelName}/crossword");

        string[] problemSplitted = problemTextAsset.text.Split("####################");
        string crosswordText = problemSplitted[0];
        string crosswordClues = problemSplitted[1];

        wordSearchPuzzle.Initialize(crosswordText, crosswordClues);

        minigameDevice.puzzleModal = wordSearchPuzzle;
    }


    void DistributeMatchingType(MinigameDevice minigameDevice)
    {
        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        string levelName = DatabaseManager.instance.currentLanguage.currentLevelName;
        TextAsset problemTextAsset = Resources.Load<TextAsset>($"Problems/{languageName}/{levelName}/matchingType");

        matchingTypePuzzle.Initialize(problemTextAsset.text);

        minigameDevice.puzzleModal = matchingTypePuzzle;
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
