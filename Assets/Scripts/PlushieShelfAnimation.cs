using DG.Tweening;
using System.Collections;
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
    IRoomdecorStore _roomdecorStore;
    [SerializeField] PlushiesInventory _plushieInventoryScreen;
    [SerializeField] GameObject homeBtn;
    [SerializeField] GameObject backBtn;
    [SerializeField] GameObject storeBtn;
    [SerializeField] GameObject rightArrowBtn;
    [SerializeField] GameObject leftArrowBtn;
    public bool dontPlayAnimation = false;
    private void OnEnable()
    {
        Invoke(nameof(SetReferences), 0.5f);
    }
    private void Start()
    {
        _roomdecorStore = ServiceLocator.GetService<IRoomdecorStore>();
    }
    void SetReferences()
    {
        shelfName = room.currentActiveShelf.itemName;
        //targetPosition = room.GetShelf(shelfName).GetComponent<RectTransform>();
        targetPosition = room.currentActiveShelf.targetPos;
        rectSize.x = startPosition.rect.width;
        rectSize.y = startPosition.rect.height;
        CancelInvoke("SetReferences");
    }
    public IEnumerator ShelfAnimation()
    {
        RectTransform rt = this.GetComponent<RectTransform>();
        rt.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, false, false, ShakeRandomnessMode.Full);

        yield return new WaitForSeconds(0.7f);
        Sequence seq1 = DOTween.Sequence();
        rt.SetParent(room.GetComponent<RectTransform>());
  
        SetAnchorsForShelfObject(rt, targetPosition);

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
     
        if (_roomdecorStore != null)
        {
            rt.gameObject.SetActive(true);
            _roomdecorStore.MyRoomButton();
            _plushieInventoryScreen.StopPlushieScreenSound();
            _plushieInventoryScreen.gameObject.SetActive(false);
            homeBtn.SetActive(true);
            backBtn.SetActive(true);
            storeBtn.SetActive(true);
            rightArrowBtn.SetActive(true);
            leftArrowBtn.SetActive(true);
            if (GameHandler.instance)
                GameHandler.instance.SwitchGameState(GameStates.RoomDecorScreen);

        }
        //PlayerPrefs.SetInt("OpenRoomDecor", 1);
        StopCoroutine(ShelfAnimation());
    }

    void SetAnchorsForShelfObject(RectTransform rt, RectTransform requiredTransform)
    {
        rt.anchorMin = requiredTransform.anchorMin;
        rt.anchorMax = requiredTransform.anchorMax;
        rt.pivot = requiredTransform.pivot;
    }

    public void ResetShelf()
    {
        RectTransform rt = this.GetComponent<RectTransform>();

        this.transform.localScale = Vector3.one;
        SetAnchorsForShelfObject(rt, startPosition);
        rt.right = startPosition.right;
        rt.offsetMin = startPosition.offsetMin;
        rt.offsetMax = startPosition.offsetMax;
        dontPlayAnimation = true;
    }
}
