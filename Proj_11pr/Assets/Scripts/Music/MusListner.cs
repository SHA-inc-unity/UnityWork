using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusListner : MonoBehaviour
{
    [SerializeField] List<AudioClip> clips;
    [SerializeField] AudioSource audioSource;

    private int i = 0;

    void Update()
    {
        if(audioSource.clip == null || audioSource.clip.length == audioSource.time)
        {
            i = (i + 1) % clips.Count;
            audioSource.clip = clips[i];
            audioSource.Play();
        }
    }
}
