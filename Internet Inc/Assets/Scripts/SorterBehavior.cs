using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameplayManager;

public class SorterBehavior : MonoBehaviour
{
    [SerializeField] GameplayManager gameplayManager;
    DNSLevel thisLevel;

    [SerializeField] GameObject animatedText;

    // Start is called before the first frame update
    void Start()
    {
        thisLevel = gameplayManager.ThisLevel;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponent<Image>().color = new Color32(174, 174, 174, 255);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("Colliding with " + collision.name);

        if (Input.GetMouseButtonUp(0))
        {
            SortingObjectBehavior sortingObject = collision.GetComponent<SortingObjectBehavior>();
            if (sortingObject == null)
            {
                return;
            }

            string sbTarget = sortingObject.Target;
            string[] parts = sbTarget.Split('.');
            if (parts.Length != 3)
            {
                // Invalid
                return;
            }

            bool isCorrect = parts[(int)thisLevel] == Target;

            GameObject g = Instantiate(animatedText, transform.position + (Vector3.forward * 3), Quaternion.identity, GameObject.Find("Sorting Boxes").transform);
            g.transform.SetAsFirstSibling();
            Text textG = GameObject.Find("Correct Text").GetComponent<Text>();

            if (!isCorrect)
            {
                textG.text = "Incorrect!";
                textG.color = Color.red;
            }

            gameplayManager.NewSortAttempt(isCorrect);

            Debug.Log("Dropped " + collision.name + " ? " + isCorrect);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GetComponent<Image>().color = Color.white;
    }

    public string Target { get; set; }
}
