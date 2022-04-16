using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    
    private AudioSource musicSource;
    private Slider volumeSlider;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        musicSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        musicSource.volume = volumeSlider.value;
    }

    public float GetVolume()
    {
        return volumeSlider.value;
    }

    public void SetVolume(float volume)
    {
        volumeSlider = GameObject.Find("Volume Slider").GetComponent<Slider>();
        volumeSlider.value = volume;
    }
}
