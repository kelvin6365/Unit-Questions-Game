using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(AudioSource))]
public class UiSoundPlayer : MonoBehaviour
{
    [SerializeField] public Toggle MusicToggle;
    [SerializeField] public Toggle SFXToggle;

    private static UiSoundPlayer instance;
    public static UiSoundPlayer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UiSoundPlayer>();
                if (instance == null)
                {
                    instance = new GameObject("Spawned AudioManager", typeof(UiSoundPlayer)).GetComponent<UiSoundPlayer>();
                }

            }

            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    #region Fields
    private AudioSource musicSource;
    private AudioSource musicSource2;
    private AudioSource sfxSource;
    private bool firstMusicSourceIsPlaying;

    #endregion

    public static UiSoundPlayer GetInstance()
    {
        return instance;
    }


    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        // {
        //     instance = this;
        //     UIAS = GetCompent<AudioSource>();
        // }

        {
            DontDestroyOnLoad(this.gameObject);
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource2 = gameObject.AddComponent<AudioSource>();
            sfxSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource2.loop = true;
            if (PlayerPrefs.GetString("MusicMute") == "0")
            {
                musicSource.mute = true;
                musicSource2.mute = true;
                MusicToggle.isOn = false;
            }
            if (PlayerPrefs.GetString("SFXMute") == "0")
            {
                sfxSource.mute = true;
                SFXToggle.isOn = false;
            }
        }
    }

    // public void PlayUISound(AudioClip mySound)
    // {
    //     UIAS.PlayOneShot(mySound);
    // }

    public void PlayMusic(AudioClip musicClip, float transitionTime)
    {
        AudioSource activeSource = (firstMusicSourceIsPlaying) ? musicSource : musicSource2;
        activeSource.clip = musicClip;
        activeSource.Play();
        activeSource.volume = 0;


        StartCoroutine(UndateMusic(activeSource, transitionTime));

    }
    private IEnumerator UndateMusic(AudioSource activeSource, float transitionTime)
    {
        float t = 0.0f;
        for (t = 0.0f; t < transitionTime; t += Time.deltaTime)
        {
            activeSource.volume = (t / transitionTime);
            yield return null;
        }
    }
    public void PlayMusicWithFade(AudioClip newClip, float transitionTime = 1.0f)
    {
        AudioSource activeSource = (firstMusicSourceIsPlaying) ? musicSource : musicSource2;
        StartCoroutine(UndateMusicWithFade(activeSource, newClip, transitionTime));

    }

    public void PlayMusicWithCrossFade(AudioClip musicClip, float transitionTime = 1.0f)
    {
        AudioSource activeSource = (firstMusicSourceIsPlaying) ? musicSource : musicSource2;
        AudioSource newSource = (firstMusicSourceIsPlaying) ? musicSource2 : musicSource;

        firstMusicSourceIsPlaying = !firstMusicSourceIsPlaying;

        newSource.clip = musicClip;
        newSource.Play();
        StartCoroutine(UndateMusicWithCrossFade(activeSource, newSource, transitionTime));

    }
    private IEnumerator UndateMusicWithFade(AudioSource activeSource, AudioClip newClip, float transitionTime)
    {
        if (!activeSource.isPlaying)
        {
            activeSource.Play();
        }

        float t = 0.0f;

        for (t = 0.0f; t < transitionTime; t += Time.deltaTime)
        {
            activeSource.volume = (1 - (t / transitionTime));
            yield return null;
        }

        activeSource.Stop();
        activeSource.clip = newClip;
        activeSource.Play();

        for (t = 0.0f; t < transitionTime; t += Time.deltaTime)
        {
            activeSource.volume = (t / transitionTime);
            yield return null;
        }

    }

    private IEnumerator UndateMusicWithCrossFade(AudioSource original, AudioSource newSource, float transitionTime)
    {

        float t = 0.0f;

        for (t = 0.0f; t < transitionTime; t += Time.deltaTime)
        {
            original.volume = (1 - (t / transitionTime));
            newSource.volume = (t / transitionTime);
            yield return null;
        }

        original.Stop();
    }
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        sfxSource.PlayOneShot(clip, volume);
    }

    public void SetMusicVolume(float volume)
    {
        if (this.gameObject.GetComponent<Toggle>().isOn)
            musicSource.volume = volume;
        musicSource2.volume = volume;
    }


    public void MuteMusicVolume()
    {
        if (!MusicToggle.isOn)
        {
            musicSource.mute = true;
            musicSource2.mute = true;
        }
        else
        {
            musicSource.mute = false;
            musicSource2.mute = false;
        }
        PlayerPrefs.SetString("MusicMute", MusicToggle.isOn ? "1" : "0");
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
    public void MuteSFXVolume()
    {
        if (!SFXToggle.isOn)
        {
            sfxSource.mute = true;
        }
        else
        {
            sfxSource.mute = false;
        }
        PlayerPrefs.SetString("SFXMute", SFXToggle.isOn ? "1" : "0");
    }
}
