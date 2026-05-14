using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class AchievementTab : MonoBehaviour
{
    [SerializeField] private GameObject AchievementCanvas;
    public bool AchievementCanvasState = false;

    private void Update()
    {
        foreach (var achievement in AchievementClear.instance.achievementSOs)
        {
            AchievementClear.instance.AchievementDark[achievement.achievementID].SetActive(!achievement.clear);
        }
    }
    public void ShowAchievement()
    {
        if (AchievementCanvasState) AchievementCanvasState = false;
        else AchievementCanvasState = true;
        AchievementCanvas.SetActive(AchievementCanvas);
        DLJ_SoundManager.instance.Play("Button");
    }

    public void LeaveAchievement()
    {
        AchievementCanvas.SetActive(false);
        DLJ_SoundManager.instance.Play("Button");
    }
}
