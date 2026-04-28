using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadingState : IGameState
{
    public void Enter()
    {
        UICanvasManager.instance.FadeStart();
        UICanvasManager.instance.ControlObject(ObjectType.Loading, true);
    }

    public void Execute()
    {

    }

    public void Exit()
    {
    }
}
