using DG.Tweening;
using UnityEngine;

public class ChestTopRotation : MonoBehaviour
{
    [SerializeField] Vector3 targetEularAngle;
    [SerializeField] Transform chestTop;
    [SerializeField] float speed;
    [SerializeField] GameObject coinsEffectParent;
    [SerializeField] GameObject treasureBox;
    private void OnEnable()
    {
        chestTop.DOLocalRotate(targetEularAngle, speed).SetEase(Ease.Linear).OnComplete(() => {
            chestTop.DOPause();
            coinsEffectParent.SetActive(true);
            Invoke(nameof(DisableTreasureBox),1.5f);
        });
    }

    void DisableTreasureBox()
    {
        treasureBox.SetActive(false);
        CancelInvoke(nameof(DisableTreasureBox));
    }
}
