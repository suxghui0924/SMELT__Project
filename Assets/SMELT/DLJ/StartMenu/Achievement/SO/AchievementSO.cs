using System;
using UnityEngine;

public enum Achievements
{
    FirstJoined,
    FirstSell,
    TenthSell,
    FirstMine,
    ThirdMine,
    TenMinPlayed,
    HundredThousandMoney,
    Millionaire,
    ThirtyMinPlayed,
    End,
    Suxghui,
    OneHourPlayed
}

[CreateAssetMenu(fileName = "AchievementDataSO", menuName = "ScriptableObject/AchievementData")]
public class AchievementSO : ScriptableObject
{
    public string achievementID;
    public string achievementDisplayName;
    public Achievements achievementState;
    public string achievementDescription;
    public bool clear; 
}
