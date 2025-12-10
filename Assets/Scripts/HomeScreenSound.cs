using UnityEngine;

public class HomeScreenSound : MonoBehaviour
{
    [SerializeField] AudioSource sourceForBgMusic;
    [SerializeField] AudioSource inventorySource;

    private void OnEnable()
    {
        EnableSound();
    }
    public void SetVolumeForBgMusic(float val)
    {
        sourceForBgMusic.volume = val;
    }
    public void EnableSound()
    {
        if (inventorySource)
            SoundManager.instance.StopSound(inventorySource);
        SoundManager.instance.PlaySound(sourceForBgMusic, SoundManager.instance.audioClips.bgMusic, true, false, 1.0f, true);

    }
}
