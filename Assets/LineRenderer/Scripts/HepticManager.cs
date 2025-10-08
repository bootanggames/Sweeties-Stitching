using MoreMountains.NiceVibrations;
using UnityEngine;

public class HepticManager : Singleton<HepticManager>
{
    public override void SingletonAwake()
    {
        base.SingletonAwake();
    }

    public override void SingletonOnDestroy()
    {
        base.SingletonOnDestroy();
    }

    public void HapticEffect()
    {
        MMVibrationManager.Haptic(HapticTypes.Success);
    }
}
