using UnityEngine;
using System.Collections;

public class AutoDestroyAudio : MonoBehaviour
{
    public void Init(AudioClip clip, float volume)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = 0f;
        source.loop = false;
        source.playOnAwake = false;

        source.Play();
        StartCoroutine(DestroyAfterPlay(source, clip.length));
    }

    public void InitLoop(AudioClip clip, float volume)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = 0f;
        source.loop = true;
        source.playOnAwake = false;

        source.Play();
    }

    private IEnumerator DestroyAfterPlay(AudioSource source, float fallbackTime)
    {
        float timer = 0f;
        float maxTime = fallbackTime + 1f;

        yield return new WaitUntil(() =>
        {
            timer += Time.deltaTime;
            return !source.isPlaying || timer >= maxTime;
        });

        Destroy(gameObject);
    }
}
