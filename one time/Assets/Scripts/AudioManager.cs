using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {get; private set;}
    
    [SerializeField, Tooltip("An array of all sounds in the entire game.")]
    private Sound[] sounds;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // Destroy the duplicate
            Destroy(this);
        }

        // Dont destroy this object when restarting
        DontDestroyOnLoad(gameObject);

        // Loop through our array of sounds
        foreach (Sound s in sounds)
        {
            // Add an audio source component for that sound
            s.audioSource = gameObject.AddComponent<AudioSource>();
            s.audioSource.clip = s.audioClip;
            s.audioSource.volume = s.volume;
            s.audioSource.pitch = s.pitch;
            s.audioSource.loop = s.loop;
        }
    }

    private void Start()
    {
        // Play Main theme music
        PlaySound("MainTheme");
    }

    public void PlaySound(string name)
    {
        // Find the sound in the sounds array based on the name passed in 
        Sound sound = Array.Find(sounds, sound => sound.name == name);

        // Check if we found the sound
        if (sound == null)
        {
            Debug.LogWarning($"Could not find {name} sound!");
            return; // Stop the function
        }

        Debug.Log($"Playing {name} sound");
        // Play the sound
        sound.audioSource.Play();
    }

    public void StopSound(string name)
    {
        // Find the sound in the sounds array based on the name passed in 
        Sound sound = Array.Find(sounds, sound => sound.name == name);

        // Check if we found the sound
        if (sound == null)
        {
            Debug.LogWarning($"Could not find {name} sound!");
            return; // Stop the function
        }
        // Play the sound
        sound.audioSource.Stop();
    }
}