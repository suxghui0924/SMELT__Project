using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadingState : IGameState 
{   
    public void Enter()
    {
        UICanvasManager.instance.FadeStart();
        SceneManager.LoadScene("NewLoading");
    }

    public void Execute()
    {

    }

    public void Exit()
    {
    }
}
