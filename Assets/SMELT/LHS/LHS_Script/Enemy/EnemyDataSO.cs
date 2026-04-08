using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "Scriptable Objects/EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
    public float enemySpeed;
    public int enemyMaxHp;
    public int enemyDamage;
    public string enemyName;
}
