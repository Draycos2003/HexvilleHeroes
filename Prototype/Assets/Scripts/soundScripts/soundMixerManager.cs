using UnityEngine;
using UnityEngine.Audio;

public class soundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("masterVolume", level);
    }

    public void SetSoundFX(float level)
    {
        audioMixer.SetFloat("soundFXVolume", level);
    }

    public void SetMusic(float level)
    {
        audioMixer.SetFloat("musicVolume", level);
    }

}
