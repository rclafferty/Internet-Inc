using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    // Possibly won't need anymore
    public enum DNSLevel { Subdomain, Authoritative, TopLevel };
    public DNSLevel ThisLevel { get; private set; }
    float initialTextWidth;
    
    List<SortingAttempt> sortingAttempts;

    [Header("Sorting Objects")]
    [SerializeField] GameObject[] sortingBoxObjects;
    [SerializeField] Text[] sortingBoxText;
    [SerializeField] Text requestURL;

    [Header("Gameplay UI")]
    [SerializeField] Image progressBarImage;
    [SerializeField] Text progressBartText;
    [SerializeField] Text equivalenceUIText;

    [Header("Promotion UI")]
    [SerializeField] Canvas promotionUI;
    AdvanceUI advanceUI; // AdvanceUI component of promotionUI
    [SerializeField] Text promotionHeader;
    [SerializeField] Text promotionText;
    [SerializeField] Text promotionTextLocation;
    [SerializeField] Text staySubtext;
    [SerializeField] Text advanceSubtext;

    [Header("Imported Text Files")]
    [SerializeField] TextAsset requestsText;
    [SerializeField] TextAsset domainText;
    [SerializeField] TextAsset equivalenceText;
    [SerializeField] TextAsset certificateText;

#if UNITY_EDITOR
    const int PROMOTION_THRESHOLD = 5;
#else
    const int PROMOTION_THRESHOLD = 30;
#endif
    const float PROMOTION_PERCENTAGE = 0.9f;
    const int MAX_CONSIDERED_ATTEMPTS = 45;

    [Header("Threshold Values")]
    [SerializeField] float[] thresholdPercentages;
    [SerializeField] Color[] thresholdColors;

    List<string> domains;
    List<string> requests;

    int numberRequestsPerDomain;
    int currentDomainIndex;
    int currentRequestIndex;

    bool waitingToAdvance;

    private void Awake()
    {
        waitingToAdvance = false;
        sortingAttempts = new List<SortingAttempt>();
        domains = new List<string>();
        requests = new List<string>();

        string[] domainParts;
        string[] requestParts;

        domainParts = domainText.text.Split('\n');
        foreach (string s in domainParts)
        {
            domains.Add(s.Trim());
        }

        requestParts = requestsText.text.Split('\n');
        foreach (string s in requestParts)
        {
            requests.Add(s.Trim());
        }

        numberRequestsPerDomain = requests.Count / domains.Count;

        string levelName = SceneManager.GetActiveScene().name;
        if (levelName == "subdomain")
        {
            ThisLevel = DNSLevel.Subdomain;
        }
        else if (levelName == "authoritative")
        {
            ThisLevel = DNSLevel.Authoritative;
        }
        else if (levelName == "top_level")
        {
            ThisLevel = DNSLevel.TopLevel;
        }

        Debug.Log(ThisLevel.ToString());
    }

    // Start is called before the first frame update
    void Start()
    {
        advanceUI = promotionUI.GetComponent<AdvanceUI>();
        advanceUI.EnableUI(false, false);

        equivalenceUIText.text = equivalenceText.text;

        initialTextWidth = progressBartText.rectTransform.rect.width;

        for (int i = 0; i < sortingBoxObjects.Length; i++)
        {
            sortingBoxObjects[i].GetComponent<SorterBehavior>().Target = domains[i];
            sortingBoxText[i].text = "Forward\nTo\n" + domains[i].ToLower();
        }

        SetScore();
        NewRequest();
    }

    void SetScore()
    {
        SetProgressBar(CalculateScore());
    }

    void NewRequest()
    {
        int thisRequestIndex = currentRequestIndex;
        int thisDomainIndex = currentDomainIndex;

        do
        {
            thisRequestIndex = Random.Range(0, requests.Count);
            thisDomainIndex = thisRequestIndex / numberRequestsPerDomain;
        } while (thisRequestIndex == currentRequestIndex || thisDomainIndex == currentDomainIndex);

        currentRequestIndex = thisRequestIndex;
        currentDomainIndex = thisDomainIndex;

        requestURL.text = requests[currentRequestIndex];
        requestURL.gameObject.GetComponent<SortingObjectBehavior>().Target = requests[currentRequestIndex];
    }

    public float CalculateScore()
    {
        if (sortingAttempts.Count == 0)
        {
            return 0.0f;
        }

        float scorePercentage = 0.0f;
        if (sortingAttempts.Count > MAX_CONSIDERED_ATTEMPTS)
        {
            int difference = sortingAttempts.Count - MAX_CONSIDERED_ATTEMPTS;
            for (int i = 0; i < difference; i++)
            {
                if (sortingAttempts[0].isCorrect)
                {
                    SortingAttempt.correct--;
                }
                else
                {
                    SortingAttempt.incorrect--;
                }

                sortingAttempts.RemoveAt(0);
            }
        }

        float correctPercentage = (float)SortingAttempt.correct / sortingAttempts.Count;
        float incorrectPercentage = (float)SortingAttempt.incorrect / sortingAttempts.Count;
        float progress = (float)sortingAttempts.Count / PROMOTION_THRESHOLD;

        scorePercentage = (correctPercentage - incorrectPercentage) * progress;

        if (scorePercentage > 1)
        {
            scorePercentage = 1;
        }
        
        if (scorePercentage >= PROMOTION_PERCENTAGE && sortingAttempts.Count >= PROMOTION_THRESHOLD)
        {
            if (!waitingToAdvance)
                Promote();
        }

        return scorePercentage * 100;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value">Decimal value 0 to 100 representing progress percentage</param>
    void SetProgressBar (float value)
    {
        if (waitingToAdvance)
        {
            progressBarImage.transform.localScale = new Vector3(100, 1, 1);
            progressBartText.text = "100%";
            return;
        }
        else if (value <= 1f)
        {
            progressBarImage.transform.localScale = new Vector3(1, 1, 1);
            progressBartText.text = "0%";
            return;
        }
        else
        {
            if (value > 100)
            {
                value = 100.0f;
            }

            progressBarImage.transform.localScale = new Vector3(value, 1, 1);
            progressBartText.text = value.ToString("##0") + "%";
        }

        Rect textRect = progressBartText.rectTransform.rect;
        float barWidth = progressBarImage.rectTransform.rect.width * progressBarImage.rectTransform.localScale.x;
        float textWidth = textRect.width;

        if (barWidth > initialTextWidth)
        {
            textRect.width = barWidth;
            progressBartText.rectTransform.sizeDelta = new Vector2(textRect.width, textRect.height);
        }

        for (int i = 0; i < thresholdPercentages.Length; i++)
        {
            if (value >= thresholdPercentages[i])
            {
                progressBarImage.color = thresholdColors[i];
                break;
            }
        }

        // progressBartText.text = ((int)(value)).ToString();
    }

    public void NewSortAttempt(bool isCorrect)
    {
        if (sortingAttempts.Count > 0)
        {
            // Is it an accidental duplication?
            float time = Time.time;
            float oldTime = sortingAttempts[sortingAttempts.Count - 1].time;
            if (time - oldTime < 0.1f)
            {
                // Invalid
                return;
            }
        }

        sortingAttempts.Add(new SortingAttempt(isCorrect, Time.time));
        SetScore();
        if (isCorrect)
        {
            NewRequest();
        }
    }

    void Promote()
    {
        promotionHeader.text = "You've been promoted!";
        string line = "You've been recognized for your efforts and have been promoted to the\n\n\n\nYou may stay and practice the ## protocol or you may advance to the next office.";

        string[] parts = certificateText.text.Split('\n');

        string currentProtocol = parts[0];
        string nextOfficeLocation = parts[1];
        string staySubtextLine1 = parts[2];
        string staySubtextLine2 = parts[3];
        string staySubtextString = "";
        Debug.Log(staySubtextLine2);
        if (!string.IsNullOrEmpty(staySubtextLine2))
        {
            staySubtextString = staySubtextLine1 + "\n" + staySubtextLine2;
        }
        else
        {
            staySubtextString = staySubtextLine1;
        }

        string advanceSubtextLine1 = "Proceed To";
        string advanceSubtextLine2 = nextOfficeLocation;
        string advanceSubtextString = "";
        Debug.Log(advanceSubtextLine2);
        if (!string.IsNullOrEmpty(advanceSubtextLine2))
        {
            advanceSubtextString = advanceSubtextLine1 + "\n" + advanceSubtextLine2;
        }
        else
        {
            advanceSubtextString = advanceSubtextLine1;
        }

        line = line.Replace("##", currentProtocol);
        promotionTextLocation.text = nextOfficeLocation;
        staySubtext.text = staySubtextString;
        advanceSubtext.text = advanceSubtextString;

        if (ThisLevel == DNSLevel.TopLevel)
        {
            nextOfficeLocation = "Expand Company";

            promotionHeader.text = "Congratulations!";
            line = "Now that you've pioneered your way through the company, you should consider expanding your reach internationally.\n\nYou may stay and practice the Root DNS Server protocol or you may continue to expand your company.";
            promotionTextLocation.text = "";
            staySubtext.text = "Practice Root\nDNS Lookup";
            advanceSubtext.text = "Proceed To\n" + nextOfficeLocation;
        }

        advanceUI.SetAdvanceButtonText("Advance To " + nextOfficeLocation);

        promotionText.text = line;

        advanceUI.EnableUI(true, false);
        waitingToAdvance = true;
        SortingAttempt.Reset();
        SetScore();
    }
}

public class SortingAttempt
{
    public static int correct = 0;
    public static int incorrect = 0;

    public bool isCorrect;
    public float time;

    public SortingAttempt(bool tf, float t)
    {
        isCorrect = tf;
        if (isCorrect)
        {
            correct++;
        }
        else
        {
            incorrect++;
        }

        time = t;
    }

    public static void Reset()
    {
        correct = 0;
        incorrect = 0;
    }
}