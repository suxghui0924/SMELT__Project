using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class AchievementTab : MonoBehaviour
{
    [SerializeField] private GameObject AchievementCanvas;
    public bool AchievementCanvasState = false;
    private GameObject AchievementCanvasObject;
    private void Update()
    {
        foreach (var achievement in AchievementClear.instance.achievementSOs)
        {
            AchievementCanvasObject = AchievementClear.instance.AchievementDark[achievement.achievementID];
            if(AchievementCanvasObject != null)
                AchievementCanvasObject.SetActive(!achievement.clear);
        }
    }
    public void ShowAchievement()
    {
        if (AchievementCanvasState) AchievementCanvasState = false;
        else AchievementCanvasState = true;
        AchievementCanvas.SetActive(AchievementCanvas);
    }

    public void LeaveAchievement()
    {
        AchievementCanvas.SetActive(false);
    }
}