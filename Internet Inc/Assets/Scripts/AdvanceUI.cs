using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdvanceUI : MonoBehaviour
{
    [SerializeField] GameObject tintImage;
    [SerializeField] GameObject backgroundImage;
    [SerializeField] GameObject promotionHeader;
    [SerializeField] GameObject promotionText;
    [SerializeField] GameObject[] promotionButtons;
    [SerializeField] GameObject stayAdvanceButton;

    // Start is called before the first frame update
    void Start()
    {
        EnableUI(false);
        // stayAdvanceButton.SetActive(false); // TODO: Implement
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Stay()
    {
        // TODO: Disable UI
        EnableUI(false);

        stayAdvanceButton.SetActive(true);
    }

    public void EnableUI(bool tf)
    {
        tintImage.SetActive(tf);
        backgroundImage.SetActive(tf);
        promotionHeader.SetActive(tf);
        promotionText.SetActive(tf);
        
        foreach (GameObject g in promotionButtons)
        {
            g.SetActive(tf);
        }
    }
}
