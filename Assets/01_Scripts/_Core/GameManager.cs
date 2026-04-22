using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set;}
    public GameDataSO gameData;
    GameDataSO.GameState lastState;
    void Awake()    
    {
        if(instance == null)
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
                SceneManager.LoadScene("Lobby");
                break;
            case GameDataSO.GameState.Loading:
                SceneManager.LoadScene("Loading");
                break;
            case GameDataSO.GameState.House:
                SceneManager.LoadScene("House");
                VolumeManager.instance.VolumeChange("global");
                break;
            case GameDataSO.GameState.Shop:
                SceneManager.LoadScene("Shop");
                break;
            case GameDataSO.GameState.Craft:
                SceneManager.LoadScene("Craft");
                break;
            case GameDataSO.GameState.Mining:
                SceneManager.LoadScene("Mining");
                break;
            case GameDataSO.GameState.GameOver:
                GameRetry.instance.GetQty();
                break;
        }
    }
    public void BtnStart()
    {
        ChangeState(GameDataSO.GameState.Loading);
    }
}

