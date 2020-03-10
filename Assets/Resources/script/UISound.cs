using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISound : MonoBehaviour
{
    [SerializeField] private AudioClip musicSource;
    [SerializeField] private AudioClip musicSource2;
    // Start is called before the first frame update
    public static UISound instance = null;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            UiSoundPlayer.Instance.PlayMusic(musicSource, 2.0f);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);

        }

    }


    // Update is called once per frame
    void Update()
    {

    }
}
