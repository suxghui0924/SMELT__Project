using _01_Scripts._Core._States;
using SMELT.LHS.LHS_Script.MiningSystem.Stamina;
using UnityEngine;

public class HomeButton : MonoBehaviour
{
    [SerializeField] private PlayerHitBox playerHitBox;
    [SerializeField] private PlayerAttack playerAttack;
    public void OnClickHomeButton()
    {
        playerAttack._skillCoolDown = 0.6f;
        for (int i = 1; i < 2; i++)
        {
            var scale = playerHitBox.transform.GetChild(i).localScale;
            scale.x = 0.6871f;
            playerHitBox.transform.GetChild(i).localScale = scale;
        }
        TimerAndReward.Instance.db = false;
        GameManager.instance.ChangeState(new HouseState());
    }
}
