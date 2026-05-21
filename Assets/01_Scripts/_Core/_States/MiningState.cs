using UnityEngine;
using UnityEngine.SceneManagement;

public class MiningState : IGameState
{
    public void Enter()
    {
        GameManager.instance.ChangeState(new LoadingState());
        SoundManager.instance.PlayBGM("Dungeon");
        UICanvasManager.instance.SetCanvasActive(CanvasType.Hud, false);
        SceneManager.LoadScene("LHS_MiningScene");
    }

    public void Execute()
    {
    }

    public void Exit()
    {
    }
}
