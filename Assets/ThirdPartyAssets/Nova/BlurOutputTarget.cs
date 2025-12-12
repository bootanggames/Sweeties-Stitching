using Nova;
using UnityEngine;
using UnityEngine.UI;

public class BlurOutputTarget : MonoBehaviour
{
    public UIBlock2D uiBlock;
    public RawImage rawImage;
    public Image image;

    private Material targetMaterial;
    private RenderTexture lastRT;

    public void SetTexture(RenderTexture rt)
    {
        lastRT = rt;
    }

    void Update()
    {
        if (lastRT == null) return;

        // 1. Nova Support
        if (uiBlock != null)
        {
            uiBlock.SetImage(lastRT);
            return;
        }

        // 2. RawImage Support (Most efficient for RenderTextures)
        if (rawImage != null)
        {
            rawImage.texture = lastRT;
            return;
        }

        // 3. Unity Image Support (Using Material instead of Sprite conversion)
        if (image != null)
        {
            ApplyToUnityImage(lastRT);
        }
    }

    private void ApplyToUnityImage(RenderTexture rt)
    {
        // If we don't have a material yet, create one based on the UI/Default shader
        if (targetMaterial == null)
        {
            targetMaterial = new Material(Shader.Find("UI/Default"));
            image.material = targetMaterial;
        }

        // Simply set the texture on the material. No CPU copying required!
        targetMaterial.mainTexture = rt;
    }

    private void OnDestroy()
    {
        // Clean up the created material to prevent memory leaks
        if (targetMaterial != null) Destroy(targetMaterial);
    }
}