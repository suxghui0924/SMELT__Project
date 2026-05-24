using UnityEngine;
using UnityEngine.UI;

public class BossSceneHomeButton : MonoBehaviour
{
    [SerializeField] private Button homeBtn;
    [SerializeField] private Image homeBtnImage;
    public void OnClickHomeButton()
    {
        GameManager.instance.ChangeState(new HouseState());
        homeBtnImage.enabled = false;
        homeBtnImage.enabled = false;
    }
}
