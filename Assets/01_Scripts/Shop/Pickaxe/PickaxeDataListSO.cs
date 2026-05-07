using UnityEngine;

[CreateAssetMenu(fileName = "PickaxeDataListSO", menuName = "Scriptable Objects/PickaxeDataListSO")]
public class PickaxeDataListSO : ScriptableObject
{
    public PickaxeDataSO[] pickaxeDataSO;

    public string[] fruitTypes;

}
