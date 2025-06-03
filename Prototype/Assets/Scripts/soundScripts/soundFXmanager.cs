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
        // spawn in gam object 
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        // play sound 
        audioSource.PlayOneShot(clip, volume);

        // get length of sound FX clip
        float clipLength = clip.length;

        Debug.Log("SOUND");

        // destroy clip after it's done playing 
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSoundFXClip(AudioClip[] clip, Transform spawnTransform, float volume)
    {
        // assign a random index
        int rand = Random.Range(0, clip.Length);
        
        // spawn in gam object 
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        // play sound 
        audioSource.PlayOneShot(clip[rand], volume);

        // get length of sound FX clip
        float clipLength = audioSource.clip.length;

        // destroy clip after it's done playing 
        Destroy(audioSource.gameObject, clipLength);
    }
}
