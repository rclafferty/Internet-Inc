using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroductionManager : MonoBehaviour
{
    [SerializeField] Text speechText;
    int speechIndex = 0;

    string[] dialogueSequence =
    {
        "I'm Willie WiFi, the CEO of Internet Inc.",
        "First of all, congratulations on your new position!",
        "Here at Internet Inc., we handle all sorts of web requests.",
        "They range from simply fetching web pages to DNS requests.",
        "What are DNS Requests? Oh, simple! They aid in finding web servers.",
        "You'll see as you go. Now, here's your first assignment."
    };


    // Start is called before the first frame update
    void Start()
    {
        speechText.text = dialogueSequence[speechIndex++];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (speechIndex == dialogueSequence.Length)
            {
                NextScene();
                return;
            }

            speechText.text = dialogueSequence[speechIndex++];
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            NextScene();
        }
    }

    void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
