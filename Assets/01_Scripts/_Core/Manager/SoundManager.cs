using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class 
    SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public string currentHouseSong { get; private set; } = "game";

    private Dictionary<string, AudioClip> soundDict;
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;
    
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] audioClips;

    [Header("Slider")]
    [SerializeField] private Slider _bgmVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Init();
            _bgmVolumeSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
            _sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #region OnVolumeChanged
    private void OnSfxVolumeChanged(float arg0)
    {
        _sfxSource.volume = arg0;
    }

    private void OnBgmVolumeChanged(float arg0)
    {
        _bgmSource.volume = arg0;
    }
#endregion

    private void Init()
    {
        _sfxSource.volume = _sfxVolumeSlider.value;
        _bgmSource.volume = _bgmVolumeSlider.value; 
        soundDict = new Dictionary<string, AudioClip>();
        _bgmSource.loop = true;

        foreach (AudioClip clip in audioClips)
        {
            soundDict[clip.name] = clip;
        }
    }

    #region  GameManager PlaySFX & PlayBGM
    public void PlaySFX(string soundName)
    {
        if (soundDict.TryGetValue(soundName, out var clip))
        {
            _sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("SFX Not Found");
        }
    }

    public void PlayBGM(string bgmName)
    {
        if (soundDict.TryGetValue(bgmName, out var clip))
        {
            if (_bgmSource.clip != clip)
            {
                _bgmSource.clip = clip;
                _bgmSource.Play();
            }
            else
            {
                Debug.LogWarning("BGM Not Found");
            }
        }
    }
    #endregion
    
    public void SetHouseSong(string songName)
    {
        currentHouseSong = songName;
    }
}
