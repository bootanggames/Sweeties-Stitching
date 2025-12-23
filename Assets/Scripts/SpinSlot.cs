using DG.Tweening;
using UnityEngine;

public class SpinSlot : MonoBehaviour
{
    [SerializeField] float iconHeight;
    [SerializeField] float spinSpeed;
    [SerializeField] int loopCount;

    [SerializeField] RectTransform rect;
    Tween spinTween;
    public float accelDuration = 0.3f;
    public float spinDuration = 1.2f;
    public float decelDuration = 0.5f;
    public void StartSpin()
    {
        StopImmediately();

        float distance = iconHeight * loopCount;

        Sequence seq = DOTween.Sequence();

        seq.Append(rect.DOAnchorPosY(rect.anchoredPosition.y - distance * 0.2f, accelDuration)
            .SetEase(Ease.OutQuad));

        seq.Append(rect.DOAnchorPosY(rect.anchoredPosition.y - distance * 0.6f, spinDuration)
            .SetEase(Ease.Linear));

        float targetY = Mathf.Round((rect.anchoredPosition.y - distance) / iconHeight) * iconHeight;
        seq.Append(rect.DOAnchorPosY(targetY, decelDuration).SetEase(Ease.OutBack));
    }
    public void SpinStop()
    {
        if (spinTween == null) return;
        spinTween.Kill();
        float yPos = Mathf.Round(rect.anchoredPosition.y / iconHeight) * iconHeight;

        rect.DOAnchorPosY(yPos, spinSpeed).SetEase(Ease.OutBack);
    }
    public void StopImmediately()
    {
        if (spinTween != null && spinTween.IsActive()) spinTween.Kill();
    }
}
