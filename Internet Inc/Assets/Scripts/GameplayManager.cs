using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    const int PROMOTION_THRESHOLD = 35;
    const int MAX_CONSIDERED_ATTEMPTS = 45;
    const float RED_PROGRESS_BAR_PERCENTAGE = 0.0f;
    const float YELLOW_PROGRESS_BAR_PERCENTAGE = 0.4f;
    const float GREEN_PROGRESS_BAR_PERCENTAGE = 0.8f;

    [Header("Toggle Values")]
    [SerializeField] bool isAuthoritative;
    [SerializeField] bool isTLD;

    int currentIndex;
    int currentDomain;
    [SerializeField] float percentage;
    [SerializeField] int numAttempts;

    float initialTextWidth;

    [Header("TLD Values")]
    string[] domainsTLD = {
        "gov",
        "net",
        "com"
    };
    string[] requestStringsTLD = {
        // *.gov URLs
        "www.abc.gov",
        "www.mygovernment.gov",
        "www.yourgovernment.gov",
        "www.anarchy.gov",
        "www.politicians.gov",
        "www.galactic.gov",

        // *.net URLs
        "www.iknowyou.net",
        "www.youknowme.net",
        "www.birdies.net",
        "www.bicyclefriends.net",
        "www.myinternet.net",
        "www.mytelephone.net",

        // *.com URLs
        "www.mybusiness.com",
        "www.travelwithme.com",
        "www.ilikedancing.com",
        "www.socialpeople.com",
        "www.irememberyou.com",
        "www.meetmyancestors.com"
    };

    [Header("Authoritative Values")]
    string[] domainsAuthoritative = {
        "mybusiness",
        "travelwithme",
        "ilikedancing"
    };
    string[] requestStringsAuthoritative = {
        // *.mybusiness.com
        "www.mybusiness.com",
        "joinme.mybusiness.com",
        "shop.mybusiness.com",
        "about.mybusiness.com",

        // *.travelwithme.com
        "www.travelwithme.com",
        "travel.travelwithme.com",
        "where.travelwithme.com",
        "souvenirs.travelwithme.com",

        // *.ilikedancing.com
        "www.ilikedancing.com",
        "join.ilikedancing.com",
        "performances.ilikedancing.com",
        "staff.ilikedancing.com"
    };

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
    [SerializeField] Text requestText;
    [SerializeField] Text[] sortingBoxText;
    [SerializeField] Image progressBar;
    [SerializeField] Text progressText;
    [SerializeField] Text promotionHeader;
    [SerializeField] Text promotionText;

    List<Attempt> attempts;

    float time;

    private void Awake()
    {
        currentIndex = -1;
        currentDomain = -1;
        numAttempts = 0;
        percentage = 0;
        initialTextWidth = progressText.rectTransform.rect.width;

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

            sortingBoxText[i].text = "Forward\nto\n" + domains[i].ToLower();
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
        int numberPerDomain = requestStrings.Length / domains.Length;

        int r = currentIndex;
        int tempDomain = currentDomain;
        do
        {
            r = Random.Range(0, requestStrings.Length);
            tempDomain = r / numberPerDomain;
        } while (r == currentIndex || tempDomain == currentDomain);

        currentIndex = r;
        currentDomain = tempDomain;

        requestText.text = requestStrings[r];
        requestText.gameObject.GetComponent<SortingBehavior>().Target = requestStrings[r];
    }

    public void CorrectSort()
    {
        attempts.Add(new Attempt(Time.time, true));
        SetScore();
        NewRequest();
    }

    public float CalculateScore()
    {
        if (attempts.Count == 0)
            return 0.0f;
        
        if (attempts.Count > MAX_CONSIDERED_ATTEMPTS)
        {
            int difference = attempts.Count - MAX_CONSIDERED_ATTEMPTS;
            for (int i = 0; i < difference; i++)
            {
                // Re-evaluate # of correct/incorrect
                if (attempts[0].isCorrect)
                {
                    Attempt.correct--;
                }
                else
                {
                    Attempt.incorrect--;
                }

                // remove oldest
                attempts.RemoveAt(0);
            }
        }

        percentage = (((float)Attempt.correct / attempts.Count) - ((float)Attempt.incorrect / attempts.Count)) * ((float)attempts.Count / PROMOTION_THRESHOLD);

        if (percentage > 1)
        {
            percentage = 1;
        }

        if (percentage * 100 >= 90 && attempts.Count >= PROMOTION_THRESHOLD)
        {
            Advance();
        }

        return percentage;
    }
    public void SetScore()
    {
        float progressPercent = CalculateScore();
        percentage = progressPercent * 100;
        numAttempts = attempts.Count;

        if (attempts.Count == 0)
        {
            scoreText.text = "Correct: 0%";

            progressBar.transform.localScale = new Vector3(0.1f, 1, 1);
            progressText.text = "0%";
        }
        else
        {
            progressBar.transform.localScale = new Vector3(progressPercent * 10, 1, 1);
            progressText.text = (percentage).ToString("##0") + "%";
        }

        if (progressPercent >= GREEN_PROGRESS_BAR_PERCENTAGE)
        {
            progressBar.color = Color.green;
        }
        else if (progressPercent >= YELLOW_PROGRESS_BAR_PERCENTAGE)
        {
            progressBar.color = Color.yellow;
        }
        else
        {
            progressBar.color = Color.red;
        }

        float barWidth = progressBar.rectTransform.rect.width;
        float barScale = progressBar.rectTransform.localScale.x;
        float barWidthCalculated = barWidth * barScale;
        float textWidth = progressText.rectTransform.rect.width;
        Rect textRect = progressText.rectTransform.rect;
        if (barWidthCalculated > initialTextWidth)
        {
            textRect.width = barWidthCalculated;
            progressText.rectTransform.sizeDelta = new Vector2(textRect.width, textRect.height);

        }
        Debug.Log("Width: " + textRect.width.ToString("#0.0") + "   Bar Width: " + barWidthCalculated.ToString("#0.0"));
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

        string sceneName = SceneManager.GetActiveScene().name;
        string promotionTemplateLine1 = "You've been recognized for your efforts and have been offered a promotion to the ";
        string promotionTemplateLine2 = "You may stay and practice the ";
        string promotionTemplateLine2part2 = " protocol or you may advance to the next office.\nWhich will you choose?";

        if (sceneName == "sub_domain")
        {
            promotionHeader.text = "You've been promoted!";
            promotionText.text = promotionTemplateLine1 + "District Office." + "\n" + promotionTemplateLine2 + "Authoritative DNS Server" + promotionTemplateLine2part2;
        }
        else if (sceneName == "authoritative")
        {
            promotionHeader.text = "You've been promoted!";
            promotionText.text = promotionTemplateLine1 + "Corporate Office." + "\n" + promotionTemplateLine2 + "Top-Level Domain DNS Server" + promotionTemplateLine2part2;
        }
        else if (sceneName == "top_level")
        {
            promotionHeader.text = "Congratulations!";
            promotionText.text = "Now that you've pioneered your way through the company, you should consider expanding your reach internationally.\nYou may stay and practice the Root DNS Server protocol or you may continue to expand your company.\nWhich will you choose?";
        }
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

    public static int correct = 0;
    public static int incorrect = 0;

    public Attempt(float t, bool c)
    {
        time = t;
        isCorrect = c;

        if (isCorrect)
            correct++;
        else
            incorrect++;
    }
}