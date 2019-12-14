using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIDragAndDrop : MonoBehaviour
{
    bool dragging;

    // Start is called before the first frame update
    void Start()
    {
        dragging = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (dragging)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void Drag()
    {
        dragging = !dragging;
    }
}
