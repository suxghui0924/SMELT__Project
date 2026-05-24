using System;
using _01_Scripts._Core._States;
using _01_Scripts.Player.GameOver;
using UnityEngine;

namespace _01_Scripts.Player.TestDev
{
    public class TestDevControl : MonoBehaviour
    {
        #if UNITY_EDITOR
        private void Update()
        {
            #region timeScale
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                Time.timeScale = 10;
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                Time.timeScale = 1;
            }
            #endregion
            #region gold Change
            if (Input.GetKeyUp(KeyCode.Space))        InventoryManager.Instance.AddGold(100_000_000);
            #endregion
            #region fruit Change
            if (Input.GetKeyDown(KeyCode.Alpha1))       InventoryManager.Instance.AddItem("fruitstone_apple",  10);
            if (Input.GetKeyDown(KeyCode.Alpha2))       InventoryManager.Instance.AddItem("fruitstone_melon",  10);
            if (Input.GetKeyDown(KeyCode.Alpha3))       InventoryManager.Instance.AddItem("fruitstone_orange", 10);
            if (Input.GetKeyDown(KeyCode.Alpha4))       InventoryManager.Instance.AddItem("fruitstone_lemon",  10);
            if (Input.GetKeyDown(KeyCode.Alpha5))       InventoryManager.Instance.AddItem("fruitstone_grape",  10);
            #endregion
            #region SceneChange
            if (Input.GetKeyDown(KeyCode.Alpha6))
                GameManager.instance.ChangeState(new LobbyState());
            if (Input.GetKeyDown(KeyCode.Alpha7))
                GameManager.instance.ChangeState(new HouseState());
            if (Input.GetKeyDown(KeyCode.Alpha8))
                GameManager.instance.ChangeState(new BossState());
            if (Input.GetKeyDown(KeyCode.Alpha9))
                GameManager.instance.ChangeState(new GameOverState());
            if (Input.GetKeyDown(KeyCode.F1))
                GameManager.instance.ChangeState(new GameDieState());
            if (Input.GetKeyDown(KeyCode.F2))
                GameManager.instance.ChangeState(new TutorialState());
            if (Input.GetKeyDown(KeyCode.F3))
                GameManager.instance.ChangeState(new MiningState());
            #endregion
        }
        #endif
    }
}