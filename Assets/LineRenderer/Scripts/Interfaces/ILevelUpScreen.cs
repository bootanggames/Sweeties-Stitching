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
    void NextPage();
    void PlayLevelUpSound();
    void PlayCelebrationSound();
    void PlayLevelUpSongSound();
    void StopSound();
}
