using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAndDragBehavior : MonoBehaviour
{
    Rigidbody2D r2d;
    Vector3 mousePosition;
    Vector2 direction;
    float moveSpeed = 100f;
    float zDistance;

    Quaternion initRotation;
    Vector3 initPosition;

    // Start is called before the first frame update
    void Start()
    {
        r2d = GetComponent<Rigidbody2D>();

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
        transform.rotation = Quaternion.Euler(Vector3.zero);

        transform.GetComponent<SpriteRenderer>().sortingOrder = 100;
    }

    private void OnMouseUp()
    {
        transform.position = initPosition;
        transform.rotation = initRotation;

        transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
    }
}
