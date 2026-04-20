using System;
using UnityEngine;
using UnityEngine.Android;
using System.Reflection;
using UnityEngine.InputSystem.Processors;
[CreateAssetMenu(fileName = "GameDataSO", menuName = "Scriptable Objects/GameDataSO")]
public class GameDataSO : ScriptableObject
{
    public enum GameState { Lobby, Loading, House, Shop, Craft, Mining, GameOver };
    public GameState curState = GameState.Lobby;

    /*public int Gold;
    public int requiredPayment;
    public int curDay;
    public void ChangeFloatDate(string name, float value)
    {
        FieldInfo field = this.GetType().GetField(name);
        if (field != null)
        {
            field.SetValue(this, value);
        }
        else
        {
            Debug.Log("Float 변수형을 찾을 수 없습니다.");
        }
    }
    public void ChangeIntDate(string name, int value)
    {
        FieldInfo field = this.GetType().GetField(name);
        if (field != null)
        {
            field.SetValue(this, value);
        }
        else
        {
            Debug.Log("변수 찾을수 없습니다.");
        }
    }*/
    public void ChangeGameState(GameState newState)
    {
        curState = newState;
    }
    public void ResetDate()
    {
        if (InventoryManager.Instance != null)
        {
            curState = GameState.Lobby;
            /*            Gold = InventoryManager.Instance.Gold;
                        requiredPayment = InventoryManager.Instance.MaintenanceCost;
                        curDay = InventoryManager.Instance.CurrentDay;*/

        }
    }

    public void CheakPayment()
    {
        if (InventoryManager.Instance.EndOfDay())
        {
            Debug.Log("돈이 부족합니다, 게임 오버");
            ChangeGameState(GameState.GameOver);

        }
        else
        {
            Debug.Log("돈이 충분합니다, 다음날로 넘어갑니다");
        }
    }
}