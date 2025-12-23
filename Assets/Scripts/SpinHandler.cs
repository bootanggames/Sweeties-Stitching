using System.Collections;
using UnityEngine;

public class SpinHandler : MonoBehaviour
{
    public SpinSlot[] reels;

    public void Spin()
    {
        StopAllCoroutines();
        StartCoroutine(SpinRoutine());
    }

    IEnumerator SpinRoutine()
    {
        foreach (var reel in reels)
            reel.StartSpin();

        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].SpinStop();
            yield return new WaitForSeconds(0.25f);
        }
    }
}
