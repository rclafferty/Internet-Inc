using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Image progressBarImage;
    [SerializeField] Text progressBarText;

    [SerializeField] float[] percentageThresholds;
    [SerializeField] Color[] thresholdColors;

    private float initialTextWidth;

    // Start is called before the first frame update
    void Start()
    {
        initialTextWidth = progressBarText.rectTransform.rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetProgressBar(float value)
    {
        if (value <= 0.1f)
        {
            progressBarImage.transform.localScale = new Vector3(0.1f, 1, 1);
            progressBarText.text = "0%";
        }
        else
        {
            progressBarImage.transform.localScale = new Vector3(value * 10, 1, 1);
            progressBarText.text = ((int)(value * 100)).ToString("##0") + "%";
        }

        // Calculate text width
        float barWidth = progressBarImage.rectTransform.rect.width * progressBarImage.rectTransform.localScale.x;
        float textWidth = progressBarText.rectTransform.rect.width;
        Rect textRect = progressBarText.rectTransform.rect;

        // Set text width
        if (barWidth > initialTextWidth)
        {
            textRect.width = barWidth;
            progressBarText.rectTransform.sizeDelta = new Vector2(textRect.width, textRect.height);
        }

        for (int i = 0; i < percentageThresholds.Length; i++)
        {
            if (value > percentageThresholds[i])
            {
                progressBarImage.color = thresholdColors[i];
                break;
            }
        }
    }
}
