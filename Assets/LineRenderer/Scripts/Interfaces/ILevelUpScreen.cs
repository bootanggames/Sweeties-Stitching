using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface ILevelUpScreen : IGameService
{
    GameObject levelUpScreen {  get; }
    GameObject levelUpFadeScreen {  get; }
    GameObject levelUpIntroScreen {  get; }
    TextMeshProUGUI levelScreenText {  get; }
    GameObject confettiCameraRenderObj {  get; }
    HomeScreenSound homeScreen {  get; }
    GameObject renderTextureImageObj { get; }
    GameObject levelUpCamera { get; }
    ParticleSystem[] levelUpEffect { get; }
    List<PlushieSpriteContainer> pageSliderContainer {  get; }
    void NextPage();
    void PrevPage();
    void PlayLevelUpSound();
    void PlayCelebrationSound();
    void PlayLevelUpSongSound();
    void StopSound();
}
