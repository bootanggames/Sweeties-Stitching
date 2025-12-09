using UnityEngine;

public interface ISoundManager : IGameService
{
   SoundScriptable audioClips { get; }
   AudioSource audioSource {  get; }

}
