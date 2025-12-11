using FirstGearGames.SmoothCameraShaker;
using System.Collections.Generic;
using TMPro;
using TS.PageSlider;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpScreen : MonoBehaviour, ILevelUpScreen
{
    [field: SerializeField] public GameObject levelUpScreen {  get; private set; }
    [field:SerializeField] public GameObject levelUpFadeScreen {  get; private set; }
    [field:SerializeField] public GameObject levelUpIntroScreen {  get; private set; }
    [field: SerializeField] public TextMeshProUGUI levelScreenText {  get; private set; }
    [field: SerializeField] public GameObject confettiCameraRenderObj { get; private set; }
    [SerializeField] PageScroller pageScroller;
    [SerializeField] PageSlider pageSlider;
    [field: SerializeField] public List<PlushieSpriteContainer> pageSliderContainer {  get; private set; }
    [SerializeField] LevelUpPlushieInfo[] plushieInfo;
    [SerializeField] AudioSource audioSource;
    [field: SerializeField]public HomeScreenSound homeScreen {  get; private set; }
    [field: SerializeField] public GameObject renderTextureImageObj { get; private set; }
    [field: SerializeField] public GameObject levelUpCamera { get; private set; }
    [field: SerializeField] public ParticleSystem[] levelUpEffect { get; private set; }
    [field: SerializeField] public GameObject unlockedPlushieWord { get; private set; }
    [field: SerializeField] public TextMeshProUGUI levelNumber { get; private set; }

    [SerializeField] ShakerInstance _shakerInstance;
    [SerializeField] ShakeData shakeData;
    private void OnEnable()
    {
        RegisterService();
    }
    private void OnDisable()
    {
        UnRegisterService();
    }
    public void RegisterService()
    {
        ServiceLocator.RegisterService<ILevelUpScreen>(this);
    }

    public void UnRegisterService()
    {
        ServiceLocator.UnRegisterService<ILevelUpScreen>(this);
    }
    private void Start()
    {
        Invoke(nameof(GetLevel), 0.3f);
    }
    void GetLevel()
    {
        int levelIndex = PlayerPrefs.GetInt("Level");
        levelNumber.text = levelIndex.ToString();
        int levelUp = PlayerPrefs.GetInt("LevelUp");

        if (levelUp == 1)
        {
            levelScreenText.text = "Level" + levelIndex;
            levelUpScreen.SetActive(true);
            PlayLevelUpSound();
            foreach (PlushieSpriteContainer container in pageSliderContainer)
            {
                if(container.levelId.Equals(levelIndex + 1))
                {
                    for(int i=0;i<plushieInfo.Length;i++)
                    {
                        plushieInfo[i].plushie.sprite = container.plushieDetail[i].plushie;
                        plushieInfo[i].plushieName.text = container.plushieDetail[i].plushieName;
                    }
                    break;
                }
            }
            CameraShake2D();
            PlayerPrefs.SetInt("LevelUp", 0);
            renderTextureImageObj.SetActive(true);
            confettiCameraRenderObj.SetActive(false);
            //levelUpCamera.SetActive(true);
            foreach(ParticleSystem ps in levelUpEffect)
            {
                ps.gameObject.SetActive(true);
                ps.Play();
            }
        }
     
    }

    public void NextPage()
    {
        if (pageScroller != null)
        {
            var page = pageScroller._currentPage;
            page++;
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
    public void PlayLevelUpSound()
    {
        AudioClip _clip = SoundManager.instance.audioClips.levelUp;
        SoundManager.instance.PlaySound(audioSource, _clip, false, false, 1, false);
        HepticManager.instance.HapticEffect();

    }
    public void PlayCelebrationSound()
    {
        //AudioClip _clip = SoundManager.instance.audioClips.celebrationJingleTrumpets;
        //SoundManager.instance.PlaySound(audioSource, _clip, false, false, 1, false);
        HepticManager.instance.HapticEffect();
    }
    public void PlayLevelUpSongSound()
    {
        //AudioClip _clip = SoundManager.instance.audioClips.levelUpSong;
        //SoundManager.instance.PlaySound(audioSource, _clip, false, false, 1, false);
    }
    public void StopSound()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
   public void CameraShake2D()
    {
        ShakerInstance instance = CameraShakerHandler.Shake(shakeData);
        instance.Data.SetShakeCanvases(true);
        //instance.MultiplyMagnitude(_mass, -1);

    }
}
