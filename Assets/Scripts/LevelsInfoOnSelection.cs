using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TS.PageSlider;
using UnityEngine;

public class LevelsInfoOnSelection : MonoBehaviour
{
    public List<LevelObjectivePageDetail> levelPage;
    [field: SerializeField]public PageScroller pageScroller {  get; private set; }
    [SerializeField] PageSlider pageSlider;
    [SerializeField] GameObject sparkleEffectPrefab;
    [SerializeField] RectTransform targetPos;
    private void Start()
    {
        int levelUp = PlayerPrefs.GetInt("LevelUp");
        if(levelUp == 0)
            Invoke(nameof(GoToNextLevelPage), 0.5f);

        //StartCoroutine(AnimateCurrentUnlockedPlushie());
    }
    void GoToNextLevelPage()
    {
        int levelIndex = PlayerPrefs.GetInt("Level");
        //Debug.LogError(" " + levelIndex);
        NextPage(levelIndex);
        CancelInvoke(nameof(GoToNextLevelPage));
    }
    public void NextPage(int page)
    {
        if (pageScroller != null)
        {
            //var page = pageScroller._currentPage;
            //page++;
            if (page < pageSlider._pages.Count)
                pageScroller.ScrollToPage(page);
            else
                page = pageSlider._pages.Count - 1;
        }
    }
    public void PrevPage()
    {
        if (pageScroller != null)
        {
            var page = pageScroller._currentPage;
            page--;
            if (page >= 0)
                pageScroller.ScrollToPage(page);
            else
                page = 0;
        }
    }

    public IEnumerator AnimateCurrentUnlockedPlushie()
    {
        int levelIndex = PlayerPrefs.GetInt("Level");
        LevelObjectivePageDetail currentPage = levelPage[levelIndex];
        GameObject currentPlushie = null;
        int plushieIndex = PlayerPrefs.GetInt("Level_" + levelIndex + "_Plushie");
        int lockState = PlayerPrefs.GetInt("Level_" + levelIndex + "Plushie_" + plushieIndex);
        if (lockState == 1)
            currentPlushie = levelPage[levelIndex].levelDetail[plushieIndex].plushieObject;
        else
            currentPlushie = levelPage[levelIndex].levelDetail[0].plushieObject;
        RectTransform plushieRectTransform = currentPlushie.GetComponent<RectTransform>();
 
        GameObject effect = Instantiate(sparkleEffectPrefab);
        effect.transform.SetParent(this.transform);
        effect.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        //currentPlushie.transform.SetParent(this.transform);
        LevelDetail ld = levelPage[levelIndex].levelDetail[plushieIndex].levelObject.GetComponent<LevelDetail>();
        Vector3 startPos = ld.startPosition.localPosition;
        Debug.LogError(" " + startPos);
        Sequence seq1 = DOTween.Sequence();
        Sequence seq2 = DOTween.Sequence();
        yield return new WaitForSeconds(1);
        seq1.Join(plushieRectTransform.DOAnchorPos(targetPos.localPosition, 0.5f).SetEase(Ease.Linear));
        seq1.Join(currentPlushie.transform.DOScale(new Vector3(4, 4, 4), 0.25f).SetEase(Ease.Linear));
        seq1.OnComplete(() =>
        {
            effect.gameObject.SetActive(true);
            effect.GetComponent<ParticleSystem>().Play();
            seq1.Kill();

        });
        //yield return seq1.WaitForCompletion();
        //yield return new WaitForSeconds(1);

        //seq2.Join(plushieRectTransform.DOAnchorPos(ld.startPosition.localPosition, 1f).SetEase(Ease.Linear));
        //seq2.Join(currentPlushie.transform.DOScale(Vector3.one, 1f).SetEase(Ease.Linear));
        //seq2.OnComplete(() =>
        //{
        //    currentPlushie.transform.SetParent(currentPage.levelDetail[plushieIndex].levelObject.transform);

        //});
    }
}
