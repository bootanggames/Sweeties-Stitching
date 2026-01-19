using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface ILevelUpScreen : IGameService
{
    LevelUpData levelUpData{ get; }
    ParticleSystem[] levelUpEffect { get; }
    void NextPage(int levelNmbr);
    void PrevPage();
    void PlayLevelUpSound();
    void PlayCelebrationSound();
    void PlayLevelUpSongSound();
    void StopSound();
    void DisableParticleEffects();
}
