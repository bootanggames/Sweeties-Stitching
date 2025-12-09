using UnityEngine;

public class Singleton<T>:MonoBehaviour where T : Component
{
    public static T instance;
    public virtual void Awake()
    {
        if(instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        SingletonAwake();
    }
    public virtual void Start()
    {
        SingletonStart();
    }
    public virtual void OnDestroy()
    {
        if (instance != this)
            return;
       instance = null;
        SingletonOnDestroy();
    }
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
    public virtual void SingletonStart()
    {

    }
    public virtual void SingletonAwake()
    {

    }
    public virtual void SingletonOnDestroy()
    {

    }
}
