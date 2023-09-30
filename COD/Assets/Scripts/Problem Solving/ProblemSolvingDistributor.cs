using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProblemSolvingDistributor : MonoBehaviour
{
    GameObject[] computerObjects;
    private int index = 0;

    void Start()
    {
        computerObjects = GameObject.FindGameObjectsWithTag("Computer");
        GameObject[] shuffledComputerObjects = Shuffle(computerObjects);

        ProblemSolving[] problemSolvings = GetComponentsInChildren<ProblemSolving>(includeInactive: true);

        for (int i = 0; i < problemSolvings.Length; i++)
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

    private static T[] Shuffle<T>(T[] array)
    {
        int n = array.Length;

        for (int i = 0; i < n; i++)
        {
            int r = i + Random.Range(0, n - i); // Random index from i to n-1
            T temp = array[i];
            array[i] = array[r];
            array[r] = temp;
        }

        return array;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            GameObject computer = computerObjects[index];
            ObjectNavigation.Navigate(computer);
            index++;
        }
    }
}
