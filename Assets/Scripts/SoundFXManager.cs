using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;

    [SerializeField] private AudioSource soundFXObject;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        //Spawnaa ‰‰niobjekti
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //audio clipin asetus
        audioSource.clip = audioClip;
        //volyymi 
        audioSource.volume = volume;
        //play sound
        audioSource.Play();
        //hakee ‰‰niclipi pituuden
        float clipLength = audioSource.clip.length;
        //tuhoaa ‰‰ni objektin kun ‰‰ni on soitettu
        Destroy(audioSource.gameObject, clipLength);
    }

    internal void PlaySoundFXClip(LayerMask scoreAddSound, Transform transform, float v)
    {
        throw new NotImplementedException();
    }
}
