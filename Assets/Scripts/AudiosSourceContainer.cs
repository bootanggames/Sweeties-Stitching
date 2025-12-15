using UnityEngine;

public class AudiosSourceContainer : Singleton<AudiosSourceContainer>
{
    public AudioSource homeScreen;
    public AudioSource plushieInventoryScreen;
    public AudioSource roomInventoryScreen;
    public override void SingletonAwake()
    {
        base.SingletonAwake();
    }
    public override void SingletonOnDestroy()
    {
        base.SingletonOnDestroy();
    }

}
