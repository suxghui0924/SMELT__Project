using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

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

    public void VolumeStart(String str_name, float wait)
    {
        Debug.Log("작동중");
        foreach (var vol in m_Volumes)
        {
            if (vol.name == str_name.FirstCharacterToUpper() + "_Volume")
            {
                StartCoroutine(StartVolume(vol, wait));
            }
        }
    }

    IEnumerator StartVolume(Volume vol, float wait)
    {
        yield return StartCoroutine(VolumeSet(vol, 0, 1.0f, wait));
        yield return StartCoroutine(VolumeSet(vol, 1.0f, 0, wait));
    }
    IEnumerator VolumeSet(Volume vol, float start, float end, float wait)
    {
        float dur = wait;
        float time = 0;
        while (dur > time)
        {
            time += Time.deltaTime;

            vol.weight = Mathf.Lerp(start, end, time / dur);
            //vol.weight = Mathf.Lerp(start, end, time / dur * time / dur * (3f - 2f * time / dur));

            yield return null;
        }
    }
}
