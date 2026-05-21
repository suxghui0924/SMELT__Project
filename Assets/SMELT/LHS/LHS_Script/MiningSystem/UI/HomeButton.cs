using _01_Scripts._Core._States;
using SMELT.LHS.LHS_Script.MiningSystem.Stamina;
using UnityEngine;

public class HomeButton : MonoBehaviour
{
    public void OnClickHomeButton()
    {
        TimerAndReward.Instance.db = false;
        GameManager.instance.ChangeState(new HouseState());
    }
}
