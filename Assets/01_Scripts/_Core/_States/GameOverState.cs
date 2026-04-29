using Unity.VisualScripting;
using UnityEngine;

public class GameOverState : IGameState
{
    public void Enter()
    {
        UICanvasManager.instance.ControlObject(ObjectType.GameOver, true);
        UICanvasManager.instance.GetQty();
    }

    public void Execute()
    {
    }

    public void Exit()
    {
    }
}
