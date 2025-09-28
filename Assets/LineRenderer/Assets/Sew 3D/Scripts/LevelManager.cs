using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetSystems;
using System;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header(" Elements ")]
    [SerializeField] private GameObject[] levels;
    [SerializeField] private Transform levelsParent;
    [SerializeField] private RectTransform targetRect;
    [SerializeField] private ParticleControl coinParticles;
    [SerializeField] private ParticleSystem confetti;

    [Header(" Settings ")]
    private int level;


    [Header(" Events ")]
    public static Action OnNextLevelSet;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        LoadData();


        UIManager.onNextLevelButtonPressed += SetNextLevel;
        Sewable.OnComplete += SetLevelComplete;

        UIManager.onRetryButtonPressed += Retry;

    }

    private void OnDestroy()
    {
        UIManager.onNextLevelButtonPressed -= SetNextLevel;
        Sewable.OnComplete -= SetLevelComplete;

        UIManager.onRetryButtonPressed -= Retry;
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateLevel();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GenerateLevel()
    {
        int levelIndex = level % levels.Length;

        levelsParent.Clear();

        Instantiate(levels[levelIndex], levelsParent);
    }

    private void IncreaseLevelIndex()
    {
        level++;
        SaveData();
    }

    private void SetNextLevel()
    {
        Debug.Log("Next Level !!");
        IncreaseLevelIndex();

        OnNextLevelSet?.Invoke();

        SceneManager.LoadScene(0);

        //FindObjectOfType<GameManager>().FuelEmptyCallback();
    }

    private void SetLevelComplete()
    {
        confetti.Play();

        LeanTween.delayedCall(1.5f, () => coinParticles.PlayControlledParticles(Utils.GetScreenCenter(), targetRect, UnityEngine.Random.Range(10, 50)));

        LeanTween.delayedCall(3.5f, () => AdManager.instance.ShowInterstitialAd());
        
        LeanTween.delayedCall(4, () => UIManager.setLevelCompleteDelegate?.Invoke());
    }

    private void Retry()
    {
        SceneManager.LoadScene(0);
    }


    private void LoadData()
    {
        level = PlayerPrefs.GetInt("LEVEL");
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("LEVEL", level);
    }

    private void LevelPlusOne()
    {
        IncreaseLevelIndex();
    }

    private void ResetLevel()
    {
        PlayerPrefs.SetInt("LEVEL", 0);
    }
}
