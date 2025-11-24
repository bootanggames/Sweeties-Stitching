using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [field: SerializeField] public SoundScriptable audioClips {  get; private set; }
    [field: SerializeField] public AudioSource audioSource { get; private set; }

    public override void SingletonAwake()
    {
        base.SingletonAwake();
    }
    public override void SingletonOnDestroy()
    {
        base.SingletonOnDestroy();
    }

    public void PlaySound(AudioSource source, AudioClip clip, bool loop, bool mute, float volume, bool playOnAwake)
    {
        source.clip = clip;
        source.loop = loop;
        source.mute = mute;
        source.volume = volume;
        source.playOnAwake = playOnAwake;
        source.Play();
    }

    public void StopSound(AudioSource source)
    {
        if(source.clip != null)
            source.Stop();
    }
    public void ResetAudioSource()
    {
        StopSound(audioSource);
        audioSource.clip = null;
    }

    public void ButtonClick()
    {
        ResetAudioSource();
        PlaySound(audioSource, audioClips.buttonClick, false, false, 1, false);
    }
}
