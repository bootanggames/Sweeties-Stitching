using UnityEngine;

public static partial class GameEvents
{
    public static class SoundManagerEvents
    {
        public static readonly GameEvent<AudioSource, AudioClip, bool, bool, float, bool> OnSetAudioClip = new();
        public static readonly GameEvent<AudioSource> onStopAudio = new();
        public static readonly GameEvent onResetAudioSource = new();
    }
}
