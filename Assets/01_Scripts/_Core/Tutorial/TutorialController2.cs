using System;
using UnityEngine;

namespace _01_Scripts.Player.Tutorial
{
    public class TutorialController2 : MonoBehaviour
    {
        private void Start()
        {
            HandleFirstJoinCheck();
        }

        private void HandleFirstJoinCheck()
        {
            if (AchievementManager.Instance.AchievementStateDic.TryGetValue(Achievements.FirstJoined, out AchievementSO achievementSO))
            {
                if (!achievementSO.clear)
                {
                    UICanvasManager.instance.ControlObject(ObjectType.Tutorial, true);
                }
            }
        }
    }
}