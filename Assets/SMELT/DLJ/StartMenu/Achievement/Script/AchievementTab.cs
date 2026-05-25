using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class AchievementTab : MonoBehaviour
{
    [SerializeField] private GameObject AchievementCanvas;
    public bool AchievementCanvasState = false;
    private GameObject _object;
    private void Update()
    {
        foreach (var achievement in AchievementClear.instance.achievementSOs)
        {
            _object = AchievementClear.instance.AchievementDark[achievement.achievementID];
            if ( _object != null)
                _object.SetActive(!achievement.clear);
        }
    }
    public void ShowAchievement()
    {
        AchievementCanvasState = !AchievementCanvasState;
        AchievementCanvas.SetActive(AchievementCanvasState);
    }

    public void LeaveAchievement()
    {
        AchievementCanvas.SetActive(false);
    }
}
