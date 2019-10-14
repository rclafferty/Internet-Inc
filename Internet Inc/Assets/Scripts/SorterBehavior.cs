using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SorterBehavior : MonoBehaviour
{
    [SerializeField] GameplayManager gameplayManager;
    [SerializeField] public bool isAuthoritative;
    [SerializeField] public bool isTopLevel;
    [SerializeField] string thisTarget;

    // Start is called before the first frame update
    void Start()
    {
        gameplayManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();
    }

    // Update is called once per frame
    void Update()
    {
        thisTarget = Target;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // Debug.Log("Mouse up");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetMouseButtonUp(0))
        {
            SortingBehavior sb = collision.GetComponent<SortingBehavior>();
            if (sb == null)
                return;

            string sbTarget = sb.Target;
            string[] parts = sbTarget.Split('.');
            if (parts.Length != 3)
                return;

            if (isTopLevel)
            {
                if (parts[2] == Target)
                {
                    gameplayManager.CorrectSort();

                    Destroy(sb.gameObject);
                }
                else
                {
                    gameplayManager.IncorrectSort();
                }
            }
            else if (isAuthoritative)
            {
                if (parts[1] == Target)
                {
                    gameplayManager.CorrectSort();

                    Destroy(sb.gameObject);
                }
                else
                {
                    gameplayManager.IncorrectSort();
                }
            }
        }
    }

    public string Target { get; set; }
}
