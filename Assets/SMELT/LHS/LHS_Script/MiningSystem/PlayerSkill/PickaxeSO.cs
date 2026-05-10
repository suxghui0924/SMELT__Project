using UnityEngine;

[CreateAssetMenu(fileName = "PickaxeSO", menuName = "Scriptable Objects/PickaxeSO")]
public class PickaxeSO : ScriptableObject
{
    public string PickaxeName;
    public int PickaxeDamage;
    public float PickaxeDPS;

    [Header("Shop")]                                             // 추가
    public string pickaxeId;         // 곡괭이 고유 ID (ex: pickaxe_default) // 추가
    public int    goldCost;          // 구매에 필요한 골드        // 추가
    public string requiredItemId;    // 필요한 광석 ID (없으면 빈 문자열) // 추가
    public int    requiredItemAmount; // 필요한 광석 수량          // 추가
}
