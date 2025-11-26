using DG.Tweening;
using UnityEngine;

public class SpinAnimation : MonoBehaviour
{
    [SerializeField] Vector3 originalAngles;
    [SerializeField] Vector3 targetEularAngles;
    [SerializeField] Transform objTransform;
    [SerializeField] float rotationSpeed;
    Tween spinTween = null;
    private void OnEnable()
    {
        spinTween = transform
       .DORotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360)
       .SetLoops(-1)
       .SetEase(Ease.Linear)
       .Pause(); 
    }
    void Spining(Vector3 rotationVal)
    {
        spinTween = GameEvents.DoTweenAnimationHandlerEvents.onSpining.Raise(objTransform, rotationVal, rotationSpeed, Ease.Linear);
        spinTween.OnComplete(() =>
        {
            spinTween.Kill();
            spinTween = null;
            if (objTransform.localEulerAngles.Equals(rotationVal))
                Spining(originalAngles);
            else
                Spining(targetEularAngles);
        });

    }
}
