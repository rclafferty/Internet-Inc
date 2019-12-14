using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingObjectBehavior : MonoBehaviour
{
    Vector3 mousePosition;

    Vector3 initPosition;
    Quaternion initRotation;

    bool dragging;

    // Start is called before the first frame update
    void Start()
    {
        initPosition = transform.position;
        initRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (dragging)
        {
            transform.position = Input.mousePosition;
            transform.rotation = Quaternion.identity;
        }
    }

    public void MouseDown()
    {
        dragging = true;
    }

    public void MouseUp()
    {
        dragging = false;
        transform.position = initPosition;
        transform.rotation = initRotation;
    }

    public string Target { get; set; }
}
