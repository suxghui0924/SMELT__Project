using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

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

    public void VolumeStart(String str_name)
    {
        Debug.Log("작동중");
        foreach (var vol in m_Volumes)
        {
            if (vol.name == str_name.FirstCharacterToUpper() + "_Volume")
            {
                vol.weight = 1f;
            }
        }
    }
}
