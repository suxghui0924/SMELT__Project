using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public GameDataSO gameData;
    GameDataSO.GameState lastState;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            gameData.curState = GameDataSO.GameState.Lobby;
            lastState = GameDataSO.GameState.Lobby;
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
            ChangeState(GameDataSO.GameState.GameOver);
        }
    }


    public void ChangeState(GameDataSO.GameState newState)
    {
        if (lastState != newState)
        {
            gameData.ChangeGameState(newState);
            lastState = newState;
        }
        switch (newState)
        {
            case GameDataSO.GameState.Lobby:
                UICanvasManager.instance.Init();
                SceneManager.LoadScene("Lobby");
                break;
            case GameDataSO.GameState.Loading:
                UICanvasManager.instance.FadeStart();
                UICanvasManager.instance.ControlObject("System", 0, true);
                SceneManager.LoadScene("Loading");
                break;
            case GameDataSO.GameState.House:
                VolumeManager.instance.VolumeChange("global");
                SceneManager.LoadScene("House");
                break;
            case GameDataSO.GameState.Shop:
                SceneManager.LoadScene("Shop");
                break;
            case GameDataSO.GameState.Craft:
                //SceneManager.LoadScene("Craft");
                break;
            case GameDataSO.GameState.Mining:
                SceneManager.LoadScene("Mining");
                break;
            case GameDataSO.GameState.GameOver:
                UICanvasManager.instance.ControlObject("System", 2, true);
                UICanvasManager.instance.GetQty(); 
                break;
        }
    }
    public void BtnStart()
    {
        ChangeState(GameDataSO.GameState.Loading);
    }
}

