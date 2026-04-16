using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[CreateAssetMenu(fileName = "SOUpgrading", menuName = "Scriptable Objects/SOUpgrading")]
public class SOUpgrading : ScriptableObject
{
    public string needName;
    public string upName;
    public float upTime;
    public int needMoney;
}
