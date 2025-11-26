using DG.Tweening;
using UnityEngine;

public class SpinAnimation : MonoBehaviour
{
    [SerializeField] Transform objTransform;
    [SerializeField] float rotationSpeed;
    Tween spinTween = null;
    private void OnEnable()
    {
        Spining();
    }
    void Spining()
    {
        spinTween = GameEvents.DoTweenAnimationHandlerEvents.onSpining.Raise(objTransform, rotationSpeed, Ease.Linear);
        spinTween.Play();
    }
}
