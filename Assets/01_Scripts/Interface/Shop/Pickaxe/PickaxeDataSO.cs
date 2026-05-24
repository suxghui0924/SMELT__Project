using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "PickaxeDataSO", menuName = "PickaxeData/PickaxeDataSO")]
public class PickaxeDataSO : ScriptableObject
{
    public int index;
    public string pickaxeId;    // 곡괭이 고유 ID (ex: pickaxe_default) // 추가
    public Sprite icon;
    public new string name;
    public string desc;
    public int damage;
    public float speed;
    public ulong goldPrice;
    public int[] fruitPrice;
    public float hitboxSquare;
    public bool bought;
    public bool eqiuqed;
}
