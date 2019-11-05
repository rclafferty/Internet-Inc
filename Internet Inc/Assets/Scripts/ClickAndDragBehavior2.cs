using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAndDragBehavior2 : MonoBehaviour
{
    Rigidbody2D thisRigidbody;

    Vector3 mousePosition;

    float zDistance;

    Vector3 initPosition;
    Quaternion initRotation;

    // Start is called before the first frame update
    void Start()
    {
        thisRigidbody = GetComponent<Rigidbody2D>();

        initPosition = transform.position;
        initRotation = transform.rotation;

        zDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDrag()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistance));
        transform.position = new Vector3(worldPoint.x, worldPoint.y, transform.position.z);
        transform.rotation = Quaternion.identity;

        // transform.GetComponent<SpriteRenderer>().sortingOrder = 100;
    }

    private void OnMouseUp()
    {
        transform.position = initPosition;
        transform.rotation = initRotation;

        // transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
    }

    public string Target { get; set; }
}
