using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] SortingObjectBehavior requestObject;
    [SerializeField] TextAsset requestsText;

    ArrayList attempts;
    ArrayList requests;
    int requestIndex;

    // Start is called before the first frame update
    void Start()
    {
        attempts = new ArrayList();
        requests = new ArrayList();
        requestIndex = 0;

        string[] parts = requestsText.text.Split('\n');
        foreach (string part in parts)
        {
            requests.Add(part.Trim());
        }

        NewRequest();
    }

    public void Sort(bool isCorrect)
    {
        attempts.Add(new SortingAttempt(Time.time, isCorrect));
        if (isCorrect)
        {
            NewRequest();
        }
    }

    public void NewRequest()
    {
        requestIndex = Random.Range(0, requests.Count);
        requestObject.Target = requests[requestIndex].ToString();
    }
}
