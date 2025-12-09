using DG.Tweening;
using UnityEngine;

public class Sparkle_Animation : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Ease ease;
    private void OnEnable()
    {
        float val = Random.Range(0.5f, 2);
        float scaleOut = Random.Range(0.5f, 1.5f);
        float newTarget = val + scaleOut;
        ScaleInOut(newTarget, val);
    }
    void ScaleInOut(float target, float scaleIn)
    {
        GameEvents.DoTweenAnimationHandlerEvents.onScaleAnimation.Raise(this.transform, scaleIn, target, speed, ease);
    }
}
