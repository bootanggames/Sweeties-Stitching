using MoreMountains.NiceVibrations;
using UnityEngine;

public class HepticManager : Singleton<HepticManager>
{
    public override void SingletonAwake()
    {
        //MMVibrationManager.SetHapticsActive(true);
        base.SingletonAwake();
    }

    public override void SingletonOnDestroy()
    {
        base.SingletonOnDestroy();
    }

    public void HapticEffect()
    {
        MMVibrationManager.SetHapticsActive(true);
        MMVibrationManager.Haptic(HapticTypes.Success, false, true, this);
    }
}
