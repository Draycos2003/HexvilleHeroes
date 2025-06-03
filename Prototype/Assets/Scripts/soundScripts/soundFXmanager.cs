using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class soundFXmanager : MonoBehaviour
{
    public static soundFXmanager instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public void PlaySoundFXClip(AudioClip clip, Transform spawnTransform, float volume)
    {
        if (clip == null) return;

        GameObject temp = new GameObject("TempAudio");
        temp.transform.position = spawnTransform.position;

        AutoDestroyAudio auto = temp.AddComponent<AutoDestroyAudio>();
        auto.Init(clip, volume);
    }

    public void PlayRandomSoundFXClip(AudioClip[] clips, Transform spawnTransform, float volume)
    {
        if (clips == null || clips.Length == 0) return;

        int rand = Random.Range(0, clips.Length);
        PlaySoundFXClip(clips[rand], spawnTransform, volume);
    }

    public void PlaySoundMusic(AudioClip clip, Transform spawnTransform, float volume)
    {
        if (clip == null) return;

        GameObject temp = new GameObject("TempAudio");
        temp.transform.position = spawnTransform.position;

        AutoDestroyAudio auto = temp.AddComponent<AutoDestroyAudio>();
        auto.Init(clip, volume);
    }
}
