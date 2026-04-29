using UnityEngine;
using UnityEngine.SceneManagement;

public class MiningState : IGameState
{
    public void Enter()
    {
        GameManager.instance.ChangeState(new LoadingState());
        SceneManager.LoadScene("LHS_MiningScene");
    }

    public void Execute()
    {
    }

    public void Exit()
    {
    }
}
