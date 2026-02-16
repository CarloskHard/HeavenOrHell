using UnityEngine;
using UnityEngine.Events;

// Esta línea permite crear este objeto haciendo click derecho en Project -> Create
[CreateAssetMenu(menuName = "Audio/Audio Event Channel")]
public class AudioEventChannel : ScriptableObject
{
    public UnityAction<AudioClip> OnMusicRequested;
    public UnityAction<AudioClip> OnSfxRequested;

    // Para la música
    public void RaiseMusicEvent(AudioClip musicClip)
    {
        OnMusicRequested?.Invoke(musicClip);
    }

    // Para efectos de sonido (SFX)
    public void RaiseSfxEvent(AudioClip sfxClip)
    {
        OnSfxRequested?.Invoke(sfxClip);
    }
}