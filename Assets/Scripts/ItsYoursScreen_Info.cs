using UnityEngine;
using UnityEngine.UI;

public class ItsYoursScreen_Info : MonoBehaviour
{
    public Image itemIcon;
    [SerializeField] ParticleSystem starEffect;

    private void OnEnable()
    {
        starEffect.Play();
    }

}
