using System;
using TMPro;
using UnityEngine;

namespace _01_Scripts.Player.GameOver
{
    public class GameDie : MonoBehaviour
    {
        private void OnEnable()
        {
            SoundManager.instance.PlayBGM("Gameover2");
        }

        public void OnButton()
        {
            UICanvasManager.instance.ControlObject(ObjectType.GameDie, false);
            GameManager.instance.ChangeState(new HouseState());
        }
    }
}