using UnityEngine;

public class HomeScreenSound : MonoBehaviour
{
    private void OnEnable()
    {
        EnableSound();
    }
    public void SetVolumeForBgMusic(float val)
    {
        AudiosSourceContainer.instance.homeScreen.volume = val;
    }
    public void EnableSound()
    {
        if(AudiosSourceContainer.instance != null)
        {
            SoundManager.instance.StopSound(AudiosSourceContainer.instance.plushieInventoryScreen);
            SoundManager.instance.PlaySound(AudiosSourceContainer.instance.homeScreen, SoundManager.instance.audioClips.bgMusic, true, false, 1.0f, true);

        }
    }
}
