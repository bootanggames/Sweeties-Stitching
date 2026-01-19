using System;
using TMPro;
using UnityEngine;

[Serializable]
public struct LevelUpData 
{
    [field: SerializeField] public GameObject levelUpScreen { get; private set; }
    [field: SerializeField] public GameObject levelUpFadeScreen { get; private set; }
    [field: SerializeField] public GameObject levelUpIntroScreen { get; private set; }
    [field: SerializeField] public TextMeshProUGUI levelScreenText { get; private set; }
    [field: SerializeField] public GameObject confettiCameraRenderObj { get; private set; }
    [field: SerializeField] public HomeScreenSound homeScreen { get; private set; }
    [field: SerializeField] public GameObject renderTextureImageObj { get; private set; }
    [field: SerializeField] public GameObject levelUpCamera { get; private set; }
    [field: SerializeField] public GameObject unlockedPlushieWord { get; private set; }
    [field: SerializeField] public TextMeshProUGUI levelNumber { get; private set; }
    [field: SerializeField] public GameObject homeCanvas { get; private set; }
}
