using UnityEngine;

namespace _01_Scripts._Core._States
{
    public class TutorialState : IGameState
    {
        public void Enter()
        {
            UICanvasManager.instance.SetCanvasActive(CanvasType.Title, false);
            SceneLoader.LoadScene("TutorialScene");
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }
    }
}