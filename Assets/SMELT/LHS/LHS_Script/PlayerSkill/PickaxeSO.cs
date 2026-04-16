using UnityEngine;

[CreateAssetMenu(fileName = "PickaxeSO", menuName = "Scriptable Objects/PickaxeSO")]
public class PickaxeSO : ScriptableObject
{
    public string PickaxeName;
    public int PickaxeDamage;
    public float PickaxeDPS;
}
