using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    [Header("Toggle Values")]
    [SerializeField] bool isAuthoritative;
    [SerializeField] bool isTLD;

    int currentIndex;

    [Header("TLD Values")]
    [SerializeField] string[] domainsTLD = { "gov", "net", "com" };
    string[] requestStringsTLD = { "www.abc.gov", "www.iknowyou.net", "www.meetnewfriends.com" };

    [Header("Authoritative Values")]
    [SerializeField] string[] domainsAuthoritative = { "facebook", "twitter", "instagram" };
    string[] requestStringsAuthoritative = { "www.facebook.com", "www.twitter.com", "www.instagram.com" };

    [Header("Preset Values")]
    [SerializeField] GameObject requestObjectPrefab;
    GameObject requestObject;
    [SerializeField] Sprite[] requestSprites;

    [SerializeField] GameObject[] sortingBoxes;

    string[] requestStrings;

    string[] domains;

    [Header("UI")]
    [SerializeField] Text scoreText;
    [SerializeField] Text levelText;
    [SerializeField] Text equivalenceText;
    [SerializeField] Text incorrectText;
    int score;
    int incorrect;

    // Start is called before the first frame update
    void Start()
    {
        if (isAuthoritative)
        {
            domains = domainsAuthoritative;
            requestStrings = requestStringsAuthoritative;
            levelText.text = "District Office";
            equivalenceText.text = "EQ: Top Level Domain DNS";
        }
        else if (isTLD)
        {
            domains = domainsTLD;
            requestStrings = requestStringsTLD;
            levelText.text = "Coporate Office";
            equivalenceText.text = "EQ: Root DNS";
        }
        else
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }

        SorterBehavior sb;
        for(int i = 0; i < sortingBoxes.Length; i++)
        {
            sb = sortingBoxes[i].GetComponent<SorterBehavior>();
            sb.Target = domains[i];
            sb.isAuthoritative = isAuthoritative;
            sb.isTopLevel = isTLD;
        }

        SetScore(0);
        NewRequest();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewRequest()
    {
        requestObject = Instantiate(requestObjectPrefab);

        int r = Random.Range(0, sortingBoxes.Length);
        do
        {
            r = Random.Range(0, sortingBoxes.Length);
        } while (r == currentIndex);

        currentIndex = r;

        requestObject.GetComponent<SortingBehavior>().Target = requestStrings[r];
        requestObject.GetComponent<SpriteRenderer>().sprite = requestSprites[r];
    }

    public void CorrectSort()
    {
        SetScore(score + 1);
        NewRequest();
    }

    public void SetScore(int s)
    {
        score = s;
        /* scoreText.text = "Correct Attempts: " + score; */
        // incorrectText.text = "Correct Attempts: " + score + "\nIncorrect Attempts: " + incorrect;

        int attempts = score + incorrect;
        if (attempts == 0)
        {
            scoreText.text = "Correct: 0%";
        }
        else
        {
            if (attempts > 30)
                attempts = 30;

            float percentage = ((float)score / attempts) * 100;
            scoreText.text = "Correct: " + percentage.ToString("0") + "%";

            if (attempts > 20 && percentage > 90)
            {
                Advance();
            }
        }
    }

    public void IncorrectSort()
    {
        incorrect++;
        SetScore(score);
    }

    void Advance()
    {
        // TODO: Move to next level
    }
}
