using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameplayManager2;

public class SorterBehavior2 : MonoBehaviour
{
    GameplayManager2 gameplayManager;
    DNSLevel thisLevel;

    // Start is called before the first frame update
    void Start()
    {
        gameplayManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager2>();
        thisLevel = gameplayManager.ThisLevel;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetMouseButtonUp(0))
        {
            ClickAndDragBehavior2 sb = collision.GetComponent<ClickAndDragBehavior2>();
            if (sb == null)
                return;

            string sbTarget = sb.Target;
            string[] parts = sbTarget.Split('.');
            if (parts.Length != 3)
                return;

            bool isCorrect = false;
            if (thisLevel == DNSLevel.TopLevel)
            {
                isCorrect = parts[2] == Target;
            }
            else if (thisLevel == DNSLevel.Authoritative)
            {
                isCorrect = parts[1] == Target;
            }
            /* else if (thisLevel == DNSLevel.Subdomain)
            {
                isCorrect = parts[0] == Target;
            } */

            gameplayManager.Sort(isCorrect);
        }
    }

    public string Target { get; set; }
}
