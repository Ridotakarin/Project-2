using TMPro;
using UnityEngine;

public class TextFaceFix : MonoBehaviour
{
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void LateUpdate()
    {
        float parentScaleX = transform.parent.lossyScale.x;
        Vector3 newScale = originalScale;

        if (parentScaleX < 0)
        {
            newScale.x = -Mathf.Abs(originalScale.x);
        }
        else
        {
            newScale.x = Mathf.Abs(originalScale.x);
        }

        transform.localScale = newScale;
    }
}
