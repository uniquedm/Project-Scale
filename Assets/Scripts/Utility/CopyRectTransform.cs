using UnityEngine;

public class CopyRectTransform : MonoBehaviour
{
    public RectTransform sourceRectTransform;
    private RectTransform targetRectTransform;

    private void Awake()
    {
        targetRectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Check if both RectTransforms are assigned
        if (sourceRectTransform != null && targetRectTransform != null)
        {
            // Copy properties from source to target RectTransform
            targetRectTransform.sizeDelta = sourceRectTransform.sizeDelta;
        }
        else
        {
            Debug.LogError("Source or target RectTransform is not assigned!");
        }
    }
}
