using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "PickaxeDataSO", menuName = "PickaxeData/PickaxeDataSO")]
public class PickaxeDataSO : ScriptableObject
{
    // ��� , ���, ��, ���� ,����
    public Sprite icon;
    public new string name;
    public string desc;
    public int goldPrice;
    public int[] fruitPrice;
    public double hitboxSquare;
    public bool eqiuqed;
}
