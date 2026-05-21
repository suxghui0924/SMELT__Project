using UnityEngine;

namespace _01_Scripts._Core._States
{
    public class GameDieState : IGameState
    {
        public void Enter()
        {
            Time.timeScale = 0.0f;
            UICanvasManager.instance.ControlObject(ObjectType.GameDie, true);
            
        }

        public void Execute()                          
        {
        }

        public void Exit()
        {
            Time.timeScale = 1.0f;
        }
    }
}