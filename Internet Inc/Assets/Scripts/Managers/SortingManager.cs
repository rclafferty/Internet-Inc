using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortingManager : MonoBehaviour
{
    [SerializeField] GameplayManager gameplayManager;
    [SerializeField] GameObject[] sortingBoxes;
    [SerializeField] GameObject[] sortingBoxesTexts;
    [SerializeField] TextAsset domainsText;

    // Start is called before the first frame update
    void Start()
    {
        string[] parts = domainsText.text.Split('\n');
        for (int i = 0; i < sortingBoxes.Length; i++)
        {
            sortingBoxes[i].GetComponent<SorterBehavior>().Target = parts[i];
            sortingBoxesTexts[i].GetComponent<Text>().text = "Forward\nto\n" + parts[i].ToUpper();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Sort(bool isCorrect)
    {
        gameplayManager.Sort(isCorrect);
    }
}
