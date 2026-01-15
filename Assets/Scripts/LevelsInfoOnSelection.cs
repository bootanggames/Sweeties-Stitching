using Coffee.UIExtensions;
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
    [SerializeField] float transitionSpeed;
    IRoomdecorStore roomdecorStore;
    private void Start()
    {
        roomdecorStore = ServiceLocator.GetService<IRoomdecorStore>();
        //int levelUp = PlayerPrefs.GetInt("LevelUp");
        //if (levelUp == 0)
        //    
            
    }
    public void NextLevelPage()
    {
        Invoke(nameof(GoToNextLevelPage), 0.5f);
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
        int plushieIndex = PlayerPrefs.GetInt("Level_" + levelIndex + "_Plushie");
        LevelDetail ld = levelPage[levelIndex].levelDetail[plushieIndex].levelObject.GetComponent<LevelDetail>();
        ld.lockedImage.SetActive(true);

        yield return new WaitForSeconds(0.15f);
        GoToNextLevelPage();

        LevelObjectivePageDetail currentPage = levelPage[levelIndex];
        GameObject currentPlushie = null;
        int lockState = PlayerPrefs.GetInt("Level_" + levelIndex + "Plushie_" + plushieIndex);
        if (lockState == 1)
            currentPlushie = levelPage[levelIndex].levelDetail[plushieIndex].plushieObject;
        else
            currentPlushie = levelPage[levelIndex].levelDetail[0].plushieObject;
        RectTransform plushieRectTransform = currentPlushie.GetComponent<RectTransform>();
 
        GameObject effect = Instantiate(sparkleEffectPrefab);
        effect.transform.SetParent(this.transform);
        effect.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        yield return new WaitForSeconds(0.5f);

        currentPlushie.transform.SetParent(this.transform);
        ld.startPosition.SetParent(this.transform);

        Vector3 startPos = ld.startPosition.anchoredPosition3D;
        Sequence seq1 = DOTween.Sequence();
        Sequence seq2 = DOTween.Sequence();
        seq1.Join(plushieRectTransform.DOAnchorPos(targetPos.localPosition, transitionSpeed).SetEase(Ease.Linear));
        seq1.Join(currentPlushie.transform.DOScale(new Vector3(3.5f, 3.5f, 3.5f), transitionSpeed).SetEase(Ease.Linear));
        seq1.OnComplete(() =>
        {
            effect.gameObject.SetActive(true);
            effect.GetComponent<ParticleSystem>().Play();
            ld.locked = false;
            ld.lockedImage.SetActive(false);
            seq1.Kill();

        });
        yield return seq1.WaitForCompletion();
        yield return new WaitForSeconds(0.5f);

        seq2.Join(plushieRectTransform.DOAnchorPos(startPos, transitionSpeed).SetEase(Ease.Linear));
        seq2.Join(currentPlushie.transform.DOScale(Vector3.one, transitionSpeed).SetEase(Ease.Linear));
        yield return seq2.WaitForCompletion();
        yield return new WaitForSeconds(0.5f);

        effect.transform.SetParent(currentPlushie.transform);
        effect.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        effect.transform.SetParent(ld.transform);
        effect.GetComponent<UIParticle>().scale /= 2;
        ld.startPosition.SetParent(ld.transform);
        effect.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(1.5f);
        currentPlushie.transform.SetParent(ld.transform);
        MainMenuHandler.instance.screens.disableControlsScreen.SetActive(false);
        Destroy(effect, 1);
        seq2.Kill();
        //if (roomdecorStore != null)
        //    roomdecorStore.OpenScreen();
        StopCoroutine(AnimateCurrentUnlockedPlushie());
    }
}
