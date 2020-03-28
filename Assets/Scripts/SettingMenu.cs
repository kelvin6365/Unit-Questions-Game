using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    // Start is called before the first frame update
    UiSoundPlayer uiSoundPlayer;
    void Start()
    {
        uiSoundPlayer = FindObjectOfType<UiSoundPlayer>();
        if (this.gameObject.name == "MusicToggle")
        {
            this.gameObject.GetComponent<Toggle>().isOn = PlayerPrefs.GetString("MusicMute") == "0" ? false : true;
        }
        if (this.gameObject.name == "SoundToggle")
        {
            this.gameObject.GetComponent<Toggle>().isOn = PlayerPrefs.GetString("SFXMute") == "0" ? false : true;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MuteMusic()
    {
        uiSoundPlayer.MuteMusicVolume();
    }

    public void MuteSound()
    {
        uiSoundPlayer.MuteSFXVolume();
    }
}
