using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    private IGameState curState;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {

            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ChangeState(new GameOverState());
        }
    }


    public void ChangeState(IGameState newState)
    {
        if (curState != null)
            curState.Exit();
        curState = newState;
        curState.Enter();
    }
    public void BtnStart()
    {
        ChangeState(new LoadingState());
    }
}

