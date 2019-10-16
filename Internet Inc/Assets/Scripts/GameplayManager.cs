﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] Canvas advanceUI;
    [SerializeField] Text scoreText;
    [SerializeField] Text levelText;
    [SerializeField] Text equivalenceText;
    [SerializeField] Text incorrectText;
    [SerializeField] Text[] sortingBoxText;

    List<Attempt> attempts;

    float time;

    private void Awake()
    {
        string name = SceneManager.GetActiveScene().name;
        if (name == "authoritative")
        {
            isAuthoritative = true;
            isTLD = false;
        }
        else if (name == "top_level")
        {
            isAuthoritative = false;
            isTLD = true;
        }

        attempts = new List<Attempt>();
    }

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

            sortingBoxText[i].text = "Forward\nto\n" + domains[i].ToUpper();
        }

        advanceUI.enabled = false;
        SetScore();
        NewRequest();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
    }

    public void NewRequest()
    {
        requestObject = Instantiate(requestObjectPrefab);

        int r = Random.Range(0, sortingBoxes.Length);// TODO: Change to # of prompts, not boxes
        do
        {
            r = Random.Range(0, sortingBoxes.Length);
        } while (r == currentIndex);

        currentIndex = r;

        requestObject.GetComponent<SortingBehavior>().Target = requestStrings[r];
        requestObject.GetComponent<SpriteRenderer>().sprite = null; // requestSprites[r];
    }

    public void CorrectSort()
    {
        attempts.Add(new Attempt(Time.time, true));
        SetScore();
        NewRequest();
    }

    public float CalculateScore()
    {
        if (attempts.Count > 30)
        {
            int difference = attempts.Count - 30;
            for (int i = 0; i < difference; i++)
            {
                attempts.RemoveAt(0); // remove oldest
            }
        }

        int score = 0;
        for (int i = 0; i < attempts.Count; i++)
        {
            if (attempts[i].isCorrect == true)
            {
                score++;
            }
        }

        return ((float)score / attempts.Count) * 100;
    }

    public void SetScore()
    {
        float percentage = CalculateScore();

        if (attempts.Count == 0)
        {
            scoreText.text = "Correct: 0%";
        }
        else
        {
            scoreText.text = "Correct: " + percentage.ToString("0") + "%";

            if (attempts.Count > 20 && percentage > 90)
            {
                Advance();
            }
        }
    }

    public void IncorrectSort()
    {
        attempts.Add(new Attempt(Time.time, false));
        SetScore();
    }

    void Advance()
    {
        Debug.Log("You've been promoted!");
        advanceUI.enabled = true;
    }

    public void AdvanceButtonClicked()
    {
        advanceUI.enabled = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StayButtonClicked()
    {
        advanceUI.enabled = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

public class Attempt
{
    public float time;
    public bool isCorrect;

    public Attempt(float t, bool c)
    {
        time = t;
        isCorrect = c;
    }
}