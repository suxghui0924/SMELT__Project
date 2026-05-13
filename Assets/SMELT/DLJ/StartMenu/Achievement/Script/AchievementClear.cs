using UnityEngine;

public class AchievementClear : MonoBehaviour
{
    private double startTime;
    private double playTime;

    public AchievementSO[] achievementSOs;

    public static AchievementClear instance;

    private ChangeTip changeTip;

    [SerializeField] public GameObject[] AchievementDark;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        startTime = Time.timeAsDouble;
        Debug.Log(1);
        ClearAchievement(Achievements.FirstJoined);
    }
    private void Update()
    {
        playTime = Time.timeAsDouble - startTime;
        if (playTime >= 600) ClearAchievement(Achievements.TenMinPlayed);
        if (playTime >= 1800) ClearAchievement(Achievements.ThirtyMinPlayed);
        if (playTime >= 3600) ClearAchievement(Achievements.OneHourPlayed);
    }
    public void ClearAchievement(Achievements state)
    {
        foreach (var achievement in achievementSOs)
        {
            achievement.count++;
            
            if (achievement.achievementState != state) continue;
            if (achievement.clear) continue;

            AchievementManager.Instance.AchPopUp(achievement);

            AchievementManager.Instance.AchievementClear(achievement.achievementState);

            achievement.clear = true;
            
            Debug.Log(achievement.achievementDisplayName);
            return;
        }
    }
}
