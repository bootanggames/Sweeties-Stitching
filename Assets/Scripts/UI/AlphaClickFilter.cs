using UnityEngine;
using UnityEngine.UI;

public class AlphaClickFilter : MonoBehaviour
{
    [Range(0f, 1f)]
    public float alphaThreshold = 0.1f;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.alphaHitTestMinimumThreshold = alphaThreshold;
    }
}