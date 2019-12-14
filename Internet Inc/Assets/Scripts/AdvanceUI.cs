using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdvanceUI : MonoBehaviour
{
    [SerializeField] GameObject tintImage;
    [SerializeField] GameObject certificateImage;
    [SerializeField] GameObject stayAdvanceButton;

    // Start is called before the first frame update
    void Start()
    {
        // EnableUI(true, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableUI(bool uiActive, bool stayAdvanceActive)
    {
        if (uiActive)
        {
            stayAdvanceActive = false;
        }

        tintImage.SetActive(uiActive);
        certificateImage.SetActive(uiActive);

        stayAdvanceButton.SetActive(stayAdvanceActive);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Stay()
    {
        EnableUI(false, true);
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
