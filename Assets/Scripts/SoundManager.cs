using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;
    [System.Serializable]
    public class sound
    {
        public string name;
        public List<AudioClip> clips;
    }

    private static SoundManager _instance = null;
 
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                return null;
            }
            else
            {
                return _instance;
            }
        }
    }
    private void Awake()
    {
        if (_instance != null)
        {
        }
        else
        {
            _instance = this;
        }
    }
    private void Start()
    {
        PlayBGM("NormalBattle");
    }
    public void PlayBGM(string name)
    {
        sound s = Array.Find(musicSounds,x => x.name == name);
        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            int rand = UnityEngine.Random.Range(0, s.clips.Count);
            musicSource.clip = s.clips[rand];
            musicSource.Play();
        }
    }
    public void PlaySFX(string name)
    {
        sound s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("SFX Sound not found");
        }
        else
        {
            int rand = UnityEngine.Random.Range(0, s.clips.Count);
            sfxSource.clip = s.clips[rand];
            sfxSource.Play();
        }
    }
}
