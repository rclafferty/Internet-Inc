using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortingObjectBehavior : MonoBehaviour
{
    [SerializeField] Rigidbody2D thisRigidbody;

    Vector3 mousePosition;
    float zDistance;

    Vector3 initPosition;
    Quaternion initRotation;

    [SerializeField] Text forwardFacingText;
    string target;

    // Start is called before the first frame update
    void Start()
    {
        initPosition = transform.position;
        initRotation = transform.rotation;

        zDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
    }

    private void OnMouseDrag()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistance));
        transform.position = new Vector3(worldPoint.x, worldPoint.y, transform.position.z);
        transform.rotation = Quaternion.identity;
    }

    private void OnMouseUp()
    {
        transform.position = initPosition;
        transform.rotation = initRotation;
    }

    public string Target {
        get
        {
            return target;
        }
        set
        {
            target = value;
            forwardFacingText.text = value;
        }
    }
}
