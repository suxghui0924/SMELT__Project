using UnityEngine;

public class AchievementClear : MonoBehaviour
{
    private double startTime;
    private double playTime;

    public AchievementSO[] achievementSOs;

    public static AchievementClear instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        startTime = Time.timeAsDouble;
    }
    private void Update()
    {
        playTime = Time.timeAsDouble - startTime;
        if (playTime >= 600) TenMinPlayed();
        if (playTime >= 1800) ThirtyMinPlayed();
        if (playTime >= 3600) OneHourPlayed();
    }
    #region 꼴보기 싫어요
    private void TenMinPlayed()
    {
        foreach (var achievement in achievementSOs)
        {
            if (achievement.achievementState == Achievements.TenMinPlayed && !achievement.clear)
            {
                AchievementManager.Instance.AchPopUp(achievement);
                achievement.clear = true;
            }
            else continue;
        }
    }
    private void ThirtyMinPlayed()
    {
        foreach (var achievement in achievementSOs)
        {
            if (achievement.achievementState == Achievements.ThirtyMinPlayed && !achievement.clear)
            {
                AchievementManager.Instance.AchPopUp(achievement);
                achievement.clear = true;
            }
            else continue;
        }
    }
    private void OneHourPlayed()
    {
        foreach (var achievement in achievementSOs)
        {
            if (achievement.achievementState == Achievements.ThirtyMinPlayed && !achievement.clear)
            {
                AchievementManager.Instance.AchPopUp(achievement);
                achievement.clear = true;
            }
            else continue;
        }
    }
    private void FirstSell()
    {
        foreach (var achievement in achievementSOs)
        {
            if (achievement.achievementState == Achievements.FirstSell && !achievement.clear && achievement.count == 0)
            {
                AchievementManager.Instance.AchPopUp(achievement);
                achievement.clear = true;
            }
            else continue;
        }
    }
    private void TenthSell()
    {
        foreach (var achievement in achievementSOs)
        {
            if (achievement.achievementState == Achievements.TenthSell && !achievement.clear && achievement.count == 10)
            {
                AchievementManager.Instance.AchPopUp(achievement);
                achievement.clear = true;
            }
            else continue;
        }
    }
    private void FirstMine()
    {
        foreach (var achievement in achievementSOs)
        {
            if (achievement.achievementState == Achievements.FirstMine && !achievement.clear && achievement.count == 0)
            {
                AchievementManager.Instance.AchPopUp(achievement);
                achievement.clear = true;
            }
            else continue;
        }
    }
    private void ThirdMine()
    {
        foreach (var achievement in achievementSOs)
        {
            if (achievement.achievementState == Achievements.ThirdMine && !achievement.clear && achievement.count == 3)
            {
                AchievementManager.Instance.AchPopUp(achievement);
                achievement.clear = true;
            }
            else continue;
        }
    }
    #endregion
}
