using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : Singleton<GameHandler>
{
    public override void SingletonAwake()
    {
        base.SingletonAwake();
    }
    public override void SingletonOnDestroy()
    {
        base.SingletonOnDestroy();
    }
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameComplete()
    {
        GameCompleteSoundEffect();
    }

    void GameCompleteSoundEffect()
    {
        SoundManager.instance.ResetAudioSource();

        AudioSource _source = SoundManager.instance.audioSource;
        AudioClip _clip = SoundManager.instance.audioClips.completed;
        SoundManager.instance.PlaySound(_source, _clip, false, false, 1, false);
    }
}
