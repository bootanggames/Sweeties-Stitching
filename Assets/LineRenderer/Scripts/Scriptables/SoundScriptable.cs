using UnityEngine;

[CreateAssetMenu(fileName ="Sound Clips Container",menuName ="AudioClipContainer/ClipContainer")]
public class SoundScriptable : ScriptableObject
{
    public AudioClip bgMusic;
    public AudioClip buttonClick;
    public AudioClip coinCollection;
    public AudioClip completed;
    public AudioClip sewing;
    public AudioClip purchaseComplete;
}
