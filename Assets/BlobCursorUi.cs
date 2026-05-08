using UnityEngine;

public class BlobCursorUI : MonoBehaviour
{
    public MouseCatcher mouseCatcher;   // drag the GameObject with MouseCatcher here
    public RectTransform dot;           // drag your UI Image RectTransform here
    public bool hideWhenInactive = true;

    void Update()
    {
        if (mouseCatcher == null || dot == null) return;

        Vector2 p = mouseCatcher.CurrentPointerScreen;

        if (hideWhenInactive && mouseCatcher.useBlob && !mouseCatcher.blobActive)
        {
            dot.gameObject.SetActive(false);
            return;
        }

        dot.gameObject.SetActive(true);
        dot.position = p; // works for Screen Space - Overlay canvas
    }
}