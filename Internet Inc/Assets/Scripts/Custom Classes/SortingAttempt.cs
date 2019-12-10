using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingAttempt : MonoBehaviour
{
    public static int correct = 0;
    public static int incorrect = 0;

    public float time;
    public bool isCorrect;

    public SortingAttempt(float t, bool c)
    {
        time = t;
        isCorrect = c;

        if (isCorrect)
        {
            correct++;
        }
        else
        {
            incorrect++;
        }
    }
}
