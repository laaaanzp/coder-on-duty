using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProblemSolvingDistributor : MonoBehaviour
{
    [SerializeField] private GameObject problemSolvingModalPrefab;

    private TextAsset[] problemTextAssets;


    void Start()
    {
        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        string sceneName = SceneManager.GetActiveScene().name;
        problemTextAssets = Resources.LoadAll<TextAsset>($"Problems/{languageName}/{sceneName}");

        GameObject[] computerObjects = GameObject.FindGameObjectsWithTag("Computer");
        GameObject[] shuffledComputerObjects = Tools.ShuffleArray(computerObjects);

        List<ProblemSolving> problemSolvings = new List<ProblemSolving>();

        foreach (TextAsset problemTextAsset in problemTextAssets)
        {
            GameObject problemSolvingModal = Instantiate(problemSolvingModalPrefab, transform);
            ProblemSolving problemSolving = problemSolvingModal.GetComponent<ProblemSolving>();

            string[] problemSplitted = problemTextAsset.text.Split("##############################");
            string[] answers = Tools.ShuffleArray(problemSplitted[0].Split('\n'));
            string problem = problemSplitted[1].Trim();

            problemSolving.Initialize(problem, answers);

            problemSolvings.Add(problemSolving);
        }

        for (int i = 0; i < problemSolvings.Count; i++)
        {
            ElectronicDevice electronicDevice = shuffledComputerObjects[i].GetComponent<ElectronicDevice>();
            electronicDevice.problemModal = problemSolvings[i];
        }

        foreach (GameObject computerObject in computerObjects)
        {
            ElectronicDevice electronicDevice = computerObject.GetComponent<ElectronicDevice>();
            electronicDevice.Initialize();
        }
    }
}
