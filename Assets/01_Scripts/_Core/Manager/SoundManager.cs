using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class 
    SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private Dictionary<string, AudioClip> soundDict;
    [SerializeField] private AudioSource _bgmource;
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
            DontDestroyOnLoad(gameObject);
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
        _sfxVolumeSlider.value = arg0;
    }

    private void OnBgmVolumeChanged(float arg0)
    {
        _bgmVolumeSlider.value = arg0;
    }
#endregion

    private void Init()
    {
        soundDict = new Dictionary<string, AudioClip>();
        _bgmource.loop = true;

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
            if (_bgmource.clip != clip)
            {
                _bgmource.clip = clip;
                _bgmource.Play();
            }
            else
            {
                Debug.LogWarning("BGM Not Found");
            }
        }
    }
    #endregion
}
