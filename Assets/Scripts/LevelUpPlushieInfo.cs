using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPlushieInfo : MonoBehaviour
{
    public Image plushie;
    public TextMeshProUGUI plushieName;
    [SerializeField] AudioSource source;
    public GameObject effect;
    private void OnEnable()
    {
        PlaySound();
    }
    public void PlaySound()
    {
        SoundManager.instance.PlaySound(source, SoundManager.instance.audioClips.ting, false, false, 1, false);
        HepticManager.instance.HapticEffect();
    }
}
