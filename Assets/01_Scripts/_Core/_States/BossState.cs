using UnityEngine;
using UnityEngine.SceneManagement;

namespace _01_Scripts._Core._States
{
    public class BossState : IGameState
    {
        public void Enter()
        {
            SoundManager.instance.PlayBGM("Boss");
            UICanvasManager.instance.SetCanvasActive(CanvasType.Hud, false);
            SceneLoader.LoadScene("Work_boss_kgz");
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }
    }
}