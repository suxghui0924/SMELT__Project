using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class Upgrading : MonoBehaviour
{
    [SerializeField] private SOUpgrading upso;
    private void Start()
    {
        string upName = upso.UPname;
        float upTime = upso.UPtime;
    }
        
    private void OnMouseDown()
    {

    }
}
