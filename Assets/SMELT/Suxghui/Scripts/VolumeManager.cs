using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager instance;

    [SerializeField] private Volume[] m_Volumes;
    private Volume global, heat, damage, ui;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        global = m_Volumes[0].GetComponent<Volume>();
        heat = m_Volumes[1].GetComponent<Volume>();
        damage = m_Volumes[2].GetComponent<Volume>();
        ui = m_Volumes[3].GetComponent<Volume>();
    }

    public void SetVolume(string str_name, float weight)
    {
        foreach (var vol in m_Volumes)
        {
            if (vol.name == str_name.FirstCharacterToUpper() + "_Volume")
            {
                vol.weight = weight;
            }
        }
    }
    public void VolumeChange(string str_name)
    {
        foreach(var vol in m_Volumes)
        {
            vol.weight = 0f;
            if(vol.name == str_name.FirstCharacterToUpper() + "_Volume")
            {
                vol.weight = 1f;
            }
        }
    }

    public void VolumeStart(String str_name,string dir, float wait)
    {
        foreach (var vol in m_Volumes)
        {
            if (vol.name == str_name.FirstCharacterToUpper() + "_Volume")
            {
                StartCoroutine(StartVolume(vol, dir, wait));
            }
        }
    }

    IEnumerator StartVolume(Volume vol, string dir, float wait)
    {
        yield return StartCoroutine(VolumeSet(vol, dir, 0, 1.0f, wait));
        yield return StartCoroutine(VolumeSet(vol, dir, 1.0f, 0, wait));
    }
    IEnumerator VolumeSet(Volume vol, string dir, float start, float end, float wait)
    {
        float dur = wait;
        float lens_Dur = wait / 5f;
        float time = 0;
        float startX = 0.5f;
        float targetX = 0.5f;
        if (vol.profile.TryGet<LensDistortion>(out var lens))
        {
            if (dir == "left")
            {
                startX = (startX == 0) ? .5f: 0;
                targetX = (targetX == 0) ? 0 : .5f;
            }
            else
            {
                startX = (startX == 0) ? .5f : 1f;
                targetX = (targetX == 0) ? 1f : .5f;
            }
            while (dur > time)
            {
                time += Time.deltaTime;
                vol.weight = Mathf.Lerp(start, end, time / dur);
                lens.center.value = new Vector2(Mathf.Lerp(startX, targetX, time / dur) , 0f);
                yield return null;
            }
        }
    }
}
