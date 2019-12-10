using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SorterBehavior : MonoBehaviour
{
    [SerializeField] SortingManager sortingManager;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetMouseButtonUp(0))
        {
            SortingObjectBehavior sortingComponent = collision.GetComponent<SortingObjectBehavior>();
            if (sortingComponent == null)
            {
                return;
            }

            string sortingTarget = sortingComponent.Target;
            string[] parts = sortingTarget.Split('.');
            if (parts.Length != 3)
            {
                return;
            }

            bool isCorrect = false;

            // Put this in an if statement
            isCorrect = (parts[2] == Target);

            sortingManager.Sort(isCorrect);
            Debug.Log("Sorting attempt: " + isCorrect + "\t" + Target + " vs " + parts[2]);
        }
    }

    public string Target { get; set; }
}
