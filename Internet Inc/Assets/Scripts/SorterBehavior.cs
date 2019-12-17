using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameplayManager;

public class SorterBehavior : MonoBehaviour
{
    [SerializeField] GameplayManager gameplayManager;
    [SerializeField] WebserverGameplayManager webGameplayManager;
    DNSLevel thisLevel;

    [SerializeField] GameObject animatedText;

    // Start is called before the first frame update
    void Start()
    {
        if (gameplayManager != null)
            thisLevel = gameplayManager.ThisLevel;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // GetComponent<Image>().color = new Color32(174, 174, 174, 255);
        CheckCollision(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        CheckCollision(collision);
    }

    void CheckCollision(Collider2D collision)
    { 
        // Debug.Log("Colliding with " + collision.name);

        if (Input.GetMouseButtonUp(0))
        {
            SortingObjectBehavior sortingObject = collision.GetComponent<SortingObjectBehavior>();
            if (sortingObject == null)
            {
                return;
            }

            // Debug.Log(sortingObject.name);
            string sbTarget = sortingObject.Target;

            string[] parts = null;
            bool isCorrect = false;

            if (SceneManager.GetActiveScene().name.Contains("web_server"))
            {
                parts = Target.Split('/');
                if (parts.Length != 2)
                {
                    return;
                } 

                isCorrect = sbTarget == parts[1];
                // Debug.Log(sbTarget + " - " + parts[1]);
            }
            else
            {

                parts = sbTarget.Split('.');
                if (parts.Length != 3)
                {
                    // Invalid
                    return;
                }

                isCorrect = parts[(int)thisLevel] == Target;
            }

            GameObject g = Instantiate(animatedText, transform.position + (Vector3.forward * 3), Quaternion.identity, GameObject.Find("Sorting Boxes").transform);
            g.transform.SetAsFirstSibling();
            Text textG = g.GetComponentInChildren<Text>();

            if (!isCorrect)
            {
                textG.text = "Incorrect!";
                textG.color = Color.red;
            }

            // Debug.Log("Sorter -- " + isCorrect);

            if (gameplayManager != null)
                gameplayManager.NewSortAttempt(isCorrect);
            else
                webGameplayManager.NewSortAttempt(isCorrect);

            // Debug.Log("Dropped " + collision.name + " ? " + isCorrect);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // GetComponent<Image>().color = Color.white;
    }

    public string Target { get; set; }
}
