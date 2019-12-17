using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebserverGameplayManager : MonoBehaviour
{
    float initialTextWidth;
    List<SortingAttempt> sortingAttempts;

    List<string> domains;
    List<string> requests;

    int currentRequestIndex;

    [Header("Sorting Objects")]
    [SerializeField] SorterBehavior sortingPlane;
    [SerializeField] SortingObjectBehavior[] documents;

    [Header("Gameplay UI")]
    [SerializeField] Image progressBarImage;
    [SerializeField] Text progressBartText;
    [SerializeField] Text equivalenceUIText;

    [SerializeField] Text requestSpeechText;

    [Header("Promotion UI")]
    [SerializeField] AdvanceUI promotionUI;
    [SerializeField] Text promotionHeader;
    [SerializeField] Text promotionText;
    [SerializeField] Text promotionLocationText;
    [SerializeField] Text staySubtext;
    [SerializeField] Text advanceSubtext;

    [Header("Imported Text Files")]
    [SerializeField] TextAsset requestsText;
    [SerializeField] TextAsset domainsText;
    [SerializeField] TextAsset equivalenceText;
    [SerializeField] TextAsset certificateText;

    [Header("Threshold Values")]
    [SerializeField] float[] thresholdPercentages;
    [SerializeField] Color[] thresholdColors;

#if UNITY_EDITOR
    const int PROMOTION_THRESHOLD = 5;
#else
    const int PROMOTION_THRESHOLD = 30;
#endif

    const float PROMOTION_PERCENTAGE = 0.9f;
    const int MAX_CONSIDERED_ATTEMPTS = 45;

    bool waitingToAdvance;

    string[] requestsTemplates =
    {
        "Do you have the ## page?",
        "Do you know where ## is?",
        "Have you heard of ##?",
        "I'm looking for ##.",
        "Can you find ##?"
    };

    int requestTemplateIndex;

    private void Awake()
    {
        sortingAttempts = new List<SortingAttempt>();
        waitingToAdvance = false;
        requestTemplateIndex = -1;

        domains = new List<string>();
        requests = new List<string>();

        string[] domainsParts = domainsText.text.Split('\n');
        foreach (string s in domainsParts)
        {
            domains.Add(s.Trim());
        }

        string[] requestsParts = requestsText.text.Split('\n');
        foreach (string s in requestsParts)
        {
            requests.Add(s.Trim());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        promotionUI.EnableUI(false, false);
        currentRequestIndex = -1;

        initialTextWidth = progressBartText.rectTransform.rect.width;
        
        for (int i = 0; i < documents.Length; i++)
        {
            documents[i].Target = domains[i];
        }

        SetScore();
        NewRequest();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetScore()
    {
        SetProgressBar(CalculateScore());
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

    public void NewSortAttempt(bool isCorrect)
    {
        Debug.Log(isCorrect);

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

    void NewRequest()
    {
        int thisRequestIndex = currentRequestIndex;

        do
        {
            thisRequestIndex = Random.Range(0, requests.Count);
        } while (thisRequestIndex == currentRequestIndex);

        currentRequestIndex = thisRequestIndex;
        sortingPlane.Target = requests[currentRequestIndex];

        // Debug.Log(sortingPlane.Target);

        int thisTemplateIndex = requestTemplateIndex;
        do
        {
            thisTemplateIndex = Random.Range(0, requestsTemplates.Length);
        } while (thisTemplateIndex == requestTemplateIndex);

        requestTemplateIndex = thisTemplateIndex;
        requestSpeechText.text = requestsTemplates[requestTemplateIndex].Replace("##", sortingPlane.Target); ;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value">Decimal value 0 to 100 representing progress percentage</param>
    void SetProgressBar(float value)
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
        promotionLocationText.text = nextOfficeLocation;
        staySubtext.text = staySubtextString;
        advanceSubtext.text = advanceSubtextString;

        promotionUI.SetAdvanceButtonText("Advance To " + nextOfficeLocation);

        promotionText.text = line;

        promotionUI.EnableUI(true, false);
        waitingToAdvance = true;
        SortingAttempt.Reset();
        SetScore();
    }
}
