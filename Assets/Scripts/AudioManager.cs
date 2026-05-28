using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music")]
    public AudioClip song1;

    [Header("SFX")]
    public AudioClip MaxWin;
    public AudioClip Rolls;
    public AudioClip SmallWin;



    public static AudioManager instance;
    void Awake(){
       if(instance != null&& instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }


    void Start()
    {
    }





    public void PlayMusic(AudioClip clip){
        if (clip != null){
            musicSource.clip = clip;
            musicSource.Play();
        }
    }


    public void StopMusic(){
        musicSource.Stop();
    }



    public void PauseMusic(){
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    public void PlaySFX(AudioClip clip){
        if (clip != null){
            sfxSource.PlayOneShot(clip);
        }
    }
}

