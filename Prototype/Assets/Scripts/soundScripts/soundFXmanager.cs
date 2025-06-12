using System.Collections;
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

    public void PlayPitched2DClip(AudioClip clip, float volume, float pitch)
    {
        if (clip == null) return;

        GameObject temp2D = new GameObject("TempAudioOneShotPitched2D");

        AudioSource src = temp2D.AddComponent<AudioSource>();
        src.spatialBlend = 0f;
        src.pitch = pitch;
        src.volume = volume;

        src.PlayOneShot(clip);

        Destroy(temp2D, (clip.length / pitch) + 0.1f);
    }

    public void PlaySoundMusic(AudioClip clip, Transform spawnTransform, float volume)
    {
        if (clip == null) return;

        GameObject temp = new GameObject("TempAudio");
        temp.transform.position = spawnTransform.position;

        AutoDestroyAudio auto = temp.AddComponent<AutoDestroyAudio>();
        auto.Init(clip, volume);
    }
    
    public void PlaySoundFX3DClip(AudioClip clip, Transform spawnTransform, float volume, float minDistance, float maxDistance)
    {
        if (clip == null) return;

        GameObject temp3D = new GameObject("TempAudio3D");
        temp3D.transform.position = spawnTransform.position;

        AudioSource src = temp3D.AddComponent<AudioSource>();
        src.spatialBlend = 1f;
        src.minDistance = minDistance;
        src.maxDistance = maxDistance;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.PlayOneShot(clip, volume);

        Destroy(temp3D, clip.length + 0.1f);
    }

    public void PlayRandomSoundFX3DClip(AudioClip[] clips, Transform spawnTransform, float volume, float minDistance, float maxDistance)
    {
        if (clips == null || clips.Length == 0) return;

        int rand = Random.Range(0, clips.Length);
        PlaySoundFX3DClip(clips[rand], spawnTransform, volume, minDistance, maxDistance);
    }

    public void PlayPitched3DClip(AudioClip clip, Transform spawnTransform, float volume, float pitch, float minDistance, float maxDistance)
    {
        if (clip == null) return;

        GameObject temp3D = new GameObject("TempAudioOneShotPitched3D");
        temp3D.transform.position = spawnTransform.position;

        AudioSource src = temp3D.AddComponent<AudioSource>();
        src.spatialBlend = 1f;
        src.minDistance = minDistance;
        src.maxDistance = maxDistance;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.pitch = pitch;

        src.PlayOneShot(clip, volume);

        Destroy(temp3D, (clip.length / pitch) + 0.1f);
    }

}
