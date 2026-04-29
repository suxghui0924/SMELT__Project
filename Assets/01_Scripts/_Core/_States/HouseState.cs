using UnityEngine;
using UnityEngine.SceneManagement;

public class HouseState : IGameState
{
    public void Enter()
    {
        VolumeManager.instance.VolumeChange("global");
        SceneManager.LoadScene("House");
        UICanvasManager.instance.ControlObject(ObjectType.Top, true);
        UICanvasManager.instance.ControlObject(ObjectType.Bottom, true);
    }

    public void Execute()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {

    }
}
