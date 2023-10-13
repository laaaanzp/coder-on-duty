using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProblemSolvingDistributor : MonoBehaviour
{
    [SerializeField] private GameObject problemSolvingModalPrefab;
    private List<ProblemSolving> problemSolvings;
    private TextAsset[] problemTextAssets;


    void Start()
    {
        string languageName = DatabaseManager.instance.currentLanguage.languageName;
        string sceneName = SceneManager.GetActiveScene().name;
        problemTextAssets = Resources.LoadAll<TextAsset>($"Problems/{languageName}/{sceneName}");
        problemTextAssets = Tools.ShuffleArray(problemTextAssets);

        GameObject[] computerObjects = GameObject.FindGameObjectsWithTag("Computer");
        GameObject[] shuffledComputerObjects = Tools.ShuffleArray(computerObjects);

        problemSolvings = new List<ProblemSolving>();

        int i = 0;
        foreach (TextAsset problemTextAsset in problemTextAssets)
        {
            if (i == 3)
                break;

            GameObject problemSolvingModal = Instantiate(problemSolvingModalPrefab, transform);
            ProblemSolving problemSolving = problemSolvingModal.GetComponent<ProblemSolving>();


            string[] problemSplitted = problemTextAsset.text.Split("##############################");
            string[] answers = Tools.ShuffleArray(problemSplitted[0].Split('\n'));
            string problem = problemSplitted[1].Trim();
            string output = "";
            try
            {
                output = problemSplitted[2].Trim();
            }
            catch { }

            problemSolving.Initialize(problem, answers, output);

            problemSolvings.Add(problemSolving);
            i++;
        }

        for (i = 0; i < problemSolvings.Count; i++)
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
