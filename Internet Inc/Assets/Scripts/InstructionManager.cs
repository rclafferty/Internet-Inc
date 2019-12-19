using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionManager : MonoBehaviour
{
    bool isFocused;

    // Start is called before the first frame update
    void Start()
    {
        isFocused = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isFocused)
        {
            Destroy(gameObject);
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        isFocused = focus;
    }
}
