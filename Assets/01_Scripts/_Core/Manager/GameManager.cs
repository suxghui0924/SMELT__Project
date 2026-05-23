using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Lobby" && UICanvasManager.instance != null)
            UICanvasManager.instance.SetCanvasActive(CanvasType.Title, true);
        else if(scene.name == "House" && UICanvasManager.instance != null)
            UICanvasManager.instance.SetCanvasActive(CanvasType.Hud, true);
    }

    private void Start()
    {
        SoundManager.instance.PlayBGM("Lobby");
    }

    void Update()
    {
  
        if (Input.GetKey(KeyCode.RightShift))
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

// 