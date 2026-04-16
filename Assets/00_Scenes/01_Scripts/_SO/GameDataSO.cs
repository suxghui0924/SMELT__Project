using UnityEngine;

[CreateAssetMenu(fileName = "GameDataSO", menuName = "Scriptable Objects/GameDataSO")]
public class GameDataSO : ScriptableObject
{
    int PlayerHP = 5;
    public void Reset()
    {
        PlayerHP = 5;
    }
}
