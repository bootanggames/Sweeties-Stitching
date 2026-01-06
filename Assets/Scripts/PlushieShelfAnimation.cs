using DG.Tweening;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class PlushieShelfAnimation : MonoBehaviour
{
    [SerializeField] UpdateRoom room;
    [SerializeField] Vector3 targetScale;
    [SerializeField] RectTransform targetPosition;
    [SerializeField] Vector3 originalScale;
    [SerializeField] RectTransform startPosition;
    [SerializeField] float transitionSpeed;
    [field: SerializeField] public ItemName shelfName {  get; private set; }
    [SerializeField] Vector2 rectSize;
    [SerializeField] RectTransform shelfParent;
    [SerializeField] float shakeDuration;
    [SerializeField] float shakeStrength;
    [SerializeField] int shakeVibrato;
    [SerializeField] float shakeRandomness;
    private void OnEnable()
    {
        Invoke(nameof(SetReferences), 0.5f);
    }
    void SetReferences()
    {
        shelfName = room.currentActiveShelf.itemName;
        //targetPosition = room.GetShelf(shelfName).GetComponent<RectTransform>();
        targetPosition = room.currentActiveShelf.targetPos;
        rectSize.x = startPosition.rect.width;
        rectSize.y = startPosition.rect.height;
        Debug.LogError(" " + startPosition.rect.width + " " + startPosition.rect.height);
        CancelInvoke("SetReferences");
    }
    public IEnumerator ShelfAnimation()
    {
        RectTransform rt = this.GetComponent<RectTransform>();
        rt.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, false, false, ShakeRandomnessMode.Full);

        yield return new WaitForSeconds(0.7f);
        Sequence seq1 = DOTween.Sequence();
        rt.SetParent(room.GetComponent<RectTransform>());
        rt.anchorMin = targetPosition.anchorMin;
        rt.anchorMax = targetPosition.anchorMax;
        rt.pivot = targetPosition.pivot;
 
        rt.sizeDelta = rectSize;
        rt.anchoredPosition3D = Vector3.zero;
        yield return new WaitForSeconds(0.5f);

        seq1.Join(rt.DOAnchorPos(targetPosition.anchoredPosition, transitionSpeed).SetEase(Ease.Linear));
        seq1.Join(this.transform.DOScale(targetScale, transitionSpeed).SetEase(Ease.Linear));
        yield return seq1.WaitForCompletion();
        yield return new WaitForSeconds(0.4f);
        seq1.Kill();
        this.gameObject.SetActive(false);
        rt.SetParent(shelfParent);
        this.transform.localScale = Vector3.one;
        rt.anchorMin = startPosition.anchorMin;
        rt.anchorMax = startPosition.anchorMax;
        rt.pivot = startPosition.pivot;
        rt.right = startPosition.right;
        rt.offsetMin = startPosition.offsetMin;
        rt.offsetMax = startPosition.offsetMax;
        //yield return new WaitForSeconds(0.3f);
        if (GameHandler.instance)
            GameHandler.instance.Home("HomeScreen");
        PlayerPrefs.SetInt("OpenRoomDecor", 1);
        StopCoroutine(ShelfAnimation());
    }
}
