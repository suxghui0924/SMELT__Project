using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static string nextScene;

    public static void LoadScene(string SceneName)
    {
        //if (AchievementManager.Instance.GetAchievementState(Achievements.Money) == false)
        //{
        //    AchievementManager.Instance.AchievementClear(Achievements.Money);
        //}

        nextScene = SceneName;
        SceneManager.LoadScene("NewLoading");
    }

}
