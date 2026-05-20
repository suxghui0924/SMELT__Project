using UnityEngine;
using UnityEngine.SceneManagement;

public class HouseState : IGameState
{
    public void Enter()
    {
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
