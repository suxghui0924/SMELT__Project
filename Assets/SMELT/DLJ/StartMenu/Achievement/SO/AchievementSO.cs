using System;
using UnityEngine;

public enum Achievements
{
    FirstJoined, //
    FirstSell, //
    TenthSell, //
    FirstMine, //
    ThirdMine, //
    TenMinPlayed, //
    HundredThousandMoney,
    Millionaire,
    ThirtyMinPlayed, //
    End,
    OneHourPlayed, //
    HundredPercentClear
}

[CreateAssetMenu(fileName = "AchievementDataSO", menuName = "ScriptableObject/AchievementData")]
public class AchievementSO : ScriptableObject
{
    public int achievementID;
    public string achievementDisplayName;
    public Achievements achievementState;
    public string achievementDescription;
    public Sprite achievementSprite;
    public bool clear;
    public int count;
}
