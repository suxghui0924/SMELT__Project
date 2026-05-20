using TMPro;
using UnityEngine;

namespace _01_Scripts.Player.GameOver
{
    public class GameDie : MonoBehaviour
    {
        public void OnButton()
        {
            UICanvasManager.instance.ControlObject(ObjectType.GameDie, false);
            GameManager.instance.ChangeState(new HouseState());
        }
    }
}