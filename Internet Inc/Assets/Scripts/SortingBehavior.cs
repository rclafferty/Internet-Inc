using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingBehavior : MonoBehaviour
{
    Vector3 mousePosition;
    float zDistance;

    Quaternion initRotation;
    Vector3 initPosition;

    [SerializeField] string thisTarget;

    // Start is called before the first frame update
    void Start()
    {
        initPosition = transform.position;
        initRotation = transform.rotation;

        zDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        thisTarget = Target;
    }

    private void OnMouseDrag()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistance));
        transform.position = new Vector3(worldPoint.x, worldPoint.y, transform.position.z);
        transform.rotation = Quaternion.Euler(Vector3.zero);

        transform.GetComponent<SpriteRenderer>().sortingOrder = 10;
    }

    public void OnMouseUp()
    {
        transform.position = initPosition;
        transform.rotation = initRotation;

        transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
    }

    public string Target { get; set; }
}