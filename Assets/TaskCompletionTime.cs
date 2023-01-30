using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class TaskCompletionTime : MonoBehaviour
{

    private Stopwatch watch = new Stopwatch();
    private System.TimeSpan ts;
    private bool startButtonPressed;
    private bool stopButtonPressed;

    void Update()
    {
        if (startButtonPressed || Input.GetKeyDown("q"))
        {
            watch.Start();
        }

        if (stopButtonPressed || Input.GetKeyDown("w"))
        {

            watch.Stop();
        }

        ts = watch.Elapsed;
    }

    void OnApplicationQuit()
    {
        UnityEngine.Debug.Log(ts);
        UnityEngine.Debug.Log("Benötigte Zeit: " + ts.TotalSeconds +" Sekunden");
    }
    
}