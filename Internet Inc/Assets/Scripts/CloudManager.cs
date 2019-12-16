using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    [SerializeField] Sprite[] cloudSprites;
    [SerializeField] GameObject cloudPrefab;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnClouds());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnClouds()
    {
        while (true)
        {
            if (cloudPrefab.name == "Cloud_Office")
            {
                yield return new WaitForSeconds(10);
            }
            else
            {
                yield return new WaitForSeconds(40);
            }
            
            GameObject g = Instantiate(cloudPrefab);
            g.name = "Cloud";
            g.GetComponent<SpriteRenderer>().sprite = cloudSprites[Random.Range(0, cloudSprites.Length)];

            if (cloudPrefab.name == "Cloud_Office")
            {
                g.GetComponent<CloudBehavior>().speed = Random.Range(0.008f, 0.010f);
            }
            else
            {
                g.GetComponent<CloudBehavior>().speed = Random.Range(0.002f, 0.003f);
            }
            g.transform.position += (Vector3.up * Random.Range(-2.0f, 1.0f));
        }
    }
}
