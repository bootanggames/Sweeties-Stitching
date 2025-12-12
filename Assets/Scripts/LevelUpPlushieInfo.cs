using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class LevelUpPlushieInfo : MonoBehaviour
{
    public Image plushie;
    public TextMeshProUGUI plushieName;
    [SerializeField] AudioSource source;
    public GameObject effect;
    //private void OnEnable()
    //{
        
    //    effect.SetActive(true);
    //    effect.GetComponent<ParticleSystem>().Play();
    //    PlaySound();
    //}
    public void PlaySound()
    {
        if (source != null)
        {
            source.Stop();
            source.clip = null;
        }
        SoundManager.instance.PlaySound(source, SoundManager.instance.audioClips.ting, false, false, 1, false);
        HepticManager.instance.HapticEffect();
    }
}
