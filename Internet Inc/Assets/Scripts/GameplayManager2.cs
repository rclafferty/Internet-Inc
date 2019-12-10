using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayManager2 : MonoBehaviour
{
    public enum DNSLevel { Subdomain, Authoritative, TopLevel };
    public DNSLevel ThisLevel { get; private set; }

    List<SortingAttempt> sortingAttempts;
    float attemptTime;

    [Header("Prefabs")]
    [SerializeField] GameObject prefab_requestObject;
    GameObject requestObjectInstance;

    [Header("Scene Objects")]
    [SerializeField] GameObject[] sortingBoxes;
    [SerializeField] Text[] sortingBoxText;
    [SerializeField] Text requestObjectText;

    [Header("Progress Bar")]
    [SerializeField] Image progressBarImage;
    [SerializeField] Text progressBarText;

    [Header("Promotion UI")]
    [SerializeField] Canvas promotionUI;
    AdvanceUI advanceUI;
    [SerializeField] Text promotionHeader;
    [SerializeField] Text promotionText;

    [Header("Imported Text Files - Domains")]
    [SerializeField] TextAsset subdomainDomainsText;
    [SerializeField] TextAsset authoritativeDomainsText;
    [SerializeField] TextAsset topLevelDomainsText;
    
    [Header("Imported Text Files - Requests")]
    [SerializeField] TextAsset subdomainRequestsText;
    [SerializeField] TextAsset authoritativeRequestsText;
    [SerializeField] TextAsset topLevelRequestsText;

    [Header("Imported Text Files - Equivalence Info")]
    [SerializeField] TextAsset subdomainInformation;
    [SerializeField] TextAsset authoritativeInformation;
    [SerializeField] TextAsset topLevelInformation;

    // String arrays for imported text file content
    List<string> domains;
    List<string> requests;

    // Promotion thresholds
    const int PROMOTION_THRESHOLD = 5; // 30
    const int MAX_CONSIDERED_ATTEMPTS = 45;

    // Progress bar thresholds
    const float PROGRESS_BAR_LOW_PERCENTAGE = 0.0f;
    Color PROGRESS_BAR_LOW_COLOR = Color.red;
    const float PROGRESS_BAR_MED_PERCENTAGE = 0.4f;
    Color PROGRESS_BAR_MED_COLOR = Color.yellow;
    const float PROGRESS_BAR_HIGH_PERCENTAGE = 0.8f;
    Color PROGRESS_BAR_HIGH_COLOR = Color.green;

    float progressPercentage;

    int numberRequestsPerDomain;
    int currentDomain;
    int currentRequest;

    float initialTextWidth;

    const string PROMOTION_TEMPLATE_LINE1 = "You've been recognized for your efforts and have been offered a promotion to the ";
    const string PROMOTION_TEMPLATE_LINE2 = "You may stay and practice the ## proocol or you may advance to the next office.\nWhich will you choose?";

    private void Awake()
    {
        progressPercentage = 0.0f;

        sortingAttempts = new List<SortingAttempt>();
        domains = new List<string>();
        requests = new List<string>();

        sortingAttempts.Clear();

        string[] domainParts;
        string[] requestParts;

        string levelName = SceneManager.GetActiveScene().name;

        // If subdomain level
        if (levelName == "subdomain")
        {
            ThisLevel = DNSLevel.Subdomain;

            domainParts = subdomainDomainsText.text.Split('\n');
            foreach (string s in domainParts)
            {
                domains.Add(s.Trim());
            }

            requestParts = subdomainRequestsText.text.Split('\n');
            foreach (string s in requestParts)
            {
                requests.Add(s.Trim());
            }
        }
        // If authoritative level
        else if (levelName == "authoritative")
        {
            ThisLevel = DNSLevel.Authoritative;

            domainParts = authoritativeDomainsText.text.Split('\n');
            foreach (string s in domainParts)
            {
                domains.Add(s.Trim());
            }

            requestParts = authoritativeRequestsText.text.Split('\n');
            foreach (string s in requestParts)
            {
                requests.Add(s.Trim());
            }
        }
        // If TLD level
        else if (levelName == "top_level")
        {
            ThisLevel = DNSLevel.TopLevel;

            domainParts = topLevelDomainsText.text.Split('\n');
            foreach (string s in domainParts)
            {
                domains.Add(s.Trim());
            }

            requestParts = topLevelRequestsText.text.Split('\n');
            foreach (string s in requestParts)
            {
                requests.Add(s.Trim());
            }
        }

        numberRequestsPerDomain = requests.Count / domains.Count;
    }

    // Start is called before the first frame update
    void Start()
    {
        advanceUI = promotionUI.GetComponent<AdvanceUI>();
        advanceUI.EnableUI(false);

        initialTextWidth = progressBarText.rectTransform.rect.width;

        SorterBehavior2 sb;
        for (int i = 0; i < sortingBoxes.Length; i++)
        {
            sb = sortingBoxes[i].GetComponent<SorterBehavior2>();
            sb.Target = domains[i];

            sortingBoxText[i].text = "Forward\nto\n" + domains[i].ToLower();
        }

        SetScore();
        NewRequest();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value">Decimal value 0 to 1 representing percentage of progress</param>
    void SetProgressBar(float value)
    {
        // Set bar width
        if (value <= 0.1f)
        {
            progressBarImage.transform.localScale = new Vector3(0.1f, 1, 1);
            progressBarText.text = "0%";
        }
        else
        {
            progressPercentage = value;
            progressBarImage.transform.localScale = new Vector3(progressPercentage * 10, 1, 1);
            progressBarText.text = value.ToString("##0") + "%";
        }

        // Calculate text width
        float barWidth = progressBarImage.rectTransform.rect.width * progressBarImage.rectTransform.localScale.x;
        float textWidth = progressBarText.rectTransform.rect.width;
        Rect textRect = progressBarText.rectTransform.rect;

        // Set text width
        if (barWidth > initialTextWidth)
        {
            textRect.width = barWidth;
            progressBarText.rectTransform.sizeDelta = new Vector2(textRect.width, textRect.height);
        }

        // Set progress bar color
        if (value >= PROGRESS_BAR_HIGH_PERCENTAGE)
        {
            progressBarImage.color = PROGRESS_BAR_HIGH_COLOR;
        }
        else if (value >= PROGRESS_BAR_MED_PERCENTAGE)
        {
            progressBarImage.color = PROGRESS_BAR_MED_COLOR;
        }
        else
        {
            progressBarImage.color = PROGRESS_BAR_LOW_COLOR;
        }

        // Set progress bar text
        progressBarText.text = ((int)(value * 100)).ToString();
    }

    public void NewRequest()
    {
        int thisRequest = currentRequest;
        int thisDomain = currentDomain;

        // Find new request that is NOT the same as current one
        do
        {
            thisRequest = Random.Range(0, requests.Count);
            thisDomain = thisRequest / numberRequestsPerDomain;
        } while (thisRequest == currentRequest || thisDomain == currentDomain);

        currentRequest = thisRequest;
        currentDomain = thisDomain;

        // Set GameObject text
        requestObjectText.text = requests[currentRequest];
        string request = requests[currentRequest];
        requestObjectText.gameObject.GetComponent<ClickAndDragBehavior2>().Target = request;
    }

    public void Sort(bool isCorrect)
    {
        sortingAttempts.Add(new SortingAttempt(Time.time, isCorrect));

        // Calculate score
        SetScore();

        if (isCorrect)
        {
            // Set new request
            NewRequest();
        }
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

        if (scorePercentage * 100 >= 90 && sortingAttempts.Count >= PROMOTION_THRESHOLD)
        {
            // Promote
            Promote();
        }

        return scorePercentage;
    }

    public void SetScore()
    {
        SetProgressBar(CalculateScore());
    }

    void Promote()
    {
        promotionHeader.text = "You've been promoted!";
        string customizedLine2 = PROMOTION_TEMPLATE_LINE2;

        if (ThisLevel == DNSLevel.Subdomain)
        {
            customizedLine2 = customizedLine2.Replace("##", "Authoritative DNS Server");
            promotionText.text = PROMOTION_TEMPLATE_LINE1 + "District Office\n" + customizedLine2;
        }
        else if (ThisLevel == DNSLevel.Authoritative)
        {
            customizedLine2 = customizedLine2.Replace("##", "Top-Level Domain DNS Server");
            promotionText.text = PROMOTION_TEMPLATE_LINE1 + "Corporate Office\n" + customizedLine2;
        }
        else if (ThisLevel == DNSLevel.TopLevel)
        {
            promotionHeader.text = "Congratulations!";
            customizedLine2 = "Now that you've pioneered your way through the company, you should consider expanding your reach internationally.\nYou may stay and practice the Root DNS Server protocol or you may continue to expand your company.\nWhich will you choose?";
        }

        // TODO: Activate UI
        advanceUI.EnableUI(true);

        SortingAttempt.Reset();
    }
}