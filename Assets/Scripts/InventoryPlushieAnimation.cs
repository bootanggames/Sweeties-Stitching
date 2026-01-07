using Coffee.UIExtensions;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPlushieAnimation : MonoBehaviour
{
    [SerializeField] RectTransform startPosition;
    [SerializeField] RectTransform targetPosition;
    [SerializeField] float transitionSpeed;
    [SerializeField] GameObject sparkleEffectPrefab;
    [SerializeField] Plushie_Details _plushieDetails;
    [SerializeField] PlushieShelfAnimation _shelfAnimation;
    Transform p_Transform;
    Transform parent;
    private void OnEnable()
    {
        p_Transform = this.GetComponentInParent<PlushieContainer>().transform;
        parent = this.GetComponentInParent<PlushiesInventory>().transform;

        PlayAnimation();
    }
    void PlayAnimation()
    {
        int levelIndex = PlayerPrefs.GetInt("Level");
        int plushieIndex = PlayerPrefs.GetInt("Level_" + levelIndex + "_Plushie");
        if ((plushieIndex - 1) >= 0)
        {
            if (LevelsHandler.instance)
            {
                Level_Metadata recentlyCompletedLevel = LevelsHandler.instance.levelStructure[levelIndex].plushie[plushieIndex - 1];
                string _plushieName = recentlyCompletedLevel.levelScriptable.levelName;
                if (_plushieDetails.plushieName.Equals(_plushieName))
                {
                    StartCoroutine(ActivatePlushie());
                }
            }

        }
    }
    public IEnumerator ActivatePlushie()
    {

        yield return new WaitForSeconds(0.25f);
        this.gameObject.SetActive(true);

        if (!_shelfAnimation.dontPlayAnimation)
        {
            GameObject effect = Instantiate(sparkleEffectPrefab);
            effect.transform.SetParent(targetPosition);
            effect.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

            RectTransform rt = this.GetComponent<RectTransform>();
            this.transform.SetParent(parent);
            startPosition.SetParent(parent);
            rt.anchoredPosition = startPosition.anchoredPosition;
            Sequence seq1 = DOTween.Sequence();
            Sequence seq2 = DOTween.Sequence();
            rt.anchorMin = targetPosition.anchorMin;
            rt.anchorMax = targetPosition.anchorMax;
            rt.pivot = targetPosition.pivot;
            yield return new WaitForSeconds(0.1f);

            seq1.Join(rt.DOAnchorPos(targetPosition.anchoredPosition, transitionSpeed).SetEase(Ease.Linear));
            seq1.Join(this.transform.DOScale(new Vector3(3.5f, 3.5f, 3.5f), transitionSpeed).SetEase(Ease.Linear));

            yield return seq1.WaitForCompletion();
            yield return new WaitForSeconds(0.5f);

            effect.gameObject.SetActive(true);
            effect.GetComponent<ParticleSystem>().Play();
            seq1.Kill();
            yield return new WaitForSeconds(1.0f);
            effect.transform.SetParent(this.transform);
            seq2.Join(rt.DOAnchorPos(startPosition.anchoredPosition, transitionSpeed).SetEase(Ease.Linear));
            seq2.Join(this.transform.DOScale(Vector3.one, transitionSpeed).SetEase(Ease.Linear));
            yield return seq2.WaitForCompletion();
            yield return new WaitForSeconds(1.0f);

            startPosition.SetParent(rt);
            effect.transform.SetParent(targetPosition);
            //effect.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            effect.GetComponent<UIParticle>().scale /= 2;
            effect.GetComponent<ParticleSystem>().Play();
            yield return new WaitForSeconds(1.5f);

            if (p_Transform)
            {
                rt.SetParent(p_Transform);
                p_Transform.GetComponent<GridLayoutGroup>().enabled = false;
                //yield return new WaitForSeconds(0.01f);
                p_Transform.GetComponent<GridLayoutGroup>().enabled = true;
            }
            Destroy(effect, 1);
            seq2.Kill();
            yield return new WaitForSeconds(0.5f);
            if (_shelfAnimation) StartCoroutine(_shelfAnimation.ShelfAnimation());
            StopCoroutine(ActivatePlushie());
        }
        else
            StopCoroutine(ActivatePlushie());
    }

    
}
