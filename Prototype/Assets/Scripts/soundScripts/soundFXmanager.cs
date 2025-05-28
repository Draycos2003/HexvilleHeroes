using System.Runtime.CompilerServices;
using UnityEngine;

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

        // assign the cilp
        audioSource.clip = clip;

        //assign volume 
        audioSource.volume = volume;

        // play sound 
        audioSource.Play();

        // get length of sound FX clip
        float clipLength = audioSource.clip.length;

        // destroy clip after it's done playing 
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSoundFXClip(AudioClip[] clip, Transform spawnTransform, float volume)
    {
        // assign a random index
        int rand = Random.Range(0, clip.Length);
        
        // spawn in gam object 
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        // assign the cilp
        audioSource.clip = clip[rand];

        //assign volume 
        audioSource.volume = volume;

        // play sound 
        audioSource.Play();

        // get length of sound FX clip
        float clipLength = audioSource.clip.length;

        // destroy clip after it's done playing 
        Destroy(audioSource.gameObject, clipLength);
    }
}
