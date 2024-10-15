using System;
using System.Collections.Generic;
using UnityEngine;

public class MultithreadControl : MonoBehaviour
{
    private static readonly Queue<Action> ExecuteOnMainThread = new Queue<Action>();

    void Update()
    {
        lock (ExecuteOnMainThread)
        {
            while (ExecuteOnMainThread.Count > 0)
            {
                ExecuteOnMainThread.Dequeue().Invoke();
            }
        }
    }

    public static void RunOnMainThread(Action action)
    {
        lock (ExecuteOnMainThread)
        {
            ExecuteOnMainThread.Enqueue(action);
        }
    }
}
