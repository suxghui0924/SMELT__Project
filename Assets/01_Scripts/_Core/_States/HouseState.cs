using UnityEngine;
using UnityEngine.SceneManagement;

public class HouseState : IGameState
{
    public void Enter()
    {
        UICanvasManager.instance.SetCanvasActive(CanvasType.Title, false);
        VolumeManager.instance.VolumeChange(VolumeType.Global, 0.5f);
        SceneLoader.LoadScene("House");
    }

    public void Execute()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {

    }
}
